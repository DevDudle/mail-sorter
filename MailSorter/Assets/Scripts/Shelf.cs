using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Shelf : MonoBehaviour
{
    [SerializeField] private ShelfSlot[] slots;

    void Awake()
    {
        if (slots == null || slots.Length == 0)
        {
            slots = GetComponentsInChildren<ShelfSlot>();
        }
        Debug.Log($"Стеллаж {name}: найдено {slots.Length} слотов");
    }

    public bool HasFreeSlot()
    {
        return slots.Any(s => !s.IsOccupied);
    }

    public int GetFreeSlotCount()
    {
        return slots.Count(s => !s.IsOccupied);
    }

    public List<ShelfSlot> GetOccupiedSlots()
    {
        return slots.Where(s => s.IsOccupied).ToList();
    }
}