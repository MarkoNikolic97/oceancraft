using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public float physicsFrequency = 0.1f;
    public float gravityForce = 9.80665f;

    public float DragTreshold = 0.2f; // Ako je visee od 50% voxela ispod vode -> WaterDrag
    public Vector2 AirDrag = new Vector2(0f, 0.05f);
    public Vector2 WaterDrag = new Vector2(0.5f, 1f);


    public List<GameObject> ShipVoxels = new List<GameObject>();
    private float Mass;
    private Vector3 CenterOfMass;

  //  private GameObject OceanMask;

    private Rigidbody rigidbody;
    private World world;
    // Start is called before the first frame update
    void Start()
    {
        world = GameObject.FindGameObjectWithTag("World").GetComponent<World>();

        GenerateOceanMask();
       
        

        rigidbody = gameObject.AddComponent<Rigidbody>();
        rigidbody.useGravity = false;

        

    }

    private void Update()
    {
       // OceanMask.transform.position = transform.position;
       // OceanMask.transform.rotation = transform.rotation;
    }
    // Update is called once per frame
    float elapsedTime = 0f;
    void FixedUpdate()
    {
       

        elapsedTime += Time.fixedDeltaTime;
        if (elapsedTime >= physicsFrequency)
        {
            // Do physics
            CalculateShipData();
            UpdateGravity();
            UpdateBuoyancy();

            //
            elapsedTime = 0f;
        }
    }

    public void CalculateShipData()
    {
        float totalMass = 0f;
        Vector3 com = Vector3.zero;
        int n = ShipVoxels.Count;

        for (int i = 0; i < n; i++)
        {
            GameObject vox = ShipVoxels[i];

            com += vox.transform.localPosition * vox.GetComponent<ShipVoxel>().block.mass;
            totalMass += vox.GetComponent<ShipVoxel>().block.mass;
        }
        com /= totalMass;
        Debug.Log(totalMass);
        // Apply Changes
        CenterOfMass = com;
        Mass = totalMass;

        rigidbody.centerOfMass = CenterOfMass;
        rigidbody.mass = Mass;

    }

    public void UpdateGravity()
    {
        rigidbody.AddForce(Vector3.down * gravityForce * rigidbody.mass);
    }

    public void UpdateBuoyancy()
    {
        Vector3 centerOfBuoyancy = Vector3.zero;

        int n = ShipVoxels.Count;
        float underwaterCount = 0;
        for (int i = 0; i < n; i++)
        {
            GameObject vox = ShipVoxels[i];
            if (vox.GetComponent<BoatAlignNormal>().InWater)
            {
                centerOfBuoyancy += vox.transform.position;
                underwaterCount++;
            }
        }

        float percUnderwater = underwaterCount / transform.childCount;
        if (percUnderwater >= DragTreshold) // >= pola je u vodi
        {
            rigidbody.drag = WaterDrag.x; rigidbody.angularDrag = WaterDrag.y;
        }
        else
        {
            rigidbody.drag = AirDrag.x; rigidbody.angularDrag = AirDrag.y;
        }
       
        if (underwaterCount == 0)
            return;


        centerOfBuoyancy /= underwaterCount;

        float buoyantForce = gravityForce * world.WaterBlock.mass * underwaterCount;

        rigidbody.AddForceAtPosition(Vector3.up * buoyantForce, centerOfBuoyancy);
    }



    public GameObject GenerateOceanMask()
    {

        // Instantiate Visible Voxels
        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Destroy(block.GetComponent<Collider>());
        
        GameObject mask = new GameObject();
        mask.AddComponent<MeshFilter>();
        mask.AddComponent<MeshRenderer>();
        mask.name = "Ship Mask";                                                                         

        MeshFilter[] meshFilters = new MeshFilter[transform.childCount];

        mask.SetActive(false);

        for (int i = 0; i < ShipVoxels.Count; i++)
        {
            GameObject o = Instantiate(block, ShipVoxels[i].transform.position, Quaternion.identity);
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
       // gameObject.SetActive(false);

        
        



        MeshFilter filter = mask.GetComponent<MeshFilter>();
        filter.mesh = new Mesh();
        filter.mesh.CombineMeshes(combine, true);

        filter.mesh.RecalculateBounds();
        filter.mesh.RecalculateNormals();

        mask.GetComponent<Renderer>().material = world.oceanMaskMaterial;

        MeshCollider collider = mask.AddComponent<MeshCollider>();
        collider.convex = true;
        collider.isTrigger = true;

        mask.SetActive(true);
        
        // Destroy all combined blocks
        foreach (MeshFilter f in meshFilters)
        {
            DestroyImmediate(f.gameObject);
        }

        ShipMask sm = mask.AddComponent<ShipMask>();
        sm.Ship = gameObject;

        mask.transform.position = transform.position;
        mask.transform.SetParent(transform);

        return mask;


    }

}
