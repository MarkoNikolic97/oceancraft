using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingData
{
    public GameObject item;
    public GameObject left, right, up, down;

    public bool Equals(CraftingData data)
    {
        if (item != data.item)
            return false;
        if (left != data.left)
            return false;
        if (right != data.right)
            return false;
        if (up != data.up)
            return false;
        if (down != data.down)
            return false;

        return true;
        
    }
}
