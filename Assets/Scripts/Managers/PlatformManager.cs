using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformManager : MonoBehaviour
{
    public Platform platformPrefab;

    public int amountOfPlatforms = 15;
    public float startOffset = 5f;

    Platform[] platforms;
    Vector3 nextPosition;

    void Awake()
    {
        for (int i = 0; i < amountOfPlatforms; i++)
        {
            CreatePlatform(i);
        }
    }

    void Update()
    {

    }

    void CreatePlatform(int i)
    {
        Vector2 position;
        position.x = ((float)i - startOffset) * Random.Range(1, 2);
        position.y = Random.Range(-0.5f, 0.5f);
        Platform platform = Instantiate<Platform>(platformPrefab);
        platform.transform.localPosition = position;
        platform.name = "Platform " + (i + 1);
        Debug.Log("created platform");
    }
}