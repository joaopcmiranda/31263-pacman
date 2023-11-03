using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public enum TileContent
{
    Empty,
    Wall,
    Pellet,
    PowerPellet,
    Barrier,
    OutsideMap
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
    public int[,] LevelMap = LevelGenerator.PrepareLevelMap(Level.Level1);
    
    public PacStudentController playerController;
    
    private GhostManager m_GhostManager;
    private LifeManager m_LifeManager;
    private BackgroundMusicManager m_BackgroundMusicManager;


    private Transform m_LTransform;
    private ScoreManager m_ScoreManager;

    private int m_UneatenPellets;

    private void Start()
    {
        Setup();
        StartCoroutine(Begin());
    }

    private void Setup()
    {
        // Managers
        m_ScoreManager = gameObject.GetComponent<ScoreManager>();
        m_GhostManager = gameObject.GetComponent<GhostManager>();
        m_LifeManager = gameObject.GetComponent<LifeManager>();
        m_BackgroundMusicManager = gameObject.GetComponent<BackgroundMusicManager>();

        // cached variables
        playerController = GameObject.FindWithTag("Player").GetComponent<PacStudentController>();
        m_LTransform = levelObject.transform;

        // clear the level
        powerPelletParent.SetActive(false);
        grid.SetActive(false);

        // set up camera size and position
        var mainCamera = Camera.main;
        if (mainCamera == null) throw new NullReferenceException("Camera.main is null");
        mainCamera.orthographicSize =
            Math.Max(LevelMap.GetLength(0) / 2f + 1f, LevelMap.GetLength(1) / 2f + 1f);
        mainCamera.transform.position = new Vector3(LevelMap.GetLength(1) / 2f + 6.5f,
            LevelMap.GetLength(0) / 2f + 1f, -10);

        // Generate level
        GenerateLevel(LevelMap, m_LTransform);

        // Prepare teleporters
        var rows = LevelMap.GetLength(0);
        var cols = LevelMap.GetLength(1);

        var leftTeleporterPosition = new Vector3(-1.94f, rows / 2f - .5f, -3);
        var rightTeleporterPosition = new Vector3(cols + 1.04f, rows / 2f - .5f, -3);

        teleporterLeft.transform.position = leftTeleporterPosition + m_LTransform.position;
        teleporterRight.transform.position = rightTeleporterPosition + m_LTransform.position;
    }

    private IEnumerator Begin()
    {
        StartCoroutine(m_ScoreManager.Countdown());

        yield return new WaitForSeconds(3f);
        m_ScoreManager.BeginTimer();
        playerController.Begin();
        m_GhostManager.Begin();
        m_BackgroundMusicManager.PlayNormalMusic();
    }


    public TileContent GetTileOnPosition(Vector2 position)
    {
        var row = (int)position.y;
        var col = (int)position.x;

        if (row < 0 || row >= LevelMap.GetLength(0) || col < 0 || col >= LevelMap.GetLength(1))
            return TileContent.OutsideMap;

        var value = LevelMap[row, col];

        return value switch
        {
            0 => TileContent.Empty,
            5 => TileContent.Pellet,
            6 => TileContent.PowerPellet,
            8 => TileContent.Barrier,
            _ => TileContent.Wall
        };
    }

    public bool IsTileWalkable(Vector2 position)
    {
        return GetTileOnPosition(position) switch
        {
            TileContent.Empty => true,
            TileContent.Pellet => true,
            TileContent.PowerPellet => true,
            TileContent.OutsideMap => true,
            TileContent.Barrier => true,
            _ => false
        };
    }
    
    public bool IsTileWalkableForGhosts(Vector2 position)
    {
        return GetTileOnPosition(position) switch
        {
            TileContent.Empty => true,
            TileContent.Pellet => true,
            TileContent.PowerPellet => true,
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
                    m_UneatenPellets++;
                    break;
                case 6:
                    LevelGenerator.CreatePowerPellet(powerPellet, position, parent);
                    break;
                case 7:
                    LevelGenerator.CreateWall(tilePrefab, tJunction, position, levelArray, parent);
                    break;
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
        var x = (LevelMap.GetLength(1) - 1) / 2f;
        var y = (LevelMap.GetLength(0) - 1) / 2f - 1f;

        return new Vector2(x, y);
    }

    public void ExitToStartScreen()
    {
        SceneManager.LoadScene(0);
    }

    // Interactions

    public void PelletEaten(Vector2 position)
    {
        LevelMap[(int)position.y, (int)position.x] = 0;
        m_ScoreManager.AddScore(10);
        m_UneatenPellets--;
        if (m_UneatenPellets == 0) GameOver();
    }

    public void PowerPelletEaten(Vector2 position)
    {
        LevelMap[(int)position.y, (int)position.x] = 0;
        m_GhostManager.SetState(GhostState.Scared);
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
        m_LifeManager.GameOver();
        m_GhostManager.StopAllGhosts();
        playerController.StopPlayer();
        m_ScoreManager.StopTimer();
        m_ScoreManager.SaveHighScore();
        Invoke(nameof(ExitToStartScreen), 3f);
    }
}