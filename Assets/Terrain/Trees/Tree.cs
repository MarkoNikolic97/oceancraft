using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : ScriptableObject
{
    public Block woodBlock, leafBlock;

    public virtual void GenerateTree(Vector3Int start, Segment segment) { }
}
