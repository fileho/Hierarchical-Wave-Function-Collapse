using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hwfc
{
/// <summary>
/// Simple auto-tilling to allow easier specification of the input. Otherwise, much more variety would be required.
/// </summary>

[RequireComponent(typeof(TileType))]
[RequireComponent(typeof(SpriteRenderer))]
public class Autotilling : MonoBehaviour
{
    public enum TillingType
    {
        FourWay,
        EightWay,
        TwelveWay
    }

    [SerializeField]
    private TillingType tillingType;

    // Tiles 4-way
    [SerializeField]
    private Sprite northWest;
    [SerializeField]
    private Sprite northEast;
    [SerializeField]
    private Sprite southWest;
    [SerializeField]
    private Sprite southEast;

    // Tiles 8-way
    [SerializeField]
    private Sprite north;
    [SerializeField]
    private Sprite south;
    [SerializeField]
    private Sprite east;
    [SerializeField]
    private Sprite west;

    // Tiles 12-way
    [SerializeField]
    private Sprite northWestDiag;
    [SerializeField]
    private Sprite northEastDiag;
    [SerializeField]
    private Sprite southWestDiag;
    [SerializeField]
    private Sprite southEastDiag;

    private SpriteRenderer spriteRenderer;
    private int tileValue;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void AutoTile(GameObject[,] objects, int x, int y)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        tileValue = GetComponent<TileType>().type;

        if (NotDiagonals(objects, x, y))
            return;

        TwelveWay(objects, x, y);
    }

    private void TwelveWay(GameObject[,] objects, int x, int y)
    {
        bool ne = !IsSameCheck(objects, x + 1, y + 1);
        bool nw = !IsSameCheck(objects, x - 1, y + 1);
        bool se = !IsSameCheck(objects, x + 1, y - 1);
        bool sw = !IsSameCheck(objects, x - 1, y - 1);

        int countDiag = BtoI(ne) + BtoI(nw) + BtoI(se) + BtoI(sw);

        if (countDiag != 1)
            return;

        if (ne)
            spriteRenderer.sprite = northEastDiag;
        if (nw)
            spriteRenderer.sprite = northWestDiag;
        if (se)
            spriteRenderer.sprite = southEastDiag;
        if (sw)
            spriteRenderer.sprite = southWestDiag;
    }

    private bool NotDiagonals(GameObject[,] objects, int x, int y)
    {
        bool n = !IsSameCheck(objects, x, y + 1);
        bool s = !IsSameCheck(objects, x, y - 1);
        bool e = !IsSameCheck(objects, x + 1, y);
        bool w = !IsSameCheck(objects, x - 1, y);

        int count = BtoI(n) + BtoI(s) + BtoI(e) + BtoI(w);

        if (count == 2)
        {
            FourWay(n, s, e, w);
            return true;
        }

        if (tillingType == TillingType.FourWay)
            return true;

        if (count == 1)
        {
            EightWay(n, s, e, w);
            return true;
        }

        return tillingType == TillingType.EightWay || count != 0;
    }

    private void FourWay(bool n, bool s, bool e, bool w)
    {
        if (n && e)
            spriteRenderer.sprite = northEast;
        if (n && w)
            spriteRenderer.sprite = northWest;
        if (s && e)
            spriteRenderer.sprite = southEast;
        if (s && w)
            spriteRenderer.sprite = southWest;
    }

    private void EightWay(bool n, bool s, bool e, bool w)
    {
        if (n)
            spriteRenderer.sprite = north;
        if (s)
            spriteRenderer.sprite = south;
        if (e)
            spriteRenderer.sprite = east;
        if (w)
            spriteRenderer.sprite = west;
    }

    private bool IsSameCheck(GameObject[,] objects, int x, int y)
    {
        if (x < 0 || y < 0)
            return false;

        int sizeX = objects.GetLength(0);
        int sizeY = objects.GetLength(1);

        if (x >= sizeX || y >= sizeY)
            return false;

        return IsSame(objects[x, y]);
    }

    private bool IsSame(GameObject o)
    {
        if (o == null)
            return false;

        var t = o.GetComponent<TileType>();
        if (t == null)
            return false;

        return tileValue == t.type;
    }

    private int BtoI(bool b)
    {
        return Convert.ToInt32(b);
    }
}
}
