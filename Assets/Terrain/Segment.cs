using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Segment : MonoBehaviour
{

    public List<GameObject> Players = new List<GameObject>();
    public int ViewDistance;
    public int SpawnSize;
    public int UpdateChunkDelta = 4;
    public int taperThreshold = 16;
    // Biomes are always size*size*worldHeight   ------> All Biomes have the same Size
    [Header("General")]
    public int size;
    public int worldHeight;
    public int ChunkSize;  // Must ALWAYS be : x * ChunkSize = size-

    [Header("Surface")]
    public int surfaceScale;
    public float surfaceScale2Pass;
    public int surfaceHeight;
    public string noiseType = "Perlin";
    public string noiseType2Pass = "Billow";
    public int dirtHeight;
    public float surfaceDirtChance, surfaceStoneChance;

    [Header("Cave - Big")]
    public float bigCaveDensityTreshold;
    public float bigCaveScale = 0.05f;

    [Header("Cave - Small")]
    public float smallCaveDensityTreshold;
    public float smallCaveScale;

    public Material tileAtlas;

    public Block DirtBlock;
    public Block DefaultBlock;

    float[,,] densityData;

    [HideInInspector]
    public Voxel[,,] VoxelData;
    [HideInInspector]
    public Chunk[,,] Chunks;

    float[,] heightMap, heightMap2Pass;
    float[,,] bigCaveMap, smallCaveMap;

    private bool isUpdatingChunks = false;
    private World world;

    public void GenerateChunkColumn(Vector3Int MatrixIndex)
    {
        Vector3 currentPosition = new Vector3(MatrixIndex.x * ChunkSize, 0, MatrixIndex.z * ChunkSize);
        ColumnMatrix[MatrixIndex.x, MatrixIndex.z] = true;

        for (int x = MatrixIndex.x * ChunkSize; x < MatrixIndex.x * ChunkSize + ChunkSize; x++)
        {
            for (int z = MatrixIndex.z * ChunkSize; z < MatrixIndex.z * ChunkSize + ChunkSize; z++)
            {
                // fSize is now in range 0 to 1--
                  float fsize = (heightMap[x, z] + 1) / 2;
                  float fsize2Pass = (heightMap2Pass[x, z] + 1) / 2;

               // float fsize = heightMap[x, z];
               // float fsize2Pass = heightMap2Pass[x, z];

                // Tapering Settings-----

                float currentScale2Pass = surfaceScale2Pass;
                float currentScale1Pass = surfaceScale;
                

                int height = (int)(fsize * surfaceScale);
                int height2Pass = (int)(fsize2Pass * currentScale2Pass);

                
            
                for (int hi = 0; hi < surfaceHeight + height + height2Pass; hi++)
                {
                    if (hi < surfaceHeight && hi > 0)
                    {

                       // Debug.Log(currentPosition);
                        VoxelData[x, hi, z] = new Voxel(x, hi, z,currentPosition, this, DefaultBlock);


                    }
                    else if (hi >= surfaceHeight && hi < surfaceHeight + height)
                    {

                        VoxelData[x, hi, z] = new Voxel(x, hi, z,currentPosition, this, DefaultBlock);
                        if (Random.Range(0, 100) < surfaceDirtChance)
                        {
                            VoxelData[x, hi, z].block = DirtBlock;
                        }

                    }
                    else if (hi >= surfaceHeight + height)
                    {
                        if (hi > (surfaceHeight + height + height2Pass) - dirtHeight)
                        {
                            VoxelData[x, hi, z] = new Voxel(x, hi, z,currentPosition, this, DirtBlock);
                        }
                        else
                        {
                            VoxelData[x, hi, z] = new Voxel(x, hi, z,currentPosition, this, DefaultBlock);
                        }

                        if (Random.Range(0, 100) < surfaceStoneChance)
                        {
                            VoxelData[x, hi, z].block = DefaultBlock;
                        }


                    }

                    // Cave Generation

                    float densityBig = bigCaveMap[x, hi, z];
                    float densitySmall = smallCaveMap[x, hi, z];

                  
                    if (densityBig > bigCaveDensityTreshold)
                    {

                        VoxelData[x, hi, z] = null;

                    }

                    currentPosition.y++;
                }
                currentPosition.y = 0;
                currentPosition.z++;
            }
            currentPosition.z = MatrixIndex.z * ChunkSize;
            currentPosition.x = 0;
        }


    }

    public void GenerateSegment(float[,] heightMap, float[,] heightMap2Pass, float[,,] bigCaveMap, float[,,] smallCaveMap)
    {
        //NoiseGenerator noiseGenerator = GetComponent<NoiseGenerator>();
        // heightMap = noiseGenerator.GenerateNoiseMap2D(noiseType, size, size);
        //  heightMap2Pass = noiseGenerator.GenerateNoiseMap2D(noiseType2Pass, size, size);

        VoxelData = new Voxel[size, worldHeight, size];
        // Voxel[,,] VoxelData1 = new Voxel[size, worldHeight, size];
        Chunks = new Chunk[size / ChunkSize, worldHeight / ChunkSize, size / ChunkSize];

        this.heightMap = heightMap; this.heightMap2Pass = heightMap2Pass;
        this.bigCaveMap = bigCaveMap; this.smallCaveMap = smallCaveMap;

       // this.bigCaveDensityTreshold = world.bigCaveDensityTreshold;

        ColumnMatrix = new bool[size / ChunkSize, size / ChunkSize];



        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                if (x >= 0 && z >= 0 && x < size / ChunkSize && z < size / ChunkSize)
                {
                    //
                    Vector3Int columnIndex = new Vector3Int(x, 0, z);

                    GenerateChunkColumn(columnIndex);


                }
            }
        }

        // world.tree.GenerateTree(new Vector3Int(16, 16, 16), this);

        //  UpdateChunks();


        // StartCoroutine(GenerationCoroutine());
    }

    void Start()
    {
       // WaterData = new WaterVoxel[size, size];

        world = GameObject.FindGameObjectWithTag("World").GetComponent<World>();

        

        


        
        lastKnownPlayerPosition = Players[0].transform.position;

    }

    Vector3 lastKnownPlayerPosition;

    private float playersChunkCheckFrequency = 3f;
    private float checkElapsedTime = 0;


    public Text text;
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
            //  StartCoroutine(UpdateChunksCoroutine());
          //  UpdateChunks();
            //  Debug.Log("Number of Chunks = " + transform.childCount);
           /// text.text = transform.childCount.ToString();
        }


        



    }

    bool[,] ColumnMatrix; // true if Data is generated, false if not

    public void UpdateChunks()
    {
        foreach (GameObject player in Players)
        {

            Vector3Int c = new Vector3Int((int)player.transform.position.x / ChunkSize, (int)player.transform.position.y / ChunkSize, (int)player.transform.position.z / ChunkSize);
            for (int x = c.x - ViewDistance; x < c.x + ViewDistance; x++)
            {
                for (int z = c.z - ViewDistance; z < c.z + ViewDistance; z++)
                {
                    if (x >= 0 && z >= 0 && x < size / ChunkSize && z < size / ChunkSize)
                    {
                        if (ColumnMatrix[x, z] == false)
                        {
                            GenerateChunkColumn(new Vector3Int(x, 0, z));
                        }

                        for (int y = c.y - ViewDistance; y < c.y + ViewDistance; y++)
                        {

                            if (y >= 0 && y < worldHeight / ChunkSize)
                            {

                                if (Chunks[x, y, z] == null)
                                {

                                    GameObject chunkObject = new GameObject();
                                    Chunk chunk = chunkObject.AddComponent<Chunk>();
                                    chunk.Constructor(x, y, z, ChunkSize, this, tileAtlas);

                                    chunk.GenerateChunk();

                                    Chunks[x, y, z] = chunk;
                                }
                                else
                                {
                                    Chunks[x, y, z].ShowChunk();


                                }
                            }
                        }

                    }
                }
            }
        }


    }

    public IEnumerator UpdateChunksCoroutine()
    {
        isUpdatingChunks = true;

        foreach (GameObject player in Players)
        {

            Vector3Int c = new Vector3Int((int)player.transform.position.x / ChunkSize, (int)player.transform.position.y / ChunkSize, (int)player.transform.position.z / ChunkSize);
            for (int x = c.x - ViewDistance; x < c.x + ViewDistance; x++)
            {
                for (int z = c.z - ViewDistance; z < c.z + ViewDistance; z++)
                {
                    if (x >= 0 && z >= 0 && x < size / ChunkSize && z < size / ChunkSize)
                    {
                        if (ColumnMatrix[x, z] == false)
                        {
                            GenerateChunkColumn(new Vector3Int(x, 0, z));
                        }
                        //- --
                        for (int y = c.y - ViewDistance/2; y < c.y + ViewDistance/2; y++)
                        {

                            if (y >= 0 && y < worldHeight / ChunkSize)
                            {

                                if (Chunks[x, y, z] == null)
                                {

                                    GameObject chunkObject = new GameObject();
                                    Chunk chunk = chunkObject.AddComponent<Chunk>();
                                    chunk.Constructor(x, y, z, ChunkSize, this, tileAtlas);

                                  //
                                    Chunks[x, y, z] = chunk;
                                    yield return null;  


                                }
                                
                            }
                        }
                       



                    }
                }
            }
        }
        isUpdatingChunks = false;
    }
    //
    private Vector3Int centerChunk;

    public Chunk GenerateChunk(int x, int y, int z)
    {
        if (Chunks[x, y, z] != null)
            return null;

        GameObject chunkObject = new GameObject();
        Chunk chunk = chunkObject.AddComponent<Chunk>();
        chunk.Constructor(x, y, z, ChunkSize, this, tileAtlas);

        chunk.GenerateChunk();

        Chunks[x, y, z] = chunk;

        return chunk;
    }
    
   
}
