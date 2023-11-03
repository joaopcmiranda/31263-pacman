using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{

    private AudioSource sourceA;
    private AudioSource sourceB;
    private bool isSourceA = true;
    
    public AudioClip scaredMusic;
    public AudioClip normalMusic;
    public AudioClip oneDeathMusic; 

    private void Start()
    {
        sourceA = gameObject.AddComponent<AudioSource>();
        sourceB = gameObject.AddComponent<AudioSource>();
        sourceA.loop = true;
        sourceB.loop = true;
        sourceA.clip = normalMusic;
    }

    public void PlayScaredMusic()
    {
        SwapMusic(scaredMusic);
    }
    
    public void PlayNormalMusic()
    {
        SwapMusic(normalMusic);
    }
    
    public void PlayOneDeathMusic()
    {
        SwapMusic(oneDeathMusic);
    }
    
    private void SwapMusic(AudioClip song)
    {
        var activeSource = (isSourceA) ? sourceA : sourceB;
        var inactiveSource = (isSourceA) ? sourceB : sourceA;
        
        inactiveSource.clip = song;
        inactiveSource.Play();
        
        StartCoroutine(FadeTrack(activeSource, inactiveSource));
    }
    
    private IEnumerator FadeTrack(AudioSource active, AudioSource inactive)
    {
        const float timeToFade = 1f;
        const float maxVolume = .5f;
          
        for (float t = 0; t < timeToFade; t += Time.deltaTime)
        {
            active.volume = maxVolume * ((timeToFade - t) / timeToFade);
            inactive.volume = maxVolume * (t / timeToFade);
            yield return null;
        }
        
        active.Stop();
        active.volume = 1;
        isSourceA = !isSourceA;
    }
}
