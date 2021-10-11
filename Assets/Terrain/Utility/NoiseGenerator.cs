using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibNoise;

public class NoiseGenerator : MonoBehaviour
{
    // TO IMPLEMENT : SIMPLEX NOISE

    [Header("Perlin Settings")]
    public double perlinFrequency;
    public double perlinLacunarity;
    public double perlinPersistance;
    public int perlinOctaves;
    public int perlinSeed;
    public double perlinScale;
    [Header("Voronoi Settings")]
    public double voronoiFrequency;
    public double voronoiDisplacement;
    public int voronoiSeed;
    public bool voronoiDistance;
    public double voronoiScale;
    [Header("RidgedMultifractal Settings")]
    public double rmFrequency;
    public double rmLacunarity;
    public int rmOctaves;
    public int rmSeed;
    public double rmScale;
    [Header("Billow Settings")]
    public double billowFrequency;
    public double billowLacunarity;
    public double billowPersistance;
    public int billowOctaves;
    public int billowSeed;
    public double billowScale;
    [Header("Simplex Settings")]
    public float simplexScale;
    public int simplexSeed;


    float DropOffFunction(int x, int y, int distance, int size)
    {
        int falloffDistance = 16;
        int radius = 32;
        int centerX = size/2;
        int centerY = size/2;

        int fadeX =centerX +  (2*x - size) + radius;
        int fadeY =centerY +  (2*y - size) + radius;

        
        int r = (int)Mathf.Sqrt((x * x) + (y * y));
        int dist = (int)Vector2Int.Distance(new Vector2Int(fadeX, fadeY), new Vector2Int(centerX, centerY));
        if (dist > r)
        {
            if (dist < r + falloffDistance)
            {
                 return Mathf.SmoothStep(0f, 1f, (Mathf.Clamp(size/dist, 0f, 1f) + 1)/2f);
              //  return Mathf.SmoothStep(0f, 1f, ((radius+falloffDistance) + 1) / 2f);
            }
            else
            {
                return 0f;
            }

        }
        else
        {
            return 1f;
        }
        

        
    }

