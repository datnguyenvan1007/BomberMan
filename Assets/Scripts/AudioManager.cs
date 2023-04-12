using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip levelStart;
    [SerializeField] private AudioClip inGame;
    [SerializeField] private AudioClip findTheExit;
    [SerializeField] private AudioClip levelComplete;
    [SerializeField] private AudioClip boom;
    [SerializeField] private AudioClip justDied;
    [SerializeField] private AudioClip leftRight;
    [SerializeField] private AudioClip putBomb;
    [SerializeField] private AudioClip upDown;
    private AudioSource audioSource;
    public static AudioManager instance;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        AudioManager.instance = this;
        if (PlayerPrefs.GetInt("Sound", 1) == 0) 
            Mute();
    }

    public void PlayAudioUpDown()
    {
        audioSource.PlayOneShot(upDown);
    }
    public void PlayAudioLeftRight()
    {
        audioSource.PlayOneShot(leftRight);
    }
    public void PlayAudioPutBomb()
    {
        audioSource.PlayOneShot(putBomb);
    }
    public void PlayAudioBoom()
    {
        audioSource.PlayOneShot(boom);
    }
    public void PlayAudioJustDied()
    {
        audioSource.PlayOneShot(justDied);
    }
    public void PlayAudioFindTheItem()
    {
        audioSource.PlayOneShot(findTheExit);
    }
    public void PlayAudioLevelComplete()
    {
        audioSource.PlayOneShot(levelComplete);
    }
    public void PlayAudioLevelStart()
    {
        audioSource.PlayOneShot(levelStart);
    }
    public void PlayAudioInGame()
    {
        // audioSource.clip = inGame;
        audioSource.Play();
    }
    public void Stop()
    {
        audioSource.Stop();
    }
    public void Mute()
    {
        audioSource.mute = true;
    }
    public void UnMute()
    {
        audioSource.mute = false;
    }
}
