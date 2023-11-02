using UnityEngine;

public class CherryController : MonoBehaviour
{
    public GameObject cherryPrefab;
    public GameObject cherryParent;

    private GameObject m_ActiveCherry;
    private bool m_IsCherryOnMap;
    private Level1Manager m_LevelManager;

    private Tween m_Tween;

    private void Start()
    {
        // spawn every 10 seconds
        InvokeRepeating(nameof(SpawnCherry), 10, 10);
        m_LevelManager = gameObject.GetComponent<Level1Manager>();
    }

    private void Update()
    {
        if (m_IsCherryOnMap)
        {
            var timeFraction = (Time.time - m_Tween.startTime) / m_Tween.duration;
            if (timeFraction < 1.0f)
                m_ActiveCherry.transform.position = Vector3.Lerp(m_Tween.startPos, m_Tween.endPos, timeFraction);
            else
                Destroy(m_ActiveCherry);
        }
    }

    private void SpawnCherry()
    {
        // randomise if x or y will vary
        var xOrY = Random.value > 0.5f;
        int x;
        int y;

        var xMax = m_LevelManager.LEVEL_MAP.GetLength(1) + 1;
        var yMax = m_LevelManager.LEVEL_MAP.GetLength(0) + 1;

        if (xOrY)
        {
            // randomise x
            x = Random.Range(0, xMax);
            // randomise y to be either 0 or the size of the map
            y = Random.Range(0, 2) * yMax;
        }
        else
        {
            // randomise y
            y = Random.Range(0, yMax);
            // randomise x to be either 0 or the size of the map
            x = Random.Range(0, 2) * xMax;
        }

        // spawn cherry at random location
        m_ActiveCherry = Instantiate(cherryPrefab, cherryParent.transform.position + new Vector3(x, y, 0),
            Quaternion.identity, cherryParent.transform);
        m_IsCherryOnMap = true;
        var startPos = m_ActiveCherry.transform.position;
        m_Tween = new Tween(startPos, CalculateDestinationPassingThroughCentre(startPos), Time.time, 7f);
    }


    private Vector2 CalculateDestinationPassingThroughCentre(Vector3 start)
    {
        var centre = m_LevelManager.GetWorldPositionForTile(m_LevelManager.GetCentreOfMap());

        var directionToCentre = centre - start;

        return centre + directionToCentre;
    }
    
    public void CherryEaten()
    {
        m_IsCherryOnMap = false;
        Destroy(m_ActiveCherry);
    }
}