using UnityEngine;

namespace hwfc
{

// General purpose postprocessing after the final hwfc generation
// Derive from this class the override Run() for custom postprocessing
public class Postprocessing : MonoBehaviour
{
    public GameObject[,] tiles;

    public virtual void Run()
    {
    }
}
}
