using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public int size;
    public Vector3Int MatrixIndex; // Index of this Chunk in Biomes Matrix of Chunks
    public Segment biome;

    public Vector3Int VoxelIndex; // Index of Voxel that this Chunk begins at
    public Vector3 WorldCenter;

    int pixelSize = 16;

    Material tileAtlas;
    
    public void Constructor(int x, int y, int z, int size, Segment biome, Material tileAtlas)
    {

        this.tileAtlas = tileAtlas;

        gameObject.name = "Chunk (" + x + "," + y + "," + z + ")";
        this.size = size;
        MatrixIndex = new Vector3Int(x, y, z);
        this.biome = biome;

        // Calculate Chunk positions
        VoxelIndex = new Vector3Int(x * size, y * size, z * size);
        WorldCenter = new Vector3(VoxelIndex.x + size / 2, VoxelIndex.y + size / 2, VoxelIndex.z + size / 2);

        // Set Parent
        transform.position = VoxelIndex;
        transform.SetParent(biome.gameObject.transform);

        // Add Mesh Scripts
        gameObject.AddComponent<MeshFilter>();
        MeshRenderer chunkRenderer = gameObject.AddComponent<MeshRenderer>();
        chunkRenderer.material = tileAtlas;
    }
    List<Voxel> visibleVoxels = new List<Voxel>();

    public Vector3Int getVoxelFromWorldPosition(Vector3 hitPositionWorld)
    {
        int x = 0, y = 0, z = 0;

        float minDistance = float.MaxValue;
        float dist;
        foreach (Voxel voxel in visibleVoxels)
        {
            dist = Vector3.Distance(hitPositionWorld, voxel.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                x = voxel.x;
                y = voxel.y;
                z = voxel.z;
            }
        }

        return new Vector3Int(x, y, z);
    }

    public void VoxelHit(Vector3 hitPositionWorld, float dmg, GameObject Player)
    {
        Vector3Int voxelIndex = getVoxelFromWorldPosition(hitPositionWorld);
        Voxel voxel = biome.VoxelData[voxelIndex.x, voxelIndex.y, voxelIndex.z];

        voxel.currentHP -= dmg;

        Player.GetComponent<PlayerController>().SetOutlineTexture(voxel);
        Player.GetComponent<PlayerController>().OutlineBox.GetComponent<MeshFilter>().mesh.uv = Player.GetComponent<PlayerController>().OutlineBlock.GetUV();
        Player.GetComponent<PlayerController>().OutlineBox.GetComponent<MeshFilter>().mesh.RecalculateBounds();
        Player.GetComponent<PlayerController>().OutlineBox.GetComponent<MeshFilter>().mesh.RecalculateNormals();

        if (voxel.currentHP <= 0)
        {
            biome.VoxelData[voxel.x, voxel.y, voxel.z] = null;

            GenerateChunk();

           // Debug.Log(MatrixIndex);

            // Generate All chunks around this Chunk

            if (MatrixIndex.x + 1 < biome.size / biome.ChunkSize)
                biome.Chunks[MatrixIndex.x + 1, MatrixIndex.y, MatrixIndex.z].GenerateChunk();

            if (MatrixIndex.x - 1 >= 0)
                biome.Chunks[MatrixIndex.x - 1, MatrixIndex.y, MatrixIndex.z].GenerateChunk();

            if (MatrixIndex.y + 1 < biome.size / biome.ChunkSize)
                biome.Chunks[MatrixIndex.x, MatrixIndex.y + 1, MatrixIndex.z].GenerateChunk();

            if (MatrixIndex.y - 1 >= 0)
                biome.Chunks[MatrixIndex.x, MatrixIndex.y - 1, MatrixIndex.z].GenerateChunk();

            if (MatrixIndex.z + 1 < biome.size / biome.ChunkSize)
                biome.Chunks[MatrixIndex.x, MatrixIndex.y, MatrixIndex.z + 1].GenerateChunk();

            if (MatrixIndex.z - 1 >= 0)
                biome.Chunks[MatrixIndex.x, MatrixIndex.y, MatrixIndex.z - 1].GenerateChunk();
        }
    }

    public void AddVoxel(Vector3 hitPositionWorld, Vector3 normal, Block block)
    {

        Vector3Int voxel = getVoxelFromWorldPosition(hitPositionWorld);
        Voxel clickedVoxel = biome.VoxelData[voxel.x, voxel.y, voxel.z];


        Vector3Int newVoxelIndex = new Vector3Int((int)(clickedVoxel.x + normal.x), (int)(clickedVoxel.y + normal.y), (int)(clickedVoxel.z + normal.z));
        Vector3 newVoxelPosition = clickedVoxel.position + normal;

        biome.VoxelData[newVoxelIndex.x, newVoxelIndex.y, newVoxelIndex.z] = new Voxel(newVoxelIndex.x, newVoxelIndex.y, newVoxelIndex.z, newVoxelPosition, biome, block);

        GenerateChunk();

        // Generate All chunks around this Chunk
        
        if (MatrixIndex.x + 1 < biome.size / biome.ChunkSize)
            biome.Chunks[MatrixIndex.x + 1, MatrixIndex.y, MatrixIndex.z].GenerateChunk();

        if (MatrixIndex.x - 1 >= 0)
            biome.Chunks[MatrixIndex.x - 1, MatrixIndex.y, MatrixIndex.z].GenerateChunk();

        if (MatrixIndex.y + 1 < biome.worldHeight / biome.ChunkSize)
            biome.Chunks[MatrixIndex.x, MatrixIndex.y + 1, MatrixIndex.z].GenerateChunk();

        if (MatrixIndex.y - 1 >= 0)
            biome.Chunks[MatrixIndex.x, MatrixIndex.y - 1, MatrixIndex.z].GenerateChunk();

        if (MatrixIndex.z + 1 < biome.size / biome.ChunkSize)
            biome.Chunks[MatrixIndex.x, MatrixIndex.y, MatrixIndex.z + 1].GenerateChunk();

        if (MatrixIndex.z - 1 >= 0)
            biome.Chunks[MatrixIndex.x, MatrixIndex.y, MatrixIndex.z - 1].GenerateChunk();

        Debug.Log("Updated all around");
    }

   

    public void GenerateChunk()
    {//
     // DestroyImmediate(GetComponent<MeshFilter>());
     // Destroy Current Chunk
        if (GetComponent<MeshCollider>())
        {
            DestroyImmediate(GetComponent<MeshCollider>());
        }



        // Determine Visible Voxels
        visibleVoxels.Clear();

        Voxel[,,] VoxelData = biome.VoxelData;
        int dataSize = biome.size;
        int dataHeight = biome.worldHeight;
        for (int x = VoxelIndex.x; x < VoxelIndex.x + size; x++)
        {
            for (int y = VoxelIndex.y; y < VoxelIndex.y + size; y++)
            {
                for (int z = VoxelIndex.z; z < VoxelIndex.z + size; z++)
                {
                    bool visible = false;

                    if (x + 1 < dataSize && VoxelData[x + 1, y, z] == null)
                        visible = true;
                    else if (x - 1 >= 0 && VoxelData[x - 1, y, z] == null)
                        visible = true;
                    else if (y + 1 < dataHeight && VoxelData[x, y + 1, z] == null)
                        visible = true;
                    else if (y - 1 >= 0 && VoxelData[x, y - 1, z] == null)
                        visible = true;
                    else if (z + 1 < dataSize && VoxelData[x, y, z + 1] == null)
                        visible = true;
                    else if (z - 1 >= 0 && VoxelData[x, y, z - 1] == null)
                        visible = true;



                    if (visible && VoxelData[x, y, z] != null)
                    {
                        visibleVoxels.Add(VoxelData[x, y, z]);
                    }
                }
            }
        }
       

       

        // Instantiate Visible Voxels
        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);


        MeshFilter[] meshFilters = new MeshFilter[visibleVoxels.Count];

        for (int i = 0; i < visibleVoxels.Count; i++)
        {
            GameObject o = Instantiate(block, visibleVoxels[i].position, Quaternion.identity, transform);
            meshFilters[i] = o.GetComponent<MeshFilter>();
        }

        DestroyImmediate(block);

        /////////////////////////////// Combine Visible Blocks ////////////////////////////////////

        


        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].mesh;
            Matrix4x4 matrix = transform.worldToLocalMatrix;
            combine[i].transform = matrix * meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }
        gameObject.SetActive(false);

        // Calculate Mesh UVs
        
        Vector2[] meshUVs = new Vector2[meshFilters.Length * 24];
        for (int i = 0; i < meshFilters.Length; i++)
        {
            Block b = visibleVoxels[i].block;
            int start = i * 24;

            float tilePerc = 1f / pixelSize;
          

            float uMin = tilePerc * b.sideTile.x;
            float uMax = tilePerc * (b.sideTile.x + 1);
            float vMin = tilePerc * b.sideTile.y;
            float vMax = tilePerc * (b.sideTile.y + 1);

            float uTopMin = tilePerc * b.topTile.x;
            float uTopMax = tilePerc * (b.topTile.x + 1);
            float vTopMin = tilePerc * b.topTile.y;
            float vTopMax = tilePerc * (b.topTile.y + 1);

            float uBotMin = tilePerc * b.bottomTile.x;
            float uBotMax = tilePerc * (b.bottomTile.x + 1);
            float vBotMin = tilePerc * b.bottomTile.y;
            float vBotMax = tilePerc * (b.bottomTile.y + 1);



            // North Vertices = 0, 1, 2, 3            1 0 3  &  3 0 2
            // South Vertices = 6, 7 , 10, 11        11 10 7  &  7 10 6
            // East Vertices = 20, 21, 22, 23        22 20 21  &  21 20 23
            // West Vertices = 16, 17, 18, 19        18 16 17  &  17 16 19
            // Up Vertices = 4, 5, 8, 9              9 8 5  &  5 8 4
            // Down Vertices = 12, 13, 14, 15        14 12 13  &  13 12 15


            meshUVs[start] = new Vector2(uMin, vMin);
            meshUVs[start + 1] = new Vector2(uMax, vMin);
            meshUVs[start + 2] = new Vector2(uMin, vMax);
            meshUVs[start + 3] = new Vector2(uMax, vMax);
            meshUVs[start + 4] = new Vector2(uTopMin, vTopMax);
            meshUVs[start + 5] = new Vector2(uTopMax, vTopMax);
            meshUVs[start + 6] = new Vector2(uMax, vMin);
            meshUVs[start + 7] = new Vector2(uMin, vMin);
            meshUVs[start + 8] = new Vector2(uTopMin, vTopMin);
            meshUVs[start + 9] = new Vector2(uTopMax, vTopMin);
            meshUVs[start + 10] = new Vector2(uMax, vMax);
            meshUVs[start + 11] = new Vector2(uMin, vMax);
            meshUVs[start + 12] = new Vector2(uBotMin, vBotMin);
            meshUVs[start + 13] = new Vector2(uBotMin, vBotMax);
            meshUVs[start + 14] = new Vector2(uBotMax, vBotMax);
            meshUVs[start + 15] = new Vector2(uBotMax, vBotMin);
            meshUVs[start + 16] = new Vector2(uMin, vMin);
            meshUVs[start + 17] = new Vector2(uMin, vMax);
            meshUVs[start + 18] = new Vector2(uMax, vMax);
            meshUVs[start + 19] = new Vector2(uMax, vMin);
            meshUVs[start + 20] = new Vector2(uMin, vMin);
            meshUVs[start + 21] = new Vector2(uMin, vMax);
            meshUVs[start + 22] = new Vector2(uMax, vMax);
            meshUVs[start + 23] = new Vector2(uMax, vMin);

        }
        


        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        filter.mesh = new Mesh();
        filter.mesh.CombineMeshes(combine, true);

        filter.mesh.uv = meshUVs;

        filter.mesh.RecalculateBounds();
        filter.mesh.RecalculateNormals();


        gameObject.AddComponent<MeshCollider>();

        // Destroy all combined blocks
        foreach (MeshFilter f in meshFilters)
        {
            DestroyImmediate(f.gameObject);
        }

        gameObject.SetActive(true);



    }
    public void ShowChunk()
    {
        gameObject.SetActive(true);
    }

    public void HideChunk()
    {
        gameObject.SetActive(false);
    }


    public void Start()
    {
        pixelSize = GameObject.FindGameObjectWithTag("World").GetComponent<World>().pixelSize;
    }
}
