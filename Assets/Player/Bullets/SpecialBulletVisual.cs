using UnityEngine;

public class SpecialBulletVisual : MonoBehaviour
{
    [SerializeField] private Transform visualRoot;
    [SerializeField] private float baseLength = 1f;

    public void SetVisualLength(float length, float duration)
    {
        if (visualRoot == null)
        {
            visualRoot = transform;
        }

        Vector3 scale = visualRoot.localScale;
        scale.x = length / baseLength;
        visualRoot.localScale = scale;

        Destroy(gameObject, duration);
    }
}
