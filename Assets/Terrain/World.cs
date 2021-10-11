using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Crest;

public class World : MonoBehaviour
{
    public Tree tree;

    public GameObject Player;
    public int GlobalWaterHeight;

    public Material tileAtlas;
    public GameObject Ocean;


    public int viewDistance = 4;
    public int viewHeightDistance = 2;
    public int segmentSize;
    public int chunkSize = 16;
    public int worldHeight = 128;

   

    public Segment biome;
    public int pixelSize = 16;

    public Material oceanMaskMaterial;
    public int OceanHeight;
    public Block WaterBlock;
    
    List<Texture2D> textures;

    public Block AirBlock;
    private Coroutine ConstructionSite;

    [Header("Cave Settings")]
    public float bigCaveDensityTreshold;
    public float bigCaveScale, smallCaveScale;
    /// ///////////////////////////////////////////////////

    int size;
    // Start is called before the first frame update
    void Start()
    {
        size = segmentSize * 4; 
        NoiseGenerator noiseGenerator = GetComponent<NoiseGenerator>();
        float [,] heightMap = noiseGenerator.GenerateNoiseMap2D("Perlin", size, size);
        float [,] heightMap2Pass = noiseGenerator.GenerateNoiseMap2D("Billow", size, size);
        // Tapering
        TaperHeightMap(heightMap, 32);
        TaperHeightMap(heightMap2Pass, 32);


        float [,,] bigCaveMap = Simplex.Noise.Calc3D(size, worldHeight, size, bigCaveScale);
        float [,,] smallCaveMap = Simplex.Noise.Calc3D(size, worldHeight, size, smallCaveScale);

        List<float[,]> maps = DivideHeightMap(heightMap, 4);
        List<float[,]> maps2 = DivideHeightMap(heightMap2Pass, 4);

        List<float[,,]> bigCaveMaps = DivideCaveMap(bigCaveMap, 4);
        List<float[,,]> smallCaveMaps = DivideCaveMap(smallCaveMap, 4);

        

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Segment>().GenerateSegment(maps[i], maps2[i], bigCaveMaps[i], smallCaveMaps[i]);
            Debug.Log("Generated Segment");
        }

