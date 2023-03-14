using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip titleScene;
    [SerializeField] private AudioClip levelScene;
    [SerializeField] private AudioClip inGame;
    [SerializeField] private AudioClip findTheExit;
    [SerializeField] private AudioClip levelComplete;
    [SerializeField] private AudioClip bonusStage;
    [SerializeField] private AudioClip boom;
    [SerializeField] private AudioClip dying;
    [SerializeField] private AudioClip justDied;
    [SerializeField] private AudioClip leftRight;
    [SerializeField] private AudioClip putBomb;
    [SerializeField] private AudioClip upDown;
    private AudioSource audioSource;
    private static AudioManager instance;
    public static AudioManager Instance { get => instance; }
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        AudioManager.instance = this;
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
        audioSource.clip = levelComplete;
        audioSource.loop = false;
        audioSource.Play();
    }
}