    public float[,] GenerateNoiseMap2D(string type, int Width, int Height)
    {
        float[,] noiseMap = new float[Width, Height];

        if (type.Equals("Perlin"))
        {
          
            LibNoise.Generator.Perlin gen = new LibNoise.Generator.Perlin(perlinFrequency, perlinLacunarity, perlinPersistance, perlinOctaves, perlinSeed, QualityMode.High);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    double sampleX = x / perlinScale;
                    double sampleY = y / perlinScale;

                    noiseMap[x, y] = (float)gen.GetValue(sampleX, sampleY, 0);
                }
            }
        }
        else if (type.Equals("Voronoi"))
        {
            LibNoise.Generator.Voronoi gen = new LibNoise.Generator.Voronoi(voronoiFrequency, voronoiDisplacement, voronoiSeed, voronoiDistance);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    double sampleX = x / voronoiScale;
                    double sampleY = y / voronoiScale;

                    noiseMap[x, y] = (float)gen.GetValue(sampleX, sampleY, 0);
                }
            }
        }
        else if (type.Equals("RidgedMultifractal"))
        {
            LibNoise.Generator.RidgedMultifractal gen = new LibNoise.Generator.RidgedMultifractal(rmFrequency, rmLacunarity, rmOctaves, rmSeed, QualityMode.High);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    double sampleX = x / rmScale;
                    double sampleY = y / rmScale;

                    noiseMap[x, y] = (float)gen.GetValue(sampleX, sampleY, 0);
                }
            }
        }
        else if (type.Equals("Billow"))
        {
            LibNoise.Generator.Billow gen = new LibNoise.Generator.Billow(billowFrequency, billowLacunarity, billowPersistance, billowOctaves, billowSeed, QualityMode.High);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    double sampleX = x / billowScale;
                    double sampleY = y / billowScale;

                    noiseMap[x, y] = (float)gen.GetValue(sampleX, sampleY, 0);
                }
            }
        }
        else if (type.Equals("Simplex"))
        {
            NoiseTest.OpenSimplexNoise gen = new NoiseTest.OpenSimplexNoise(simplexSeed);


            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                      double sampleX = x / simplexScale;
                      double sampleY = y / simplexScale;
                    Simplex.Noise.Seed = simplexSeed;
                    // float val = Simplex.Noise.CalcPixel2D(x, y, simplexScale);
                    double val = gen.Evaluate(sampleX, sampleY);
                    
                    //  noiseMap[x, y] = Mathf.InverseLerp(0f, 255f, val);
                    noiseMap[x, y] = (float)val;
                }
            }
        }
        else if (type.Equals("Fade"))
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {

                    float val = DropOffFunction(x, y, 32, Width);

                    //  noiseMap[x, y] = Mathf.InverseLerp(0f, 255f, val);
                    noiseMap[x, y] = val;
                }
            }
        }


        return noiseMap;

    }
    // Values are normalised between 0 and 1
    public float[,,] GenerateNoiseMap3D(string type, int Width, int Height, int Depth)
    {
        float[,,] noiseMap = new float[Width, Height, Depth];

        if (type.Equals("Perlin"))
        {
            Debug.Log("Perlin");
            LibNoise.Generator.Perlin gen = new LibNoise.Generator.Perlin(perlinFrequency, perlinLacunarity, perlinPersistance, perlinOctaves, perlinSeed, QualityMode.High);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int z = 0; z < Depth; z++)
                    {


                        double sampleX = x / perlinScale;
                        double sampleY = y / perlinScale;
                        double sampleZ = z / perlinScale;

                        float density = (float)gen.GetValue(sampleX, sampleY, sampleZ);
                        density = Mathf.Clamp(density, 0f, 1f);
                        noiseMap[x, y, z] = density;
                    }
                }
            }
        }
        else if (type.Equals("Voronoi"))
        {
            LibNoise.Generator.Voronoi gen = new LibNoise.Generator.Voronoi(voronoiFrequency, voronoiDisplacement, voronoiSeed, voronoiDistance);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int z = 0; z < Depth; z++)
                    {


                        double sampleX = x / voronoiScale;
                        double sampleY = y / voronoiScale;
                        double sampleZ = z / voronoiScale;

                        float density = (float)gen.GetValue(sampleX, sampleY, sampleZ);
                        //density = Mathf.Clamp(density, 0f, 1f);
                        noiseMap[x, y, z] = density;
                    }
                }
            }
        }
        else if (type.Equals("RidgedMultifractal"))
        {
            LibNoise.Generator.RidgedMultifractal gen = new LibNoise.Generator.RidgedMultifractal(rmFrequency, rmLacunarity, rmOctaves, rmSeed, QualityMode.High);
            
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int z = 0; z < Depth; z++)
                    {


                        double sampleX = x / rmScale;
                        double sampleY = y / rmScale;
                        double sampleZ = z / rmScale;

                        float density = (float)gen.GetValue(sampleX, sampleY, sampleZ);
                       // density = Mathf.Clamp(density, 0f, 1f);
                        noiseMap[x, y, z] = density;
                    }
                }
            }
        }
        else if (type.Equals("Billow"))
        {
            LibNoise.Generator.Billow gen = new LibNoise.Generator.Billow(billowFrequency, billowLacunarity, billowPersistance, billowOctaves, billowSeed, QualityMode.High);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int z = 0; z < Depth; z++)
                    {


                        double sampleX = x / billowScale;
                        double sampleY = y / billowScale;
                        double sampleZ = z / billowScale;

                        float density = (float)gen.GetValue(sampleX, sampleY, sampleZ);
                      //  density = Mathf.Clamp(density, 0f, 1f);
                        noiseMap[x, y, z] = density;
                    }
                }
            }
        }
        else if (type.Equals("Simplex"))
        {
            NoiseTest.OpenSimplexNoise gen = new NoiseTest.OpenSimplexNoise(simplexSeed);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int z = 0; z < Depth; z++)
                    {


                        double sampleX = x / simplexScale;
                        double sampleY = y / simplexScale;
                        double sampleZ = z / simplexScale;

                        float density = (float)gen.Evaluate(sampleX, sampleY, sampleZ);
                       
                        noiseMap[x, y, z] = density;
                    }
                }
            }
        }

        return noiseMap;
    }
}
