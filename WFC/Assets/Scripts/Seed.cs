using UnityEngine;

[System.Serializable]
public class Seed
{
    [SerializeField]
    private int seed;
    private int currentValue;
    // Offset for the next WFC seed
    // Could be increased if generation fails very often
    private const int incrementValue = 10;

    public void Reset()
    {
        currentValue = seed;
    }

    public int Next()
    {
        return seed == 0 ? 0 : currentValue += incrementValue;
    }

    public void IncrementSeed()
    {
        ++seed;
        currentValue = seed;
    }
}
