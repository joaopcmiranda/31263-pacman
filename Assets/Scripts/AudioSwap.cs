using System.Collections;
using UnityEngine;

public class AudioSwap : MonoBehaviour
{
    public AudioClip introBg;
    public AudioClip normalBg;
    private AudioSource m_BgMusicAudioSource;

    private void Start()
    {
        m_BgMusicAudioSource = GetComponent<AudioSource>();
        m_BgMusicAudioSource.clip = introBg;
        m_BgMusicAudioSource.Play();
        StartCoroutine(SwapMusicToNormal());
    }

    private IEnumerator SwapMusicToNormal()
    {
        yield return new WaitForSeconds(35.0f);
        m_BgMusicAudioSource.Stop();
        m_BgMusicAudioSource.clip = normalBg;
        m_BgMusicAudioSource.Play();
    }
}