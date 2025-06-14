// Clase que representa una entrada individual en el ranking
[System.Serializable]
public class RankingEntry
{
    public string name;   // Nombre del jugador o personaje
    public int attack;    // Valor de ataque (puede representar puntuación u otra métrica)
}

// Clase que agrupa todas las entradas del ranking
[System.Serializable]
public class RankingList
{
    public RankingEntry[] items; // Array de entradas en el ranking
}
