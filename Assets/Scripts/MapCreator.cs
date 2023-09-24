using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class MapCreator : MonoBehaviour
{
    public GameObject pathTile;
    public GameObject grass;
    public GameObject enemyPrefab;
    public GameObject[,] groundTiles;
    public Sprite[] grassSprites;
    public GameObject boulderPrefab;
    private EnemyBehavior enemyBehavior;
    private string[] mapResources = { "Maps/map1", "Maps/map2", "Maps/map3" };
    private char[,] charGrid;
    private Vector2 startPosition;
    private List<string> directions;
    public static MapCreator Instance { get; private set; }
    private int gridWidth = 23;
    private int gridHeight = 9;

    void Start()
    {
        EnemyCreator.SetEnemyPrefab(enemyPrefab);
        LoadMapFromResources(mapResources[UnityEngine.Random.Range(0, mapResources.Length)]);
        CreatePathFromMap();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void LoadMapFromResources(string mapResource)
    {
        TextAsset mapData = Resources.Load<TextAsset>(mapResource + "/map");

        if (mapData == null)
        {
            Debug.LogError($"Failed to load map data from resource path: {mapResource + "/map"}");
            return;
        }

        string[] mapLines = mapData.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        gridWidth = mapLines[0].Length;
        gridHeight = mapLines.Length;
        charGrid = new char[gridWidth, gridHeight];

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                charGrid[x, gridHeight - y - 1] = mapLines[y][x];
            }
        }

        TextAsset pathData = Resources.Load<TextAsset>(mapResource + "/path");

        if (pathData == null)
        {
            Debug.LogError($"Failed to load path data from resource path: {mapResource + "/path"}");
            return;
        }

        string[] pathCoords = pathData.text.Split(new[] { ", ", "(", ")", " ", "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        float startX = float.Parse(pathCoords[0]);
        float startY = float.Parse(pathCoords[1]);
        startPosition = new Vector2(startX, startY);
        directions = new List<string>();
        for (int i = 2; i < pathCoords.Length; i++)
        {
            directions.Add(pathCoords[i]);
        }
    }

    private void CreatePathFromMap()
    {
        groundTiles = new GameObject[gridWidth, gridHeight];
        for (int y = gridHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (charGrid[x, y] == 'P')
                {
                    Instantiate(pathTile, new Vector3(x, y, 1), Quaternion.identity);
                }
                else if (charGrid[x, y] == 'E')
                {
                    GameObject tile = Instantiate(grass, new Vector3(x, y, 1), Quaternion.identity);
                    groundTiles[x, y] = tile;
                    Sprite randomGrassSprite = grassSprites[UnityEngine.Random.Range(0, grassSprites.Length)];
                    tile.GetComponent<SpriteRenderer>().sprite = randomGrassSprite;
                }
                else if (charGrid[x, y] == 'B')
                {
                    Instantiate(boulderPrefab, new Vector3(x, y, 0), Quaternion.identity);
                }
            }
        }
    }

    public void SpawnEnemyAndSendPath()
    {
        if (enemyPrefab != null)
        {
            int numEnemies = 5;
            StartCoroutine(SpawnEnemyWithDelay(numEnemies));
        }
        else
        {
            Debug.LogError("Enemy prefab not set!");
        }
    }

    private IEnumerator SpawnEnemyWithDelay(int numEnemies)
    {
        for (int i = 0; i < numEnemies; i++)
        {
            GameObject enemyInstance = EnemyCreator.GetPooledEnemy();  // Directly use static method
            enemyInstance.transform.position = new Vector3(startPosition.x, startPosition.y, 0);
            enemyInstance.SetActive(true);

            enemyBehavior = enemyInstance.GetComponent<EnemyBehavior>();
            if (enemyBehavior != null)
            {
                enemyBehavior.SetPath(startPosition, directions);
            }
            else
            {
                Debug.LogError("EnemyBehavior script not found!");
            }

            yield return new WaitForSeconds(1);
        }
    }
}
