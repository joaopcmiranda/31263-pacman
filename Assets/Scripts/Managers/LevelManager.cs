using System;
using UnityEngine;

public enum TileContent
{
    EMPTY,
    WALL,
    PELLET,
    POWER_PELLET,
    OUTSIDE_MAP
}

public class LevelManager : MonoBehaviour
{
    private static readonly int[,] levelMap =
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

    public static readonly int[,] LEVEL_MAP = PrepareLevelMap();

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


    private void Start()
    {
        // clear the level
        powerPelletParent.SetActive(false);
        grid.SetActive(false);

        // set up camera size and position
        var mainCamera = Camera.main;
        if (mainCamera == null) throw new NullReferenceException("Camera.main is null");
        mainCamera.orthographicSize = Math.Max(levelMap.GetLength(0) + .5f, levelMap.GetLength(1) + .5f);
        mainCamera.transform.position = new Vector3(levelMap.GetLength(1) + 5, levelMap.GetLength(0) - 1f, -10);


        GenerateLevel(LEVEL_MAP);
    }

    private static int[,] PrepareLevelMap()
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

    public static TileContent GetTileOnPosition(Vector2 position)
    {
        var row = (int)position.y;
        var col = (int)position.x;

        if (row < 0 || row >= LEVEL_MAP.GetLength(0) || col < 0 || col >= LEVEL_MAP.GetLength(1))
            return TileContent.OUTSIDE_MAP;

        var value = LEVEL_MAP[row, col];

        return value switch
        {
            0 => TileContent.EMPTY,
            5 => TileContent.PELLET,
            6 => TileContent.POWER_PELLET,
            _ => TileContent.WALL
        };
    }

    public static bool IsTileWalkable(Vector2 position)
    {
        return GetTileOnPosition(position) switch
        {
            TileContent.EMPTY => true,
            TileContent.PELLET => true,
            TileContent.POWER_PELLET => true,
            _ => false
        };
    }

