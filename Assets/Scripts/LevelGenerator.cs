using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public float perlinNoiseFactor;
    public Transform headPrefab;
    public Transform linkPrefab;
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
            Transform link = Instantiate(linkPrefab, new Vector3(Random.Range(boundsSize, terrainData.size.x - boundsSize), spawnHeight, Random.Range(boundsSize, terrainData.size.z - boundsSize)), Quaternion.identity, linksFolder);
            link.name = "link" + i;
            link.tag = "CollectibleLink";
        }

        // Generation of terrain
        Vector2 orig = new Vector2(Random.value * 100f, Random.value * 100f);
        float[,] heights = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);
        for (int i = 0; i < terrainData.heightmapWidth; i++)
        {
            for (int j = 0; j < terrainData.heightmapHeight; j++)
            {
                heights[i, j] = Mathf.PerlinNoise(orig.x + i / perlinNoiseFactor, orig.y + j / perlinNoiseFactor) / 2f;
            }
        }
        //terrainData.SetHeights(0, 0, heights);
    }
	
	void Update ()
    {
		
	}
}
