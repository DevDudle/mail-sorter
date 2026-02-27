using System;
using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Настройки спавна")]
    [SerializeField] private float cooldown = 30f;           // Текущее время между посылками
    [SerializeField] private float minCooldown = 1f;         // Минимальное время (сложность)
    [SerializeField] private float cooldownDecreaseRate = 0.1f; // На сколько уменьшать таймер
    [SerializeField] private GameObject packagePrefab;       // Префаб коробки
    [SerializeField] private string[] cities = { "Брест", "Кобрин", "Барановичи", "Пинск", "Пружаны" };

    [Header("Ссылки")]
    [SerializeField] private SpawnTable spawnTable;

    // Ивенты
    public static Action PackagesOverflowEvent; // Штраф
    public static Action<float> TimerUpdateEvent; // Обновление UI таймера

    // Ключи для сохранения
    private const string KEY_COOLDOWN = "Spawn_Cooldown";
    private const string KEY_NEXT_TIME = "Spawn_NextTime";

    // Сохраняем сложность (кулдаун) при выходе со сцены
    void OnDisable()
    {
        PlayerPrefs.SetFloat(KEY_COOLDOWN, cooldown);
        PlayerPrefs.Save();
    }

    void Start()
    {
        // Если забыл привязать стол в инспекторе — ищем сами
        if (spawnTable == null)
            spawnTable = FindFirstObjectByType<SpawnTable>();

        // Загружаем прогресс сложности
        if (PlayerPrefs.HasKey(KEY_COOLDOWN))
            cooldown = PlayerPrefs.GetFloat(KEY_COOLDOWN);

        // ВАЖНО: Запускаем инициализацию через корутину.
        // Это дает задержку в 1 кадр, чтобы ShelfSaveManager успел положить коробку на стол.
        StartCoroutine(StartupRoutine());
    }

    IEnumerator StartupRoutine()
    {
        // Ждем 1 кадр. В это время отрабатывает Start у ShelfSaveManager.
        yield return null;

        // Вычисляем, сколько времени осталось ждать (учитывая поход в магазин)
        float delay = CalculateInitialDelay();

        // Запускаем основной цикл спавна
        StartCoroutine(SpawnLoop(delay));
    }

    // Расчет времени: (Когда должен быть спавн) МИНУС (Текущее время)
    private float CalculateInitialDelay()
    {
        if (PlayerPrefs.HasKey(KEY_NEXT_TIME))
        {
            long ticks = Convert.ToInt64(PlayerPrefs.GetString(KEY_NEXT_TIME));
            DateTime targetTime = new DateTime(ticks);
            TimeSpan diff = targetTime - DateTime.Now;

            // Если время вышло, вернем 0. Если нет — вернем остаток секунд.
            return (float)Math.Max(0, diff.TotalSeconds);
        }
        // Если ключа нет (новая игра), спавним сразу (0 сек)
        return 0f;
    }

    IEnumerator SpawnLoop(float initialDelay)
    {
        // --- ФАЗА 1: Вход в сцену ---

        if (initialDelay > 0)
        {
            // Сценарий А: Мы вернулись из магазина, таймер еще тикает. Ждем.
            yield return StartCoroutine(WaitRoutine(initialDelay));
            TrySpawnPackage();
        }
        else
        {
            // Сценарий Б: Новая игра ИЛИ время вышло, пока мы были в магазине.
            // Проверяем: если стол ПУСТОЙ — спавним.
            // Если на столе УЖЕ лежит коробка (загрузилась из сохранения) — НЕ спавним, НЕ штрафуем.
            if (spawnTable != null && !spawnTable.HasPackage)
            {
                TrySpawnPackage();
            }
        }

        // --- ФАЗА 2: Бесконечный цикл ---
        while (true)
        {
            // Ждем полный кулдаун
            yield return StartCoroutine(WaitRoutine(cooldown));
            // Пытаемся заспавнить
            TrySpawnPackage();
        }
    }

    // Логика ожидания + обновление UI
    IEnumerator WaitRoutine(float duration)
    {
        // Сразу сохраняем время, когда таймер должен закончиться.
        // Если игрок выйдет из игры посередине таймера, мы узнаем об этом.
        DateTime target = DateTime.Now.AddSeconds(duration);
        PlayerPrefs.SetString(KEY_NEXT_TIME, target.Ticks.ToString());
        PlayerPrefs.Save();

        float remaining = duration;
        while (remaining > 0)
        {
            TimerUpdateEvent?.Invoke(remaining); // Обновляем текст на экране
            yield return null;
            remaining -= Time.deltaTime;
        }
        TimerUpdateEvent?.Invoke(0);
    }

    // Главная логика проверки
    void TrySpawnPackage()
    {
        if (spawnTable == null) return;

        // ПРЯМАЯ ПРОВЕРКА СТОЛА (самый надежный способ)
        if (!spawnTable.HasPackage)
        {
            // Стол пуст -> Спавним новую
            SpawnPackage();

            // Усложняем игру (уменьшаем время ожидания)
            cooldown = Mathf.Max(minCooldown, cooldown - cooldownDecreaseRate);
        }
        else
        {
            // Стол занят -> Игрок не успел убрать коробку -> ШТРАФ
            // (Событие штрафа должно быть обработано в GameManager или EconomyManager)
            Debug.Log("Штраф! Посылка не убрана вовремя.");
            PackagesOverflowEvent?.Invoke();
        }
    }

    void SpawnPackage()
    {
        GameObject obj = Instantiate(packagePrefab);
        Package pkg = obj.GetComponent<Package>();

        // Выбираем случайный город
        string randomCity = cities[UnityEngine.Random.Range(0, cities.Length)];
        pkg.SetDestination(randomCity);

        // Кладем на стол (внутри SetPackage коробка телепортируется и включается физика)
        spawnTable.SetPackage(pkg);
    }

    // --- ВАЖНО ---
    // Вызови этот метод на кнопке "НОВАЯ ИГРА" в главном меню!
    // Иначе игра начнется с таймером и сложностью от прошлой сессии.
    public static void ResetData()
    {
        PlayerPrefs.DeleteKey(KEY_COOLDOWN);
        PlayerPrefs.DeleteKey(KEY_NEXT_TIME);

        // Удаляем сохранения полок тоже, чтобы начать с чистого листа
        PlayerPrefs.DeleteKey("ShelfData");

        // Ключи улучшений (Таймер, Скорость) НЕ удаляем, если они должны сохраняться навсегда.
        // Если улучшения тоже нужно сбросить - удали ключи "SpeedLevel", "BoxCooldownUpgrade" и т.д.

        PlayerPrefs.Save();
    }
}