       StartCoroutine(GenerateChunks());
    }


    public Text text;

    // Update is called once per frame

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            // transform.GetChild(0).GetComponent<Segment>().VoxelData[voxelToInst.x, voxelToInst.y, voxelToInst.z].InstantiateVoxel();
            if (ConstructionSite == null)
            {
                ConstructionSite = StartCoroutine(ConstructionSiteRoutine());

            }
            else
            {

                Debug.Log("Build Ship");
            }
        }


        if (Input.GetKeyDown(KeyCode.E))
        {
           // tree.GenerateTree(new Vector3Int(16, 16, 16), transform.GetChild(0).GetComponent<Segment>());

            StartCoroutine(GenerateChunks());


        }
    }
    public IEnumerator GenerateChunks()
    {

        Vector3Int playerPos = new Vector3Int((int)Player.transform.position.x, (int)Player.transform.position.y, (int)Player.transform.position.z);

        Vector2Int CurrentSegment = new Vector2Int(playerPos.x / segmentSize, playerPos.z / segmentSize);
        Vector3Int CurrentChunk = new Vector3Int((playerPos.x % segmentSize) / chunkSize, (playerPos.y % worldHeight) / chunkSize, (playerPos.z % segmentSize) / chunkSize);


        // int index = CurrentSegment.x * 3 + CurrentSegment.y;
        // transform.GetChild(index).GetComponent<Segment>().GenerateChunk(CurrentChunk.x, CurrentChunk.y, CurrentChunk.z);
        // Debug.Log(transform.GetChild(index));

        int s = segmentSize / chunkSize;
        int sHeight = worldHeight / chunkSize;

        Vector3Int WorldChunk = new Vector3Int(CurrentSegment.x * s + CurrentChunk.x, CurrentChunk.y, CurrentSegment.y * s + CurrentChunk.z);

        int index;
        // transform.GetChild(index).GetComponent<Segment>().GenerateChunk(WCchunk.x, WCchunk.y, WCchunk.z);

        for (int x = WorldChunk.x - viewDistance; x < WorldChunk.x + viewDistance; x++)
        {
            for (int z = WorldChunk.z - viewDistance; z < WorldChunk.z + viewDistance; z++)
            {
                for (int y = WorldChunk.y - viewHeightDistance; y < WorldChunk.y + viewHeightDistance; y++)
                {
                    if (x >= 0 && z >= 0 && y >= 0 && x < s * 4 && z < s * 4 && y < sHeight)
                    {
                        Vector2Int WCseg = new Vector2Int(x / s, z / s);
                        Vector3Int WCchunk = new Vector3Int(x % s, y, z % s);
                        index = WCseg.x * 4 + WCseg.y;


                        if (transform.GetChild(index).GetComponent<Segment>().GenerateChunk(WCchunk.x, WCchunk.y, WCchunk.z) != null)
                        {
                            yield return null;
                        }

                    }
                }
            }
        }


        //    Debug.Log(CurrentSegment);
        //    Debug.Log(WorldChunk);


    }


    public Vector3Int ConSiteSize;
    public GameObject ConSiteOutlineBox;



    //

    public IEnumerator ConstructionSiteRoutine()
    {
        //
        bool isPlacement = true;

        Vector2Int CurrentSegment = new Vector2Int();
        Vector3Int center = new Vector3Int();

        while (isPlacement)
        {




            Ray ray = Player.GetComponentInChildren<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit = new RaycastHit();
            Physics.Raycast(ray, out hit, 100f);
            while (true)
            {
                //ConstructionSite = null;
                //ConSiteOutlineBox.SetActive(false);

                ray = Player.GetComponentInChildren<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                hit = new RaycastHit();
                Physics.Raycast(ray, out hit, 100f);

                if (hit.collider == null)
                {
                    break;
                }

                Vector3Int centerBox = new Vector3Int(Mathf.FloorToInt(hit.point.x), Mathf.FloorToInt(hit.point.y) + ConSiteSize.y / 2, Mathf.FloorToInt(hit.point.z));
                Vector3 siteScale = new Vector3(ConSiteSize.x, ConSiteSize.y, ConSiteSize.z);



                Vector3Int hitPos = new Vector3Int((int)hit.point.x, (int)hit.point.y, (int)hit.point.z);

                CurrentSegment = new Vector2Int(hitPos.x / segmentSize, hitPos.z / segmentSize);
                center = new Vector3Int(hitPos.x % segmentSize, hitPos.y + ConSiteSize.y / 2, hitPos.z % segmentSize);


                ConSiteOutlineBox.SetActive(true);
                ConSiteOutlineBox.transform.position = new Vector3(centerBox.x - 0.5f, centerBox.y + 0.5f, centerBox.z - 0.5f);
                ConSiteOutlineBox.transform.localScale = siteScale;



                if (Input.GetKeyDown(KeyCode.E))
                {
                    bool isPlacementValid = true;
                    int index = CurrentSegment.x * 3 + CurrentSegment.y;
                    Segment segment = transform.GetChild(index).GetComponent<Segment>();

                    for (int x = center.x - ConSiteSize.x / 2; x < center.x + ConSiteSize.x / 2; x++)
                    {
                        for (int z = center.z - ConSiteSize.z / 2; z < center.z + ConSiteSize.z / 2; z++)
                        {



                            for (int y = center.y - ConSiteSize.y / 2 + 1; y < center.y + ConSiteSize.y / 2 + 1; y++)
                            {
                                Vector3Int CurrentChunk = new Vector3Int((x % segmentSize) / chunkSize, y / chunkSize, (z % segmentSize) / chunkSize);
                                if (segment.VoxelData[x, y, z] != null)
                                    isPlacementValid = false;
                            }
                        }

                    }
                    if (isPlacementValid == false)
                    {
                        Debug.Log("Invalid Placement");
                    }
                    else
                    {
                        isPlacement = false;
                        break;
                    }
                    /////////////////////////

                }
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    StopCoroutine(ConstructionSite);
                }







                yield return null;


            }
            yield return null;
        }

        while (true)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                BuildShip(center, CurrentSegment);
                Debug.Log("Ship Built");
            }
            yield return null;
        }

    }


    private void BuildShip(Vector3Int center, Vector2Int CurrentSegment)
    {
        GameObject ship = new GameObject();
        ship.transform.position = center;
        ship.name = "SHIP";
        ship.AddComponent<Ship>();
        int index = CurrentSegment.x * 3 + CurrentSegment.y;
        Segment segment = transform.GetChild(index).GetComponent<Segment>();
        //--
        bool[,,] VoxelBools = new bool[segment.size, segment.worldHeight, segment.size];

        int x1 = 0, y1 = 0, z1 = 0;

        //// FIRST PASS
        for (int z = center.z - ConSiteSize.z / 2; z < center.z + ConSiteSize.z / 2; z++)
        {
            for (int y = center.y - ConSiteSize.y / 2 + 1; y < center.y + ConSiteSize.y / 2 + 1; y++)
            {

                int i = center.x - ConSiteSize.x / 2; int j = center.x + ConSiteSize.x / 2;
                int minDis = i; int maxDis = j;
                //
                while (segment.VoxelData[i, y, z] == null && i < maxDis) i++;
                while (segment.VoxelData[i, y, z] == null && j > minDis) j--;
                while (i <= j)
                {
                    VoxelBools[i, y, z] = true;
                    i++;
                } 
            }
        }




        ///////

        for (int x = center.x - ConSiteSize.x / 2; x < center.x + ConSiteSize.x / 2; x++)
        {
            for (int y = center.y - ConSiteSize.y / 2 + 1; y < center.y + ConSiteSize.y / 2 + 1; y++)
            {

                int i = center.z - ConSiteSize.z / 2; int j = center.z + ConSiteSize.z / 2;
                int minDis = i; int maxDis = j;
                //
                while (segment.VoxelData[x, y, i] == null && i < maxDis) { i++; z1++; }
                while (segment.VoxelData[x, y, j] == null && j > minDis) j--;
                while (i <= j)
                {
                    GameObject shipVoxel;
                    if (VoxelBools[x, y, i] == true)
                    {
                        if (segment.VoxelData[x, y, i] != null)
                        {

                            shipVoxel = segment.VoxelData[x, y, i].InstantiateVoxel();
                            //ShipVoxelData[x1, y1, z1] = shipVoxel;
                        }
                        else // IS NULL
                        {
                            Vector3 pos = new Vector3(x, y, i) + segment.transform.position;
                            shipVoxel = InstantiateAirBlock(pos, segment, x, y, i);
                          
                            Destroy(shipVoxel.GetComponent<BoxCollider>());
                            Destroy(shipVoxel.GetComponent<MeshFilter>());
                            Destroy(shipVoxel.GetComponent<Renderer>());
                            
                        }

                        shipVoxel.AddComponent<BoatAlignNormal>();
                        

                        shipVoxel.transform.SetParent(ship.transform);//-
                        ship.GetComponent<Ship>().ShipVoxels.Add(shipVoxel);
                    }
                        i++; z1++;
                    
                }

                y1++;
            }

            x1++;
        }
       
    }

    public GameObject InstantiateAirBlock(Vector3 position, Segment segment, int x, int y, int z)
    {
         GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
       // GameObject cube = new GameObject();
        cube.transform.position = position;
        segment.VoxelData[x, y, z] = null;
        Vector3Int currentChunk = new Vector3Int(x / segment.ChunkSize, y / segment.ChunkSize, z / segment.ChunkSize);
        segment.Chunks[currentChunk.x, currentChunk.y, currentChunk.z].GenerateChunk();

        ShipVoxel shipVox = cube.AddComponent<ShipVoxel>();
        shipVox.block = AirBlock;
       

        return cube;

    }

    List<float[,]> DivideHeightMap(float[,] heightMap, int SMsize) // SMsize = 4 for 16 segments in matrix
    {
        List<float[,]> heightMapList = new List<float[,]>();
        for (int i = 0; i < SMsize * SMsize; i++)
        {
            //Vector2Int segmentIndex = new Vector2Int(i % SMsize, i / SMsize);
            Vector2Int segmentIndex = new Vector2Int(i / SMsize, i % SMsize);
            float[,] map = new float[segmentSize, segmentSize];

            int mapX = 0, mapY = 0;

            for (int x = segmentIndex.x * segmentSize; x < (segmentIndex.x + 1) * segmentSize; x++)
            {
                for (int y = segmentIndex.y * segmentSize; y < (segmentIndex.y + 1) * segmentSize; y++)
                {
                    map[mapX, mapY] = heightMap[x, y];
                    mapY++;
                }
                mapY = 0;
                mapX++;
            }

            heightMapList.Add(map);

        }
        return heightMapList;
    }

    List<float[,,]> DivideCaveMap(float[,,] caveMap, int SMsize)
    {

        List<float[,,]> caveMapList = new List<float[,,]>();
        for (int i = 0; i < SMsize * SMsize; i++)
        {
            //Vector2Int segmentIndex = new Vector2Int(i % SMsize, i / SMsize);
            Vector2Int segmentIndex = new Vector2Int(i / SMsize, i % SMsize);
            float[,,] map = new float[segmentSize,worldHeight,segmentSize];

            int mapX = 0, mapZ = 0, mapY = 0;

            for (int x = segmentIndex.x * segmentSize; x < (segmentIndex.x + 1) * segmentSize; x++)
            {
                for (int z = segmentIndex.y * segmentSize; z < (segmentIndex.y + 1) * segmentSize; z++)
                {
                    for (int y = 0; y < worldHeight; y++)
                    {
                        
                        map[mapX, mapY , mapZ] = caveMap[x, y, z];
                        mapY++;
                    }
                    mapY = 0;
                    mapZ++;
                }
                mapZ = 0;
                mapX++;
            }

            caveMapList.Add(map);

        }
        return caveMapList;


    }


    void TaperHeightMap(float[,] heightMap, int taperTreshold)
    {

        /*
                float taperScale;
                int r = Mathf.Min(x, z, size - x, size - z);
                if (r <= taperThreshold)
                {
                    taperScale = Mathf.InverseLerp(0f, taperThreshold, r);
                    currentScale2Pass = (int)(surfaceScale2Pass * taperScale);
                    currentScale1Pass = (int)(surfaceScale * taperScale);-
                }
                */
        
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                float taperScale;
                int r = Mathf.Min(x, z, size - x, size - z);

                if (r <= taperTreshold)
                {
                    taperScale = Mathf.InverseLerp(0f, taperTreshold, r);
                    
                    heightMap[x,z] *= taperScale;

                }
            }
        }

       
    }

}

