using System;
using System.Collections.Generic;
using UnityEngine;

// Permite crear un nuevo objeto tipo Recipe desde el menú de Unity (clic derecho → Create → Crafting → Recipe)
[CreateAssetMenu(fileName = "NewRecipe", menuName = "Crafting/Recipe", order = 1)] // Esto te permitirá crear una receta desde el menú de Unity
public class Recipe : ScriptableObject
{
    // Prefab del objeto que se obtendrá al fabricar esta receta
    public GameObject resultPrefab;

    // Lista de materiales requeridos
    public List<MaterialRequired> materialsRequired; 

    // Clase anidada que representa cada material necesario
    [System.Serializable]
    public class MaterialRequired
    {
        public MaterialType materialType; // Prefab del material
        public int amount; // Cantidad necesaria del material
    }
}


