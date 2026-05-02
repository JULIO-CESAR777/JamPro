using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTransition : MonoBehaviour
{
    [SerializeField] private Image blackScreen;
    [SerializeField] private Camera cam;
    
    [SerializeField] private List<Transform> cameraPoints;

    [Header("Transición")]
    [SerializeField] private float fadeToBlackDuration = 0.5f;
    [SerializeField] private float fadeFromBlackDuration = 0.5f;
    [SerializeField] private float blackScreenWaitTime = 0.4f;

    private bool isTransitioning;
    private int index;

    private void Start()
    {
        SetBlackScreenAlpha(0f);
        isTransitioning = false;
        index = 0;
        cam.transform.position = cameraPoints[index].position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Transition") && !isTransitioning)
        {
            index++;
            StartCoroutine(DoTransition());
        }
    }

    private IEnumerator DoTransition()
    {
        isTransitioning = true;

        // Fade de transparente a negro
        yield return StartCoroutine(FadeBlackScreen(0f, 1f, fadeToBlackDuration));

        // Aquí puedes mover al jugador, cambiar cámara, cargar zona, etc.
        cam.transform.position = cameraPoints[index].position;
        yield return new WaitForSeconds(blackScreenWaitTime);

        // Fade de negro a transparente
        yield return StartCoroutine(FadeBlackScreen(1f, 0f, fadeFromBlackDuration));

        isTransitioning = false;
    }

    private IEnumerator FadeBlackScreen(float startAlpha, float endAlpha, float duration)
    {
        float counter = 0f;

        while (counter < duration)
        {
            counter += Time.deltaTime;

            float t = counter / duration;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t);

            SetBlackScreenAlpha(alpha);

            yield return null;
        }

        SetBlackScreenAlpha(endAlpha);
    }

    private void SetBlackScreenAlpha(float alpha)
    {
        if (blackScreen == null) return;

        Color color = blackScreen.color;
        color.a = alpha;
        blackScreen.color = color;
    }
}
