using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public float groundNoiseFactor;
    public float detailsNoiseFactor;
    public float maxGrassHeight;
    public float minGrassProbability;
    public Transform collectibleLinkPrefab;
    public int minCollectibleHeads;
    public int maxCollectibleHeads;
    public int minCollectibleLinks;
    public int maxCollectibleLinks;
    public int spawnHeight;
    public int boundsSize;

    private Terrain terrain;
    private TerrainData terrainData;
    

	void Awake ()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;

        // Generation of heads
        Transform headsFolder = GameObject.Find("Heads").transform;
        int numberOfCollectibleHeads = Random.Range(minCollectibleHeads, maxCollectibleHeads);
        for (int i = 0; i < numberOfCollectibleHeads; i++)
        {
            //Transform head = Instantiate(headPrefab, new Vector3(Random.Range(boundsSize, terrainData.size.x - boundsSize), spawnHeight, Random.Range(boundsSize, terrainData.size.z - boundsSize)), Quaternion.identity, headsFolder);
            //head.tag = "CollectibleHead";
        }

        // Generation of links
        Transform linksFolder = GameObject.Find("Links").transform;
        int numberOfCollectibleLinks = Random.Range(minCollectibleLinks, maxCollectibleLinks);
        for (int i = 0; i < numberOfCollectibleLinks; i++)
        {
            Transform link = Instantiate(collectibleLinkPrefab, new Vector3(Random.Range(boundsSize, terrainData.size.x - boundsSize), spawnHeight, Random.Range(boundsSize, terrainData.size.z - boundsSize)), Quaternion.identity, linksFolder);
            link.name = "link" + i;
        }

        Vector2 orig = new Vector2(Random.value * 100f, Random.value * 100f);

        // Generation of terrain
        float[,] heights = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);
        for (int i = 0; i < terrainData.heightmapWidth; i++)
        {
            for (int j = 0; j < terrainData.heightmapHeight; j++)
            {
                heights[i, j] = Mathf.PerlinNoise(orig.x + i / groundNoiseFactor, orig.y + j / groundNoiseFactor) / 2f;
            }
        }
        terrainData.SetHeights(0, 0, heights);

        // Generation of details
        float resolution = terrainData.heightmapResolution / (float)terrainData.detailResolution;
        orig = new Vector2(Random.value * 220f, Random.value * 220f);
        int[,] details = terrainData.GetDetailLayer(0, 0, terrainData.detailWidth, terrainData.detailHeight, 0);
        for (int i = 0; i < terrainData.detailWidth; i++)
        {
            for (int j = 0; j < terrainData.detailHeight; j++)
            {
                if (heights[Mathf.FloorToInt(resolution * i), Mathf.FloorToInt(resolution * j)] < maxGrassHeight / terrainData.size.y)
                {
                    if (Mathf.PerlinNoise(orig.x + i / detailsNoiseFactor, orig.y + j / detailsNoiseFactor) > minGrassProbability)
                    {
                        details[i, j] = 1;
                    }
                    else
                    {
                        details[i, j] = 0;
                    }
                }
                else
                {
                    details[i, j] = 0;
                }
            }
        }
        terrainData.SetDetailLayer(0, 0, 0, details);
    }
	
	void Update ()
    {
		
	}
}
