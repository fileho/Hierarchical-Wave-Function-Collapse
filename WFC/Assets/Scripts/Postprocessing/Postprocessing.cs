using UnityEngine;

namespace hwfc
{
/// <summary>
/// General purpose postprocessing after the final hwfc generation
/// Derive from this class the override Run() for custom postprocessing
/// </summary>
public class Postprocessing : MonoBehaviour
{
    public GameObject[,] tiles;

    public virtual void Run()
    {
    }
}
}
