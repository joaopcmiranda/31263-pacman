using UnityEngine;
using UnityEngine.UI;

public class LifeManager : MonoBehaviour
{
    public Image life1;
    public Image life2;
    public Image life3;
    public Text gameOverText;

    public int lifeCount = 3;

    public int LoseLife()
    {
        lifeCount--;
        switch (lifeCount)
        {
            case 2:
                life3.enabled = false;
                break;
            case 1:
                life2.enabled = false;
                break;
            case 0:
                life1.enabled = false;
                gameOverText.enabled = true;
                break;
        }

        return lifeCount;
    }
    
    public void GameOver()
    {
        lifeCount = 0;
        life1.enabled = false;
        life2.enabled = false;
        life3.enabled = false;
        gameOverText.enabled = true;
    }
}