using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel
{
    public int x, y, z;
    public Vector3 position;

    public Block block;

    public float currentHP;
    public float HP;

    public Segment segment;

    public Voxel(int x, int y, int z,Vector3 position, Segment biome, Block defaultBlock)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        //this.position = biome.transform.positionposition;
        this.position = new Vector3(x, y, z) + biome.transform.position;
      //block = defaultBlock.getCopy();
        block = defaultBlock;
        HP = block.HP;
        currentHP = block.HP;
        segment = biome;
    }

    public GameObject InstantiateVoxel()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
        segment.VoxelData[x, y, z] = null;
        Vector3Int currentChunk = new Vector3Int(x / segment.ChunkSize, y / segment.ChunkSize, z / segment.ChunkSize);
        segment.Chunks[currentChunk.x, currentChunk.y, currentChunk.z].GenerateChunk();

        ShipVoxel shipVox = cube.AddComponent<ShipVoxel>();
        shipVox.block = block.getCopy();
        cube.GetComponent<Renderer>().material = GameObject.FindGameObjectWithTag("World").GetComponent<World>().tileAtlas;
        cube.GetComponent<MeshFilter>().mesh.uv = block.GetUV();

        return cube;
        
    }
}
