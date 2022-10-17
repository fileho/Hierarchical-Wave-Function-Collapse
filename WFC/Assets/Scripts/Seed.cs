using UnityEngine;

[System.Serializable]
public class Seed
{
    [SerializeField]
    private int seed;
    private int currentValue;

    public void Reset()
    {
        currentValue = seed;
    }

    public int Next()
    {
        return seed == 0 ? 0 : ++currentValue;
    }
}
