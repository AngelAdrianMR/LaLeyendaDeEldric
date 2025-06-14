using System.Collections.Generic;
using UnityEngine;

// Permite crear este ScriptableObject desde el menú en Unity
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    // Lista que contiene todos los objetos del juego
    public List<Item> allItems = new List<Item>();
}
