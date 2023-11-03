using System;
using System.Collections.Generic;
using UnityEngine;

public enum Level
{
    Level1 = 0,
}



public class LevelGenerator : MonoBehaviour
{
    private static List<int[,]> _levels { get; } = new();
    static LevelGenerator()
    {
        int[,] level1 =
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
            { 0, 0, 0, 0, 0, 2, 5, 4, 4, 0, 3, 4, 4, 8 },
            { 2, 2, 2, 2, 2, 1, 5, 3, 3, 0, 4, 0, 0, 0 },
            { 8, 8, 8, 8, 8, 8, 5, 0, 0, 0, 4, 0, 0, 0 }
        };
        
        _levels.Add(level1);
    }
   
    
    public static int[,] PrepareLevelMap(Level level)
    {
        var levelMap = _levels[(int) level];
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
    
       public static void CreatePellet(GameObject prefab, Vector3 position, Transform parent)
    {
        var tile = Instantiate(prefab, position + parent.position, Quaternion.identity, parent);
        tile.name = $"[{position.x}, {position.y}] Pellet";
    }

    public static void CreatePowerPellet(GameObject prefab,Vector3 position, Transform parent)
    {
        var tile = Instantiate(prefab, position +  parent.position - new Vector3(0, 0.5f, 0),
            Quaternion.identity,  parent);
        tile.name = $"[{position.x}, {position.y}] Power Pellet";
    }

    public static void CreateWall(GameObject prefab, Sprite sprite, Vector3 position, int[,] levelArray, Transform parent)
    {
        var tile = Instantiate(prefab, position +  parent.position, Quaternion.identity,  parent);
        tile.name = $"[{position.x}, {position.y}] {sprite.name}";
        tile.GetComponent<SpriteRenderer>().sprite = sprite;
        tile.transform.Rotate(new Vector3(0, 0, CalculateTileRotation(position, levelArray)));
    }

    public static int CalculateTileRotation(Vector3 position, int[,] levelArray)
    {
        var piece = levelArray[(int)position.y, (int)position.x];
        int match;

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

    public static int MatchStraightNeighbours(Vector3 position, int[,] levelArray, int corner)
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

    public static int MatchCornerNeighbours(Vector3 position, int[,] levelArray, int straight)
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

    public static int MatchTJunctionNeighbours(Vector3 position, int[,] levelArray)
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

    public static bool Is(int target, int a, int b, int c)
    {
        return target == a || target == b || target == c;
    }
    
    public static int GetValueAtPosition(int row, int col, int rowOffset, int colOffset, int[,] array)
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