using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private CanvasGroup canvasGroup;

    public PlayerSettings FirstPlayerSettings;
    public PlayerSettings SecondPlayerSettings;

    public event Action StartPressed;
    public event Action FadedOut;

    private void Start()
    {
        startButton.onClick.AddListener(StartButton_OnClick);
    }

    private void StartButton_OnClick()
    {
        if (StartPressed != null) StartPressed();
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        foreach (var f in Easing.Linear(1, 0, 2f))
        {
            canvasGroup.alpha = f;
            yield return null;
        }

        if (FadedOut != null) FadedOut();
    }

    public void Show()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        foreach (var f in Easing.Linear(0, 1, 2f))
        {
            canvasGroup.alpha = f;
            yield return null;
        }

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }
}