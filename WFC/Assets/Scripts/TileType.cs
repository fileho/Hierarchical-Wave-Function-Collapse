using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hwfc
{
/// <summary>
/// Sets a value for a tile. Used to inform the Hierarchical Controller where we should run WFCs from the next layer. It
/// is required if we want to run any WFC over this tile. Should be added directly to each tile prefab used in the
/// generation.
/// </summary>

public class TileType : MonoBehaviour
{
    public int type;
}
}
