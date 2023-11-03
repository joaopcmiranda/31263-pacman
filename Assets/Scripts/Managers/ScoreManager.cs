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
    
    private bool _isTimerRunning = false;


    private void Start()
    {
        highScore = PlayerPrefs.GetInt("highScore_score_lvl_" + level, 0);
        highScoreTime = (ulong)PlayerPrefs.GetInt("highScore_time_lvl_" + level, 0);
        if (highScoreText != null) highScoreText.text = highScore.ToString();
        if (highScoreTimeText != null) highScoreTimeText.text = TimeUtil.FormatTimeMs(highScoreTime);
    }

    private void Update()
    {
        if (_isTimerRunning)
        {
            timer = (ulong)(Time.time * 1000) - startTime;
            if (timerText != null) timerText.text = TimeUtil.FormatTimeMs(timer);

            if (scoreText != null) scoreText.text = score.ToString();
        }
    }

    public void BeginTimer()
    {
        _isTimerRunning = true;
        startTime = (ulong)(Time.time * 1000);
    }

    public void StopTimer()
    {
        _isTimerRunning = false;
    }

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
    }
    
    public void SaveHighScore()
    {
        if (score > highScore || (score == highScore && timer < highScoreTime))
        {
            highScore = score;
            highScoreTime = timer;
            
            if (highScoreText != null) highScoreText.text = highScore.ToString();
            if (highScoreTimeText != null) highScoreTimeText.text = TimeUtil.FormatTimeMs(highScoreTime);
            
            PlayerPrefs.SetInt("highScore_score_lvl_" + level, highScore);
            PlayerPrefs.SetInt("highScore_time_lvl_" + level, (int)highScoreTime);
            PlayerPrefs.Save();
        }
    }
}