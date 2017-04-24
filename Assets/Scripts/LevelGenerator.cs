using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public float groundNoiseFactor;
    public float detailsNoiseFactor;
    public float grassHeight;
    public float grassHeightMargin;
    public float grassBorder;
    public float rockHeight;
    public float rockHeightMargin;
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
                heights[i, j] = Mathf.PerlinNoise(orig.x + i / groundNoiseFactor, orig.y + j / groundNoiseFactor);
            }
        }
        terrainData.SetHeights(0, 0, heights);

        // Generation of details and paint
        bool isGrass = false;
        float height = 0f, heightMargin = 0;
        float minRockHeight = (rockHeight - rockHeightMargin) / terrainData.size.y,
            maxRockHeight = (rockHeight + rockHeightMargin) / terrainData.size.y,
            minGrassHeight = (grassHeight - grassHeightMargin) / terrainData.size.y,
            maxGrassHeight = (grassHeight + grassHeightMargin) / terrainData.size.y;

        float heightmapRes = terrainData.heightmapResolution / (float)terrainData.detailResolution,
            alphamapRes = terrainData.alphamapResolution / (float)terrainData.detailResolution;
        int alphamapI = 0, alphamapJ = 0;

        orig = new Vector2(Random.value * 220f, Random.value * 220f);
        int[,] details = terrainData.GetDetailLayer(0, 0, terrainData.detailWidth, terrainData.detailHeight, 0);
        float[,,] alphamaps = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
        for (int i = 0; i < terrainData.detailWidth; i++)
        {
            for (int j = 0; j < terrainData.detailHeight; j++)
            {
                height = heights[Mathf.FloorToInt(heightmapRes * i), Mathf.FloorToInt(heightmapRes * j)];
                alphamapI = Mathf.FloorToInt(alphamapRes * i);
                alphamapJ = Mathf.FloorToInt(alphamapRes * j);

                // Determine whether there is grass or not here
                isGrass = false;
                if (height < maxGrassHeight)
                {
                    isGrass = true;
                }

                // Ground and rock painting
                if (height >= minRockHeight && height <= maxRockHeight)
                {
                    heightMargin = (height - minRockHeight) / (maxRockHeight - minRockHeight);
                    alphamaps[alphamapI, alphamapJ, 0] = 1f - heightMargin;
                    alphamaps[alphamapI, alphamapJ, 1] = 0;
                    alphamaps[alphamapI, alphamapJ, 2] = heightMargin;
                }
                else if (height < minRockHeight)
                {
                    alphamaps[alphamapI, alphamapJ, 0] = 1;
                    alphamaps[alphamapI, alphamapJ, 1] = 0;
                    alphamaps[alphamapI, alphamapJ, 2] = 0;
                }
                else if (height > maxRockHeight)
                {
                    alphamaps[alphamapI, alphamapJ, 0] = 0;
                    alphamaps[alphamapI, alphamapJ, 1] = 0;
                    alphamaps[alphamapI, alphamapJ, 2] = 1;
                }

                // Grass details
                if (isGrass)
                {
                    if (height < Random.Range(minGrassHeight - grassBorder, minGrassHeight))
                    {
                        details[i, j] = 1;
                    }
                    else
                    {
                        details[i, j] = 0;
                    }

                    // Grass painting
                    if (height >= minGrassHeight && height <= maxGrassHeight)
                    {
                        heightMargin = (height - minGrassHeight) / (maxGrassHeight - minGrassHeight);
                        alphamaps[alphamapI, alphamapJ, 0] = heightMargin;
                        alphamaps[alphamapI, alphamapJ, 1] = 1f - heightMargin;
                        alphamaps[alphamapI, alphamapJ, 2] = 0;
                    }
                    else if (height < minGrassHeight)
                    {
                        alphamaps[alphamapI, alphamapJ, 0] = 0;
                        alphamaps[alphamapI, alphamapJ, 1] = 1;
                    }
                    else if (height > maxGrassHeight)
                    {
                        alphamaps[alphamapI, alphamapJ, 1] = 0;
                    }
                }
                else
                {
                    details[i, j] = 0;
                }
            }
        }
        terrainData.SetDetailLayer(0, 0, 0, details);
        terrainData.SetAlphamaps(0, 0, alphamaps);
    }
	
	void Update ()
    {
		
	}
}
