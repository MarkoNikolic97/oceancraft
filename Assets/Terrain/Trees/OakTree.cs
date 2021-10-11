using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OakTree : Tree
{
    [Header("Left, Right, Up, Down, Forward, Back  in %")]
    public int trunkLenght;
    public float[] trunkDirection; // LEFT, RIGHT, UP, DOWN, FORWARD, BACK

    public override void GenerateTree(Vector3Int start, Segment segment)
    {
        List<Vector3Int> leaves = new List<Vector3Int>();

        // Generate Trunk
        Vector3Int trunkEnd = Branch(start, trunkDirection, segment, trunkLenght);

        // Generate Branches
        Vector3Int LF = trunkEnd + new Vector3Int(-1, 0, 1); float[] LFdir = { 100f, 10f, 60f, 0f, 100f, 10f};
        Vector3Int RF = trunkEnd + new Vector3Int(1, 0, 1); float[] RFdir = { 10f, 100f, 60f, 0f, 100f, 10f };
        Vector3Int LB = trunkEnd + new Vector3Int(-1, 0, -1); float[] LBdir = { 100f, 10f, 60f, 0f, 10f, 100f };
        Vector3Int RB = trunkEnd + new Vector3Int(1, 0, -1); float[] RBdir = { 10f, 100f, 60f, 0f, 10f, 100f };
        Vector3Int UP = trunkEnd + new Vector3Int(0, 0, 0); float[] UPdir = { 20f, 20f, 90f, 10f, 20f, 20f };

        int lenght = 3;

        leaves.Add(Branch(LF, LFdir, segment, lenght));
        leaves.Add(Branch(RF, RFdir, segment, lenght));
        leaves.Add(Branch(LB, LBdir, segment, lenght));
        leaves.Add(Branch(RB, RBdir, segment, lenght));
        leaves.Add(Branch(UP, UPdir, segment, lenght * 2));

        foreach (Vector3Int leafPos in leaves)
        {
          //  Leaves(leafPos,leafPos, segment, 4, 8);
        }
    }

    Vector3Int Branch(Vector3Int pos, float[] direction, Segment seg, int lenght)
    {
        
        // Set current Block

        seg.VoxelData[pos.x, pos.y, pos.z] = new Voxel(pos.x, pos.y, pos.z, Vector3.zero, seg, woodBlock);

        if (lenght <= 0)
            return pos;
        // Calculate next block position-
        Vector3Int directionVector = Vector3Int.zero;
       // Random r = new Random();

        if (Random.Range(0f, 100f) <= direction[0])
            directionVector += Vector3Int.left;
        else if (Random.Range(0f, 100f) <= direction[1])
            directionVector += Vector3Int.right;

        if (Random.Range(0f, 100f) <= direction[2])
            directionVector += Vector3Int.up;
        else if (Random.Range(0f, 100f) <= direction[3])
            directionVector += Vector3Int.down;

        if (Random.Range(0f, 100f) <= direction[4])
            directionVector += new Vector3Int(0, 0, 1);
        else if (Random.Range(0f, 100f) <= direction[5])
            directionVector += new Vector3Int(0, 0, -1);



        return Branch(pos + directionVector, direction, seg, lenght - 1);

    }

    void Leaves(Vector3Int pos,Vector3Int start, Segment seg, int radius, int k)
    {
        if (k <= 0 || seg.VoxelData[pos.x, pos.y, pos.z] != null)
            return;

        int x = pos.x - start.x; int y = pos.y - start.y; int z = pos.z - start.z;
        int r = x * x + y * y + z * z;
        if (r <= radius * radius)
            seg.VoxelData[pos.x, pos.y, pos.z] = new Voxel(pos.x, pos.y, pos.z, Vector3.zero, seg, leafBlock);

        
        Leaves(pos + Vector3Int.left,start, seg, radius,k-1);
        Leaves(pos + Vector3Int.right,start, seg, radius, k-1);
        Leaves(pos + Vector3Int.up,start, seg, radius, k-1);
        Leaves(pos + Vector3Int.down,start, seg, radius, k-1);
        Leaves(pos + new Vector3Int(0, 0, 1),start, seg, radius, k-1);
        Leaves(pos + new Vector3Int(0, 0, -1),start, seg, radius, k-1);

    }
}
