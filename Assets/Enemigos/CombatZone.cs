using UnityEngine;

public class CombatZone : MonoBehaviour
{
    [Header("Bloqueos")]
    [SerializeField] private Collider2D leftBlock;
    [SerializeField] private Collider2D rightBlock;

    [Header("Spawner")]
    [SerializeField] private EnemySpawner spawner;

    private bool activated = false;
    private bool cleared = false;

    private Collider2D triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        OpenZone();

        if (spawner != null)
            spawner.SetCombatZone(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated || cleared) return;
        if (!other.CompareTag("Player")) return;

        activated = true;

        

        CloseZone();

        if (spawner != null)
            spawner.SpawnAll();

      
    }

    public void ZoneCleared()
    {
        if (cleared) return;

        cleared = true;
        OpenZone();

        if (triggerCollider != null)
            triggerCollider.enabled = false;
    }

    private void CloseZone()
    {
        if (leftBlock != null)
            leftBlock.enabled = true;

        if (rightBlock != null)
            rightBlock.enabled = true;
    }

    private void OpenZone()
    {
        if (leftBlock != null)
            leftBlock.enabled = false;

        if (rightBlock != null)
            rightBlock.enabled = false;
    }
}