    private void GenerateLevel(int[,] levelArray)
    {
        var rows = levelArray.GetLength(0);
        var cols = levelArray.GetLength(1);

        for (var i = 0; i < rows; i++)
        for (var j = 0; j < cols; j++)
        {
            var value = levelArray[i, j];
            var position = new Vector3(j, i, 0);

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
                    CreatePowerPellet(position);
                    break;
                case 7:
                    CreateTJunction(position, levelArray);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void CreatePellet(Vector3 position)
    {
        var tile = Instantiate(tilePrefab, position + transform.position, Quaternion.identity, transform);
        tile.name = $"[{position.x}, {position.y}] Pellet";
        Debug.Log(tile.name + " " + tile.transform.position);
        tile.GetComponent<SpriteRenderer>().sprite = pellet;
    }

    private void CreatePowerPellet(Vector3 position)
    {
        var tile = Instantiate(powerPellet, position + transform.position - new Vector3(0, 0.5f, 0),
            Quaternion.identity, transform);
        tile.name = $"[{position.x}, {position.y}] Power Pellet";
    }


    private void CreateOutsideWallCorner(Vector3 position, int[,] levelArray)
    {
        var tile = Instantiate(tilePrefab, position + transform.position, Quaternion.identity, transform);
        tile.name = $"[{position.x}, {position.y}] Outside Wall Corner";
        tile.GetComponent<SpriteRenderer>().sprite = outsideWallCorner;
        tile.transform.Rotate(new Vector3(0, 0, CalculateTileRotation(position, levelArray)));
    }

    private void CreateOutsideWallStraight(Vector3 position, int[,] levelArray)
    {
        var tile = Instantiate(tilePrefab, position + transform.position, Quaternion.identity, transform);
        tile.name = $"[{position.x}, {position.y}] Outside Wall Straight";
        tile.GetComponent<SpriteRenderer>().sprite = outsideWallStraight;
        tile.transform.Rotate(new Vector3(0, 0, CalculateTileRotation(position, levelArray)));
    }

    private void CreateInsideWallCorner(Vector3 position, int[,] levelArray)
    {
        var tile = Instantiate(tilePrefab, position + transform.position, Quaternion.identity, transform);
        tile.name = $"[{position.x}, {position.y}] Inside Wall Corner";
        tile.GetComponent<SpriteRenderer>().sprite = insideWallCorner;
        tile.transform.Rotate(new Vector3(0, 0, CalculateTileRotation(position, levelArray)));
    }

    private void CreateInsideWallStraight(Vector3 position, int[,] levelArray)
    {
        var tile = Instantiate(tilePrefab, position + transform.position, Quaternion.identity, transform);
        tile.name = $"[{position.x}, {position.y}] Inside Wall Straight";
        tile.GetComponent<SpriteRenderer>().sprite = insideWallStraight;
        tile.transform.Rotate(new Vector3(0, 0, CalculateTileRotation(position, levelArray)));
    }

    private void CreateTJunction(Vector3 position, int[,] levelArray)
    {
        var tile = Instantiate(tilePrefab, position + transform.position, Quaternion.identity, transform);
        tile.name = $"[{position.x}, {position.y}] T Junction";
        tile.GetComponent<SpriteRenderer>().sprite = tJunction;
        tile.transform.Rotate(new Vector3(0, 0, CalculateTileRotation(position, levelArray)));
    }

    private int CalculateTileRotation(Vector3 position, int[,] levelArray)
    {
        var piece = levelArray[(int)position.y, (int)position.x];
        var match = 0;

        switch (piece)
        {
            case 1:
                match = 2;
                return -90 + MatchCornerNeighbours(position, levelArray, match);
            case 2:
                match = 1;
                return -90 + MatchStraightNeighbours(position, levelArray, match);
            case 3:
                match = 4;
                return MatchCornerNeighbours(position, levelArray, match);
            case 4:
                match = 3;
                return MatchStraightNeighbours(position, levelArray, match);
            case 7:
                return MatchTJunctionNeighbours(position, levelArray);
            default:
                return 0;
        }
    }

    private int MatchStraightNeighbours(Vector3 position, int[,] levelArray, int corner)
    {
        var col = (int)position.x;
        var row = (int)position.y;
        if (col < 0 || col >= levelArray.GetLength(1) || row < 0 || row >= levelArray.GetLength(0))
            throw new ArgumentOutOfRangeException(nameof(position), "position is out of range");

        var above = GetValueAtPosition(row, col, -1, 0, levelArray);
        var below = GetValueAtPosition(row, col, 1, 0, levelArray);
        var left = GetValueAtPosition(row, col, 0, -1, levelArray);
        var right = GetValueAtPosition(row, col, 0, 1, levelArray);

        // The straight piece, the corner piece, the t junction

        var straight = levelArray[row, col];
        const int junction = 7;

        if (Is(above, corner, straight, junction) && Is(below, corner, straight, junction))
            return 0;
        if (Is(left, corner, straight, junction) && Is(right, corner, straight, junction))
            return 90;
        if (Is(above, corner, straight, junction) || Is(below, corner, straight, junction))
            return 0;

        return 90;
    }

    private int MatchCornerNeighbours(Vector3 position, int[,] levelArray, int straight)
    {
        var col = (int)position.x;
        var row = (int)position.y;
        if (col < 0 || col >= levelArray.GetLength(1) || row < 0 || row >= levelArray.GetLength(0))
            throw new ArgumentOutOfRangeException(nameof(position), "position is out of range");


        var above = GetValueAtPosition(row, col, -1, 0, levelArray);
        var below = GetValueAtPosition(row, col, 1, 0, levelArray);
        var left = GetValueAtPosition(row, col, 0, -1, levelArray);
        var right = GetValueAtPosition(row, col, 0, 1, levelArray);
        var topLeft = GetValueAtPosition(row, col, -1, -1, levelArray);
        var topRight = GetValueAtPosition(row, col, -1, 1, levelArray);
        var bottomLeft = GetValueAtPosition(row, col, 1, -1, levelArray);
        var bottomRight = GetValueAtPosition(row, col, 1, 1, levelArray);


        var corner = levelArray[row, col];
        const int junction = 7;

        // Perfect L 

        //   #
        // # L X
        //   X

        if (Is(above, straight, corner, junction)
            && Is(left, straight, corner, junction)
            && !Is(below, straight, corner, junction)
            && !Is(right, straight, corner, junction))
            return 180;

        //   #
        // X L #
        //   X

        if (Is(above, straight, corner, junction)
            && Is(right, straight, corner, junction)
            && !Is(below, straight, corner, junction)
            && !Is(left, straight, corner, junction))
            return -90;

        //   X
        // X L #
        //   #

        if (Is(below, straight, corner, junction)
            && Is(right, straight, corner, junction)
            && !Is(above, straight, corner, junction)
            && !Is(left, straight, corner, junction))
            return 0;

        //   X
        // # L X
        //   #

        if (Is(below, straight, corner, junction)
            && Is(left, straight, corner, junction)
            && !Is(above, straight, corner, junction)
            && !Is(right, straight, corner, junction))
            return 90;

        // Complex L


        // X #  
        // # L
        //   #

        if (Is(above, straight, corner, junction)
            && Is(left, straight, corner, junction)
            && Is(below, straight, corner, junction)
            && !Is(topLeft, straight, corner, junction))
            return 180;

        //   # X
        // # L #
        // 

        if (Is(above, straight, corner, junction)
            && Is(left, straight, corner, junction)
            && Is(right, straight, corner, junction)
            && !Is(topRight, straight, corner, junction))
            return -90;

        //   # 
        //   L #
        //   # X

        if (Is(above, straight, corner, junction)
            && Is(right, straight, corner, junction)
            && Is(below, straight, corner, junction)
            && !Is(bottomRight, straight, corner, junction))
            return 0;

        //    
        // # L #
        // X #

        if (Is(below, straight, corner, junction)
            && Is(left, straight, corner, junction)
            && Is(right, straight, corner, junction)
            && !Is(bottomLeft, straight, corner, junction))
            return 90;

        return 0;
    }

    private int MatchTJunctionNeighbours(Vector3 position, int[,] levelArray)
    {
        var col = (int)position.x;
        var row = (int)position.y;
        if (col < 0 || col >= levelArray.GetLength(1) || row < 0 || row >= levelArray.GetLength(0))
            throw new ArgumentOutOfRangeException(nameof(position), "position is out of range");

        var above = GetValueAtPosition(row, col, -1, 0, levelArray);
        var below = GetValueAtPosition(row, col, 1, 0, levelArray);
        var left = GetValueAtPosition(row, col, 0, -1, levelArray);
        var right = GetValueAtPosition(row, col, 0, 1, levelArray);

        int[] sides = { left, above, right, below };
        var rotation = -90;
        foreach (var side in sides)
        {
            rotation += 90;
            if (side is 3 or 4)
                return rotation;
        }

        return rotation;
    }

    private static bool Is(int target, int a, int b, int c)
    {
        return target == a || target == b || target == c;
    }

    private static int GetValueAtPosition(int row, int col, int rowOffset, int colOffset, int[,] array)
    {
        var newRow = row + rowOffset;
        var newCol = col + colOffset;

        if (newRow >= 0
            && newRow < array.GetLength(0)
            && newCol >= 0
            && newCol < array.GetLength(1))
            return array[newRow, newCol];

        return 0;
    }
}