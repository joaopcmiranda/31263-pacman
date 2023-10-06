using System;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Sprite outsideWallCorner;
    public Sprite outsideWallStraight;
    public Sprite insideWallCorner;
    public Sprite insideWallStraight;
    public Sprite pellet;
    public GameObject powerPellet;
    public Sprite tJunction;
    
    public GameObject tilePrefab;

    public GameObject powerPelletParent;
    public GameObject grid;

    private readonly int[,] levelMap =
    {
        { 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 7 },
        { 2, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 4 },
        { 2, 5, 3, 4, 4, 3, 5, 3, 4, 4, 4, 3, 5, 4 },
        { 2, 6, 4, 0, 0, 4, 5, 4, 0, 0, 0, 4, 5, 4 },
        { 2, 5, 3, 4, 4, 3, 5, 3, 4, 4, 4, 3, 5, 3 },
        { 2, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 2, 5, 3, 4, 4, 3, 5, 3, 3, 5, 3, 4, 4, 4 },
        { 2, 5, 3, 4, 4, 3, 5, 4, 4, 5, 3, 4, 4, 3 },
        { 2, 5, 5, 5, 5, 5, 5, 4, 4, 5, 5, 5, 5, 4 },
        { 1, 2, 2, 2, 2, 1, 5, 4, 3, 4, 4, 3, 0, 4 },
        { 0, 0, 0, 0, 0, 2, 5, 4, 3, 4, 4, 3, 0, 3 },
        { 0, 0, 0, 0, 0, 2, 5, 4, 4, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 2, 5, 4, 4, 0, 3, 4, 4, 0 },
        { 2, 2, 2, 2, 2, 1, 5, 3, 3, 0, 4, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 5, 0, 0, 0, 4, 0, 0, 0 }
    };


    private void Start()
    {
        // clear the level
        powerPelletParent.SetActive(false);
        grid.SetActive(false);
        
        // set up camera size and position
        var camera = Camera.main;
        if (camera == null) throw new NullReferenceException("Camera.main is null");
        camera.orthographicSize = levelMap.GetLength(0);
        camera.transform.position = new Vector3(levelMap.GetLength(1), levelMap.GetLength(0), -10);
        
        
        GenerateLevel(PrepareLevelMap());
    }

    private int[,] PrepareLevelMap()
    {
        // Mirror the levelMap array into 4 quadrants not copying the bottom row
        var newLevelMap = new int[levelMap.GetLength(0) * 2 - 1, levelMap.GetLength(1) * 2];

        var rows = levelMap.GetLength(0);
        var cols = levelMap.GetLength(1);

        for (var i = 0; i < rows; i++)
        for (var j = 0; j < cols; j++)
        {
            var value = levelMap[i, j];

            // Top left
            newLevelMap[i, j] = value;

            // Top right
            newLevelMap[i, 2 * cols - 1 - j] = value;

            // Bottom left
            newLevelMap[2 * rows - 2 - i, j] = value;

            // Bottom right
            newLevelMap[2 * rows - 2 - i, 2 * cols - 1 - j] = value;
        }

        return newLevelMap;
    }

    private void GenerateLevel(int[,] levelArray)
    {
        var rows = levelArray.GetLength(0);
        var cols = levelArray.GetLength(1);

        for (var i = 0; i < rows; i++)
        for (var j = 0; j < cols; j++)
        {
            var value = levelArray[i, j];
            var position = new Vector3(j, i, 0) + transform.position;

            switch (value)
            {
                case 0:
                    break;
                case 1:
                    CreateOutsideWallCorner(position, levelArray);
                    break;
                case 2:
                    CreateOutsideWallStraight(position, levelArray);
                    break;
                case 3:
                    CreateInsideWallCorner(position, levelArray);
                    break;
                case 4:
                    CreateInsideWallStraight(position, levelArray);
                    break;
                case 5:
                    CreatePellet(position);
                    break;
                case 6:
                    CreateTJunction(position, levelArray);
                    break;
                case 7:
                    CreatePowerPellet(position);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    private void CreateOutsideWallCorner(Vector3 position, int[,] levelArray)
    {
        var tile = Instantiate(tilePrefab,  position, Quaternion.identity, transform);
        tile.name = $"[{position.x}, {position.y}] Outside Wall Corner";
        tile.GetComponent<SpriteRenderer>().sprite = outsideWallCorner;
        
    }
    
    private void CreateOutsideWallStraight(Vector3 position, int[,] levelArray)
    {
        var tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
        tile.name =  $"[{position.x}, {position.y}] Outside Wall Straight";
        tile.GetComponent<SpriteRenderer>().sprite = outsideWallStraight;
        
    }
    
    private void CreateInsideWallCorner(Vector3 position, int[,] levelArray)
    {
        var tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
        tile.name =  $"[{position.x}, {position.y}] Inside Wall Corner";
        tile.GetComponent<SpriteRenderer>().sprite = insideWallCorner;
        
    }
    
    private void CreateInsideWallStraight(Vector3 position, int[,] levelArray)
    {
        var tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
        tile.name =  $"[{position.x}, {position.y}] Inside Wall Straight";
        tile.GetComponent<SpriteRenderer>().sprite = insideWallStraight;
        
    }
    
    private void CreatePellet(Vector3 position)
    {
        var tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
        tile.name =  $"[{position.x}, {position.y}] Pellet";
        tile.GetComponent<SpriteRenderer>().sprite = pellet;
        
    }
    
    private void CreatePowerPellet(Vector3 position)
    {
        var tile = Instantiate(powerPellet, position, Quaternion.identity, transform);
        tile.name =  $"[{position.x}, {position.y}] Power Pellet";
        
    }

    private void CreateTJunction(Vector3 position, int[,] levelArray)
    {
        var tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
        tile.name =  $"[{position.x}, {position.y}] T Junction";
        tile.GetComponent<SpriteRenderer>().sprite = tJunction;

    }
}