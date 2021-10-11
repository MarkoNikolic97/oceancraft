using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waves : MonoBehaviour
{
    public int Dimension = 10;

    public List<WaveData> waves;

    public int taperThreshold = 16;
    public float UVScale = 2f;
    public float WaveFPS = 30;
    public float WaveScale = 1f;
    protected float elapsedTime;


    //Mesh
    protected MeshFilter MeshFilter;
    protected Mesh Mesh;

    // Start is called before the first frame update
    void Start()
    {

        originalSharedMesh = GetComponent<MeshCollider>().sharedMesh;

        //Mesh Setup
        Mesh = new Mesh();
        Mesh.name = gameObject.name;

        Mesh.vertices = GenerateVerts();
        Mesh.triangles = GenerateTries();
        Mesh.uv = GenerateUVs();
        Mesh.RecalculateNormals();
        Mesh.RecalculateBounds();

        MeshFilter = gameObject.GetComponent<MeshFilter>();
        MeshFilter.mesh = Mesh;
    }
    private void OnDestroy()
    {
        GetComponent<MeshCollider>().sharedMesh = originalSharedMesh;
    }

    private Vector3[] GenerateVerts()
    {
        var verts = new Vector3[(Dimension + 1) * (Dimension + 1)];

        //equaly distributed verts
        for (int x = 0; x <= Dimension; x++)
            for (int z = 0; z <= Dimension; z++)
                verts[index(x, z)] = new Vector3(x, 0, z);

        return verts;
    }

    private int[] GenerateTries()
    {
        var tries = new int[Mesh.vertices.Length * 6];

        //two triangles are one tile
        for (int x = 0; x < Dimension; x++)
        {
            for (int z = 0; z < Dimension; z++)
            {
                tries[index(x, z) * 6 + 0] = index(x, z);
                tries[index(x, z) * 6 + 1] = index(x + 1, z + 1);
                tries[index(x, z) * 6 + 2] = index(x + 1, z);
                tries[index(x, z) * 6 + 3] = index(x, z);
                tries[index(x, z) * 6 + 4] = index(x, z + 1);
                tries[index(x, z) * 6 + 5] = index(x + 1, z + 1);
            }
        }

        return tries;
    }

    private Vector2[] GenerateUVs()
    {
        var uvs = new Vector2[Mesh.vertices.Length];

        //always set one uv over n tiles than flip the uv and set it again
        for (int x = 0; x <= Dimension; x++)
        {
            for (int z = 0; z <= Dimension; z++)
            {
                var vec = new Vector2((x / UVScale) % 2, (z / UVScale) % 2);
                uvs[index(x, z)] = new Vector2(vec.x <= 1 ? vec.x : 2 - vec.x, vec.y <= 1 ? vec.y : 2 - vec.y);
            }
        }

        return uvs;
    }

    private float fpsTimer;
    // Update is called once per frame
    void Update()
    {

        fpsTimer += Time.deltaTime;
        elapsedTime += Time.deltaTime;

        float FPSthreshold = 1 / WaveFPS;
        if (fpsTimer >= FPSthreshold)
        {
            Debug.Log(FPSthreshold);
            UpdateWaves();
            Debug.Log("Updated");
            fpsTimer = 0;
        }

       
        
    }

    private int index(int x, int z)
    {
        return x * (Dimension + 1) + z;
    }

    private int index(float x, float z)
    {
        return index((int)x, (int)z);
    }

    protected Mesh originalSharedMesh;

    void UpdateWaves()
    {
        var verts = Mesh.vertices;

       

        for (int x = 0; x <= Dimension; x++)
        {
           
            for (int z = 0; z <= Dimension; z++)
            {
                Vector3 offset = new Vector3(0,0,0);

                foreach (WaveData data in waves)
                {
                    offset += GerstnerWave(x, z, data);
                }


                verts[index(x, z)] = offset;
            }
        }



        Mesh.vertices = verts;
       
        Mesh.RecalculateNormals();
        Mesh.RecalculateBounds();
       // Mesh.RecalculateTangents();

        GetComponent<MeshCollider>().sharedMesh = Mesh;

       

    }
    public Segment biome;

    Vector3 GerstnerWave(int x, int z, WaveData data)
    {
        Vector3 p = new Vector3(x, 0, z);

        


        float k = 2 * Mathf.PI / data.Wavelength;

        float c = Mathf.Sqrt(9.81f / k);

        Vector3 d = data.WaveDirection.normalized;
        float f = k * (Vector3.Dot(d, p) - c * elapsedTime);
        float a = data.Steepness / k;


        p.x += d.x * (a * Mathf.Cos(f));
        p.y = a * Mathf.Sin(f);
        p.z += d.z * (a * Mathf.Cos(f));

        float taperScale;
        int r = Mathf.Min(x, z, Dimension - x, Dimension - z);
        if (r <= taperThreshold)
        {
            taperScale = Mathf.InverseLerp(0f, taperThreshold, r);
            p.y *= taperScale;
        }
        p.y *= WaveScale;

        return p;

    }
}
