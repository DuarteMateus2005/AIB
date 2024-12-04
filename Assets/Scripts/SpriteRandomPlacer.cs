using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRandomPlacer : MonoBehaviour
{
    public GameObject spritePrefab;
    public GameObject Platformers;
    public int numSprites = 7;
    public int numPlatformers = 4;
    public float minX = -50f;
    public float maxX = 50f;
    public float minY = -3f;
    public float maxY = 5f;

    private void Start()
    {

        //placeplatforms();

        for (int i = 0; i < numSprites; i++)
        {
            float randomX = Random.Range(minX, maxX);
            //float randomY = Random.Range(minY, maxY);
            Vector3 spawnPosition = new Vector3(randomX, -9.5f, 0f);
            Instantiate(spritePrefab, spawnPosition, Quaternion.identity, transform);
        }
    }

    void placeplatorms(){

    for (int i = 0; i < numPlatformers; i++)
        {
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);
            Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);
            Instantiate(Platformers, spawnPosition, Quaternion.identity, transform);
        }

    }
}

