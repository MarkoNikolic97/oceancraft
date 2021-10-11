using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibNoise;



public class NoiseMapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;
    public int Width, Height;
    public string NoiseType = "Perlin";


    public void DrawNoiseMap(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);
        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, (Mathf.Clamp(noiseMap[x, y], -1, 1) + 1) / 2); 
            }
        }
        texture.SetPixels(colorMap);
        texture.Apply();

        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(width, 1, height);
    }

    
    public void GenerateMap()
    {
     
     
        float[,] noiseMap = new float[Width, Height];

        noiseMap = GetComponent<NoiseGenerator>().GenerateNoiseMap2D(NoiseType, Width, Height);
      
        DrawNoiseMap(noiseMap);
    }
}
