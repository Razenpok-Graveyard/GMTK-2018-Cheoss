using UnityEngine;

public class SfxPlayer: MonoBehaviour
{
    [SerializeField] private AudioSource dragStarted;
    [SerializeField] private AudioSource dragStopped;
    [SerializeField] private AudioSource playerKill;
    [SerializeField] private AudioSource botKill;
    [SerializeField] private AudioSource warp;
    [SerializeField] private AudioSource gameOver;
    [SerializeField] private AudioSource godEffect;

    public void PlayDragStarted()
    {
        dragStarted.Play();
    }

    public void PlayDragStopped()
    {
        dragStopped.Play();
    }

    public void PlayWarp()
    {
        warp.Play();
    }

    public void PlayPlayerKill()
    {
        playerKill.Play();
    }

    public void PlayBotKill()
    {
        botKill.Play();
    }

    public void PlayGameOver()
    {
        gameOver.Play();
    }

    public void PlayGodEffect()
    {
        godEffect.Play();
    }
}