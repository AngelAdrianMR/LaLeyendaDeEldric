using System.Collections.Generic;
using UnityEngine;

// Permite crear esta base de datos desde el menú de Unity:
[CreateAssetMenu(fileName = "MissionDatabase", menuName = "Misiones/Mission Database")]
public class MissionDatabase : ScriptableObject
{
    // Lista de todas las misiones disponibles en el juego
    public List<Mission> allMissions;

    // Método que busca y devuelve una misión por su ID
    public Mission GetMissionById(string id)
    {
        return allMissions.Find(m => m.id == id);
    }

    // Traducimos todas las misiones para mostrarlas en el panel
    public void TranslateAll()
    {
        foreach (var mission in allMissions)
            mission.Translate();
    }

}
