using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Block : ScriptableObject
{
    int pixelSize = 16;

    public string Name;
    public float currentHP;
    public float HP;

    public float mass;

    public Vector2Int sideTile;
    public Vector2Int topTile;
    public Vector2Int bottomTile;
 
    

    public Block getCopy()
    {
        Block block = (Block)ScriptableObject.CreateInstance("Block");
        block.Name = Name;
        block.HP = HP;
        block.sideTile = sideTile;
        block.topTile = topTile;
        block.bottomTile = bottomTile;
        block.mass = mass;

        block.currentHP = HP;

        return block;

    }

    public Vector2[] GetUV()
    {
        float tilePerc = 1f / pixelSize;
        //Debug.Log("calculated uvs: " + tilePerc);

        float uMin = tilePerc * sideTile.x;
        float uMax = tilePerc * (sideTile.x + 1);
        float vMin = tilePerc * sideTile.y;
        float vMax = tilePerc * (sideTile.y + 1);

        float uTopMin = tilePerc * topTile.x;
        float uTopMax = tilePerc * (topTile.x + 1);
        float vTopMin = tilePerc * topTile.y;
        float vTopMax = tilePerc * (topTile.y + 1);

        float uBotMin = tilePerc * bottomTile.x;
        float uBotMax = tilePerc * (bottomTile.x + 1);
        float vBotMin = tilePerc * bottomTile.y;
        float vBotMax = tilePerc * (bottomTile.y + 1);

        Vector2[] blockUVs = new Vector2[24];

        // Nort Vertices = 0, 1, 2, 3            1 0 3  &  3 0 2
        // South Vertices = 6, 7 , 10, 11        11 10 7  &  7 10 6
        // East Vertices = 20, 21, 22, 23        22 20 21  &  21 20 23
        // West Vertices = 16, 17, 18, 19        18 16 17  &  17 16 19
        // Up Vertices = 4, 5, 8, 9              9 8 5  &  5 8 4
        // Down Vertices = 12, 13, 14, 15        14 12 13  &  13 12 15


        blockUVs[0] = new Vector2(uMin, vMin);
        blockUVs[1] = new Vector2(uMax, vMin);
        blockUVs[2] = new Vector2(uMin, vMax);
        blockUVs[3] = new Vector2(uMax, vMax);
        blockUVs[4] = new Vector2(uTopMin, vTopMax);
        blockUVs[5] = new Vector2(uTopMax, vTopMax);
        blockUVs[6] = new Vector2(uMax, vMin);
        blockUVs[7] = new Vector2(uMin, vMin);
        blockUVs[8] = new Vector2(uTopMin, vTopMin);
        blockUVs[9] = new Vector2(uTopMax, vTopMin);
        blockUVs[10] = new Vector2(uMax, vMax);
        blockUVs[11] = new Vector2(uMin, vMax);
        blockUVs[12] = new Vector2(uBotMin, vBotMin);
        blockUVs[13] = new Vector2(uBotMin, vBotMax);
        blockUVs[14] = new Vector2(uBotMax, vBotMax);
        blockUVs[15] = new Vector2(uBotMax, vBotMin);
        blockUVs[16] = new Vector2(uMin, vMin);
        blockUVs[17] = new Vector2(uMin, vMax);
        blockUVs[18] = new Vector2(uMax, vMax);
        blockUVs[19] = new Vector2(uMax, vMin);
        blockUVs[20] = new Vector2(uMin, vMin);
        blockUVs[21] = new Vector2(uMin, vMax);
        blockUVs[22] = new Vector2(uMax, vMax);
        blockUVs[23] = new Vector2(uMax, vMin);



        return blockUVs;
    }


    /*
     * blockUVs[0] = new Vector2(uMin, vMin);
        blockUVs[1] = new Vector2(uMax, vMin);
        blockUVs[2] = new Vector2(uMin, vMax);
        blockUVs[3] = new Vector2(uMax, vMax);
        blockUVs[4] = new Vector2(uTopMin, vTopMax);
        blockUVs[5] = new Vector2(uTopMax, vTopMax);
        blockUVs[6] = new Vector2(uMin, vMax);
        blockUVs[7] = new Vector2(uMax, vMax);
        blockUVs[8] = new Vector2(uTopMin, vTopMin);
        blockUVs[9] = new Vector2(uTopMax, vTopMin);
        blockUVs[10] = new Vector2(uMin, vMin);
        blockUVs[11] = new Vector2(uMax, vMin);
        blockUVs[12] = new Vector2(uMin, vMin);
        blockUVs[13] = new Vector2(uMin, vMax);
        blockUVs[14] = new Vector2(uMax, vMax);
        blockUVs[15] = new Vector2(uMax, vMin);
        blockUVs[16] = new Vector2(uMin, vMin);
        blockUVs[17] = new Vector2(uMin, vMax);
        blockUVs[18] = new Vector2(uMax, vMax);
        blockUVs[19] = new Vector2(uMax, vMin);
        blockUVs[20] = new Vector2(uMin, vMin);
        blockUVs[21] = new Vector2(uMin, vMax);
        blockUVs[22] = new Vector2(uMax, vMax);
        blockUVs[23] = new Vector2(uMax, vMin);
     * 
     */
}
