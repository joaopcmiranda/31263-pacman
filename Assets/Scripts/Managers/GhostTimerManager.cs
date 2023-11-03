using UnityEngine;
using UnityEngine.UI;
using Util;

public class GhostTimerManager : MonoBehaviour
{
    public GameObject timerObject;

    private GhostManager m_GhostManager;

    private bool m_IsTimerRunning;

    private int m_StartTime;
    private int m_Timer = 10;
    private Text m_TimerText;

    private void Start()
    {
        m_TimerText = timerObject.GetComponent<Text>();
        m_GhostManager = GetComponent<GhostManager>();
    }

    private void Update()
    {
        if (m_IsTimerRunning && m_Timer > 0)
        {
            m_Timer = 10 - ((int)Time.time - m_StartTime);
            if (m_TimerText != null) m_TimerText.text = m_Timer.ToString();
        }

        if (m_Timer <= 3 && m_GhostManager.state == GhostState.Scared)
        {
            m_GhostManager.SetState(GhostState.Recovering);
        }
        else if (m_Timer <= 0 && m_GhostManager.state == GhostState.Recovering)
        {
            m_GhostManager.SetState(GhostState.Normal);
            ResetTimer();
        }
    }

    public void BeginTimer()
    {
        timerObject.SetActive(true);
        m_TimerText.text = m_Timer.ToString();
        m_IsTimerRunning = true;
        m_StartTime = (int)Time.time;
    }

    private void ResetTimer()
    {
        timerObject.SetActive(false);
        m_Timer = 10;
        m_StartTime = 0;
    }
}