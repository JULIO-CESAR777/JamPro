using UnityEngine;
using System;
public class PlayerParallaxInGame : MonoBehaviour
{
    [Serializable]
    public class ParallaxLayer
    {
        public Transform layer;

        [Header("Movimiento")]
        [Range(0f, 1f)]
        public float parallaxFactor = 0.2f;

        public bool affectX = true;
        public bool affectY = false;

        [HideInInspector] public Vector3 startPosition;
    }

    [Header("Referencias")]
    [SerializeField] private Transform player;

    [Header("Capas")]
    [SerializeField] private ParallaxLayer[] layers;

    [Header("Configuración")]
    [SerializeField] private bool moveOppositeToPlayer = true;

    [Header("Límites")]
    [SerializeField] private bool useLimits = true;
    [SerializeField] private float maxOffsetX = 2.5f;
    [SerializeField] private float maxOffsetY = 1f;

    [Header("Suavizado")]
    [SerializeField] private bool useSmooth = true;
    [SerializeField] private float smoothSpeed = 8f;

    private Vector3 playerStartPosition;
    private GameManager gm;

    private void Start()
    {
        gm = GameManager.GetInstance();

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (player != null)
        {
            playerStartPosition = player.position;
        }

        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].layer != null)
            {
                layers[i].startPosition = layers[i].layer.position;
            }
        }
    }

    private void LateUpdate()
    {
        if (gm != null && gm.gameState == GameState.Pause) return;
        if (player == null) return;

        Vector3 playerDelta = player.position - playerStartPosition;

        for (int i = 0; i < layers.Length; i++)
        {
            ParallaxLayer currentLayer = layers[i];

            if (currentLayer.layer == null) continue;

            float directionMultiplier = moveOppositeToPlayer ? -1f : 1f;

            float offsetX = 0f;
            float offsetY = 0f;

            if (currentLayer.affectX)
            {
                offsetX = playerDelta.x * currentLayer.parallaxFactor * directionMultiplier;
            }

            if (currentLayer.affectY)
            {
                offsetY = playerDelta.y * currentLayer.parallaxFactor * directionMultiplier;
            }

            if (useLimits)
            {
                offsetX = Mathf.Clamp(offsetX, -maxOffsetX, maxOffsetX);
                offsetY = Mathf.Clamp(offsetY, -maxOffsetY, maxOffsetY);
            }

            Vector3 targetPosition = new Vector3(
                currentLayer.startPosition.x + offsetX,
                currentLayer.startPosition.y + offsetY,
                currentLayer.startPosition.z
            );

            if (useSmooth)
            {
                currentLayer.layer.position = Vector3.Lerp(
                    currentLayer.layer.position,
                    targetPosition,
                    smoothSpeed * Time.deltaTime
                );
            }
            else
            {
                currentLayer.layer.position = targetPosition;
            }
        }
    }

    public void ResetParallaxOrigin()
    {
        if (player != null)
        {
            playerStartPosition = player.position;
        }

        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].layer != null)
            {
                layers[i].startPosition = layers[i].layer.position;
            }
        }
    }
}
