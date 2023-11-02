using UnityEngine;
using UnityEngine.UI;
using Util;

public class ScoreManager : MonoBehaviour
{
    public int level = 1;

    public Text scoreText;
    public Text timerText;

    public Text highScoreText;
    public Text highScoreTimeText;

    private int highScore;
    private ulong highScoreTime;


    private int score;
    private ulong startTime;
    private ulong timer;


    private void Start()
    {
        highScore = PlayerPrefs.GetInt("highScore_score_lvl_" + level, 0);
        highScoreTime = (ulong)PlayerPrefs.GetInt("highScore_time_lvl_" + level, 0);
    }

    private void Update()
    {
        timer = (ulong)(Time.time * 1000) - startTime;
        if (timerText != null) timerText.text = TimeUtil.FormatTime(timer);

        if (scoreText != null) scoreText.text = score.ToString();

        if (highScoreText != null) highScoreText.text = highScore.ToString();

        if (highScoreTimeText != null) highScoreTimeText.text = TimeUtil.FormatTime(highScoreTime);
    }

    public void BeginTimer()
    {
        startTime = (ulong)(Time.time * 1000);
    }

    public void ResetTimer()
    {
        startTime = 0;
        timer = 0;
    }

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        if (score > highScore || (score == highScore && timer < highScoreTime))
        {
            highScore = score;
            highScoreTime = timer;
            PlayerPrefs.SetInt("highScore_score_lvl_" + level, highScore);
            PlayerPrefs.SetInt("highScore_time_lvl_" + level, (int)highScoreTime);
            PlayerPrefs.Save();
        }
    }
}