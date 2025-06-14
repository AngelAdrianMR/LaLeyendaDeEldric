using UnityEngine;

// Tipos de ítems en el juego
public enum ItemType { Helmet ,Armor, Pants, Shoes, Gloves, Sword, Pickaxe, Axe, Potion, Material }

// Tipos de materiales usados para fabricar objetos
public enum MaterialType {None, Cobre, Hierro, Madera, Cuero, Carne }

// Clase que representa un objeto (ítem) del juego
[System.Serializable]
public class Item
{
    public string itemName;        // Nombre del objeto (interno, no localizado)
    public string description;     // Descripción general (no localizada)

    public ItemType itemType;      // Tipo de objeto (armadura, arma, herramienta, etc.)

    // Bonificadores aplicados al personaje al equiparlo
    public int healthBonus;        // Aumenta la vida máxima (ej: armadura, pantalones)
    public float speedBonus;       // Aumenta la velocidad (ej: botas)
    public int attackBonus;        // Aumenta el ataque (ej: guantes, espadas)

    // Valores para objetos consumibles (pociones)
    public int plusHealth;         // Vida que recupera
    public int plusStamina;        // Estamina que recupera

    // Banderas para herramientas o pociones
    public bool isPickaxe;         // Si es un pico
    public bool isAxe;             // Si es un hacha
    public bool isPotion;          // Si es una poción

    public float price;            // Precio en monedas del ítem
    public Sprite icon;            // Icono para mostrar en la interfaz

    public bool isCraftingMaterial; // Indica si este objeto se usa para fabricar algo
    public MaterialType materialType; // Tipo de material que representa (si aplica)

    public GameObject itemPrefab;  // Prefab del objeto (si es recogible en el mundo)

    // Claves para localización de textos (usadas con LanguageManager)
    public string keyName;         // Ej: "item_espada_hierro_name"
    public string keyDesc;         // Ej: "item_espada_hierro_desc"

    // Campos no serializados que se llenan al traducir con el sistema de idiomas
    [System.NonSerialized]
    public string localizedName;        // Nombre traducido al idioma actual
    [System.NonSerialized]
    public string localizedDescription; // Descripción traducida al idioma actual

    // Metodo para localizar las traducciones
    public void UpdateLocalization()
    {
        localizedName = LanguageManager.Instance.GetText(keyName);
        localizedDescription = LanguageManager.Instance.GetText(keyDesc);
    }


}
