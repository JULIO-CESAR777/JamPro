using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        [Header("Capas")]
        public Transform layerA;
        public Transform layerB;

        [Header("Movimiento")]
        public float speedMultiplier = 1f;

        [Header("Opcional")]
        public bool useCustomWidth;
        public float customWidth = 20f;

        [HideInInspector] public float width;
    }

    [Header("Configuración")]
    [SerializeField] private Transform referencePoint;
    [SerializeField] private float baseSpeed = 1f;

    [Header("Capas de Parallax")]
    [SerializeField] private ParallaxLayer[] layers;

    private void Start()
    {
        if (referencePoint == null && Camera.main != null)
        {
            referencePoint = Camera.main.transform;
        }

        SetupLayers();
    }

    private void Update()
    {
        MoveParallax();
    }

    private void SetupLayers()
    {
        for (int i = 0; i < layers.Length; i++)
        {
            ParallaxLayer layer = layers[i];

            if (layer.layerA == null || layer.layerB == null)
                continue;

            layer.width = GetLayerWidth(layer);

            // Para moverse hacia la derecha, la segunda copia debe iniciar a la izquierda.
            Vector3 posB = layer.layerA.position;
            posB.x -= layer.width;
            layer.layerB.position = posB;
        }
    }

    private void MoveParallax()
    {
        for (int i = 0; i < layers.Length; i++)
        {
            ParallaxLayer layer = layers[i];

            if (layer.layerA == null || layer.layerB == null)
                continue;

            float moveSpeed = baseSpeed * layer.speedMultiplier * Time.deltaTime;

            layer.layerA.position += Vector3.right * moveSpeed;
            layer.layerB.position += Vector3.right * moveSpeed;

            CheckLoop(layer.layerA, layer.layerB, layer.width);
            CheckLoop(layer.layerB, layer.layerA, layer.width);
        }
    }

    private void CheckLoop(Transform currentLayer, Transform otherLayer, float width)
    {
        float referenceX = 0f;

        if (referencePoint != null)
        {
            referenceX = referencePoint.position.x;
        }

        // Si esta capa ya se fue demasiado a la derecha,
        // la mandamos a la izquierda de la otra copia.
        if (currentLayer.position.x - referenceX >= width)
        {
            Vector3 newPosition = currentLayer.position;
            newPosition.x = otherLayer.position.x - width;
            currentLayer.position = newPosition;
        }
    }

    private float GetLayerWidth(ParallaxLayer layer)
    {
        if (layer.useCustomWidth)
        {
            return layer.customWidth;
        }

        SpriteRenderer spriteRenderer = layer.layerA.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            return spriteRenderer.bounds.size.x;
        }

        Debug.LogWarning("No se encontró SpriteRenderer en " + layer.layerA.name + ". Usando customWidth.");
        return layer.customWidth;
    }
}