using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TileContent
{
    EMPTY,
    WALL,
    PELLET,
    POWER_PELLET,
    OUTSIDE_MAP
}

public class Level1Manager : MonoBehaviour
{
    public GameObject levelObject;

    public Sprite outsideWallCorner;
    public Sprite outsideWallStraight;
    public Sprite insideWallCorner;
    public Sprite insideWallStraight;
    public GameObject pellet;
    public GameObject powerPellet;
    public Sprite tJunction;

    public GameObject tilePrefab;

    public GameObject powerPelletParent;
    public GameObject grid;

    public GameObject teleporterLeft;
    public GameObject teleporterRight;
    public int[,] LEVEL_MAP = LevelGenerator.PrepareLevelMap(Level.Level1);


    private Transform m_LTransform;
    private ScoreManager m_ScoreManager;

    private void Start()
    {
        // clear the level
        powerPelletParent.SetActive(false);
        grid.SetActive(false);

        // set up camera size and position
        var mainCamera = Camera.main;
        if (mainCamera == null) throw new NullReferenceException("Camera.main is null");
        mainCamera.orthographicSize =
            Math.Max(LEVEL_MAP.GetLength(0) / 2f + 1f, LEVEL_MAP.GetLength(1) / 2f + 1f);
        mainCamera.transform.position = new Vector3(LEVEL_MAP.GetLength(1) / 2f + 6.5f,
            LEVEL_MAP.GetLength(0) / 2f + 1f, -10);

        m_LTransform = levelObject.transform;
        m_ScoreManager = gameObject.GetComponent<ScoreManager>();

        GenerateLevel(LEVEL_MAP, m_LTransform);

        var rows = LEVEL_MAP.GetLength(0);
        var cols = LEVEL_MAP.GetLength(1);

        // Prepare teleporters
        var leftTeleporterPosition = new Vector3(-1.94f, rows / 2f - .5f, -3);
        var rightTeleporterPosition = new Vector3(cols + 1.04f, rows / 2f - .5f, -3);

        teleporterLeft.transform.position = leftTeleporterPosition + m_LTransform.position;
        teleporterRight.transform.position = rightTeleporterPosition + m_LTransform.position;
    }


    public TileContent GetTileOnPosition(Vector2 position)
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

    public bool IsTileWalkable(Vector2 position)
    {
        return GetTileOnPosition(position) switch
        {
            TileContent.EMPTY => true,
            TileContent.PELLET => true,
            TileContent.POWER_PELLET => true,
            TileContent.OUTSIDE_MAP => true,
            _ => false
        };
    }

    private void GenerateLevel(int[,] levelArray, Transform parent)
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
                    LevelGenerator.CreateWall(tilePrefab, outsideWallCorner, position, levelArray, parent);
                    break;
                case 2:
                    LevelGenerator.CreateWall(tilePrefab, outsideWallStraight, position, levelArray, parent);
                    break;
                case 3:
                    LevelGenerator.CreateWall(tilePrefab, insideWallCorner, position, levelArray, parent);
                    break;
                case 4:
                    LevelGenerator.CreateWall(tilePrefab, insideWallStraight, position, levelArray, parent);
                    break;
                case 5:
                    LevelGenerator.CreatePellet(pellet, position, parent);
                    break;
                case 6:
                    LevelGenerator.CreatePowerPellet(powerPellet, position, parent);
                    break;
                case 7:
                    LevelGenerator.CreateWall(tilePrefab, tJunction, position, levelArray, parent);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public Vector3 GetWorldPositionForTile(Vector2 tilePosition)
    {
        return (Vector3)tilePosition + m_LTransform.position;
    }

    public Vector2 GetCentreOfMap()
    {
        // x is always even, y is always odd
        var x = (LEVEL_MAP.GetLength(1) - 1) / 2f;
        var y = (LEVEL_MAP.GetLength(0) - 1) / 2f - 1f;

        return new Vector2(x, y);
    }

    public void ExitToStartScreen()
    {
        SceneManager.LoadScene(0);
    }

    // Interactions

    public void PelletEaten(Vector2 position)
    {
        LEVEL_MAP[(int)position.y, (int)position.x] = 0;
        m_ScoreManager.AddScore(10);
    }

    public void PowerPelletEaten(Vector2 position)
    {
        LEVEL_MAP[(int)position.y, (int)position.x] = 0;
    }

    public void CherryEaten()
    {
        m_ScoreManager.AddScore(100);
    }
    
    public void GhostKilled()
    {
        m_ScoreManager.AddScore(300);
    }
    
    public void GameOver()
    {
        SceneManager.LoadScene(2);
    }
}