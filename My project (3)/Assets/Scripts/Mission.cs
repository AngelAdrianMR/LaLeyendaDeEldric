using UnityEngine;
using System;

[System.Serializable] // Permite que la clase sea visible y editable en el Inspector (si se usa en un ScriptableObject o JSON)
public class Mission
{
    public string id;                    // ID única de la misión (clave de referencia)
    public string missionName;          // Nombre visible de la misión
    public string description;          // Descripción de la misión (lo que debe hacer el jugador)
    public int coinReward = 0;          // Recompensa en monedas al completar

    public string requieredItemName;    // Nombre del ítem necesario para completar la misión (si aplica)
    public bool requieredCrafting = false; // Indica si la misión requiere fabricar un objeto

    public bool isActive = false;       // ¿Está activa actualmente?
    public bool isCompleted = false;    // ¿Está completada?

    public string nextMissionId;        // ID de la siguiente misión (para cadenas de misiones)

    // Referencia a las claves de las traducciones en los json
    public string keyName;
    public string keyDescription;

    [NonSerialized] public string localizedName;
    [NonSerialized] public string localizedDesc;


    // Constructor para crear una misión con su ID, nombre y descripción
    public Mission(string id, string name, string description)
    {
        this.id = id;
        this.missionName = name;
        this.description = description;
    }

    // Método para activar la misión
    public void Activate()
    {
        isActive = true;
        isCompleted = false;
    }

    // Método que la desactiva y la marca como terminada
    public void Complete()
    {
        isActive = false;
        isCompleted = true;
    }

    // Método para traducir el texto de la misión
    public void Translate()
    {
        Debug.Log($"Traduciendo misión: {id}");

        if (!string.IsNullOrEmpty(keyName))
        {
            localizedName = LanguageManager.Instance.GetText(keyName);
        }

        if (!string.IsNullOrEmpty(keyDescription))
        {
            localizedDesc = LanguageManager.Instance.GetText(keyDescription);
        }
    }

}
