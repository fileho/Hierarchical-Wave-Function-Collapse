using UnityEngine;

namespace hwfc
{
// Custom seeding of the HWFC
// Crucial for making results reproducible
// At each call it increments a seed by a specified value
// It is important since we need to deterministically seed each wfc with different value
// Otherwise same wfc with a same size would generate same outputs
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
}
