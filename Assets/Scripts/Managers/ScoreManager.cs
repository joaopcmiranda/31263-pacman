using System.Collections;
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

    public Text goText;

    private int m_HighScore;
    private ulong m_HighScoreTime;

    private bool m_IsTimerRunning;


    private int m_Score;
    private ulong m_StartTime;
    private ulong m_Timer;


    private void Start()
    {
        m_HighScore = PlayerPrefs.GetInt("highScore_score_lvl_" + level, 0);
        m_HighScoreTime = (ulong)PlayerPrefs.GetInt("highScore_time_lvl_" + level, 0);
        if (highScoreText != null) highScoreText.text = m_HighScore.ToString();
        if (highScoreTimeText != null) highScoreTimeText.text = TimeUtil.FormatTimeMs(m_HighScoreTime);
    }

    private void Update()
    {
        if (m_IsTimerRunning)
        {
            m_Timer = (ulong)(Time.time * 1000) - m_StartTime;
            if (timerText != null) timerText.text = TimeUtil.FormatTimeMs(m_Timer);

            if (scoreText != null) scoreText.text = m_Score.ToString();
        }
    }

    public IEnumerator Countdown()
    {
        goText.gameObject.SetActive(true);
        goText.text = "3";
        yield return new WaitForSeconds(1f);
        goText.text = "2";
        yield return new WaitForSeconds(1f);
        goText.text = "1";
        yield return new WaitForSeconds(1f);
        goText.text = "GO!";
        yield return new WaitForSeconds(1f);
        goText.gameObject.SetActive(false);
    }

    public void BeginTimer()
    {
        m_IsTimerRunning = true;
        m_StartTime = (ulong)(Time.time * 1000);
    }

    public void StopTimer()
    {
        m_IsTimerRunning = false;
    }

    public void AddScore(int scoreToAdd)
    {
        m_Score += scoreToAdd;
    }

    public void SaveHighScore()
    {
        if (m_Score > m_HighScore || (m_Score == m_HighScore && m_Timer < m_HighScoreTime))
        {
            m_HighScore = m_Score;
            m_HighScoreTime = m_Timer;

            if (highScoreText != null) highScoreText.text = m_HighScore.ToString();
            if (highScoreTimeText != null) highScoreTimeText.text = TimeUtil.FormatTimeMs(m_HighScoreTime);

            PlayerPrefs.SetInt("highScore_score_lvl_" + level, m_HighScore);
            PlayerPrefs.SetInt("highScore_time_lvl_" + level, (int)m_HighScoreTime);
            PlayerPrefs.Save();
        }
    }
}