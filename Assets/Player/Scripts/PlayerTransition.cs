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
    [SerializeField] private float fadeToBlackDuration = 0.3f;
    [SerializeField] private float fadeFromBlackDuration = 0.3f;
    [SerializeField] private float blackScreenWaitTime = 0.3f;

    private bool isTransitioning;
    private int index;

    private void Start()
    {
        SetBlackScreenAlpha(0f);
        isTransitioning = false;
        index = 0;

        if (cameraPoints.Count > 0)
            cam.transform.position = cameraPoints[index].position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning) return;

        TransitionTrigger trigger = other.GetComponent<TransitionTrigger>();

        if (trigger != null)
        {
            if (trigger.targetIndex == index)
                return;

            StartCoroutine(DoTransition(trigger.targetIndex));
        }
    }

    private IEnumerator DoTransition(int targetIndex)
    {
        if (targetIndex == index)
            yield break;

        isTransitioning = true;

        yield return StartCoroutine(FadeBlackScreen(0f, 1f, fadeToBlackDuration));

        index = Mathf.Clamp(targetIndex, 0, cameraPoints.Count - 1);
        cam.transform.position = cameraPoints[index].position;

        yield return new WaitForSeconds(blackScreenWaitTime);

        yield return StartCoroutine(FadeBlackScreen(1f, 0f, fadeFromBlackDuration));
        Debug.Log("Transición a: " + targetIndex);

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