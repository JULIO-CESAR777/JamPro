using UnityEngine;

public class SpecialBulletVisual : MonoBehaviour
{
    [SerializeField] private Transform visualRoot;
    [SerializeField] private float baseLength = 1f;

    private Vector3 originalScale;

    private void Awake()
    {
        if (visualRoot == null)
        {
            visualRoot = transform;
        }

        originalScale = visualRoot.localScale;
    }

    public void SetVisualLength(float length, float duration)
    {
        if (visualRoot == null) return;

        Vector3 newScale = originalScale;
        newScale.x = length / baseLength;
        visualRoot.localScale = newScale;

        // Como el sprite normalmente tiene el pivote en el centro,
        // lo movemos la mitad hacia adelante para que empiece en el origen.
        visualRoot.localPosition = new Vector3(length * 0.5f, 0f, 0f);

        Destroy(gameObject, duration);
    }
}
