using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public List<GameObject> Biomes = new List<GameObject>();
    public int WorldSize;
    public int BiomeSize;
    

    // Start is called before the first frame update
    void Start()
    {
        int biomeCount = WorldSize / BiomeSize;
        for (int x = -biomeCount/2; x < biomeCount/2; x++)
        {
            for (int z = -biomeCount/2; z < biomeCount/2; z++)
            {
                Vector3 biomePosition = new Vector3(0f, 0f, 0f);
                biomePosition.x = x * BiomeSize;
                biomePosition.z = z * BiomeSize;

                int random = Random.Range(0, Biomes.Count);
                
                GameObject spawnBiom = Biomes[random];
                Instantiate(spawnBiom, biomePosition, Quaternion.identity, gameObject.transform);
                
            }
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
