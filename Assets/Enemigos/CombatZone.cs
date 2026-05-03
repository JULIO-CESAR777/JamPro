using UnityEngine;
using System.Collections;

public class CombatZone : MonoBehaviour
{
    [Header("Bloqueos")]
    [SerializeField] private GameObject leftBlock;
    [SerializeField] private GameObject rightBlock;

    [Header("Spawner")]
    [SerializeField] private EnemySpawner spawner;

    private Animator animRight;

    private bool activated = false;
    private bool cleared = false;

    private Collider2D triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
        animRight = rightBlock.GetComponent<Animator>();
    }

    private void Start()
    {
        

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
                
        StartCoroutine(OpenZone());

        if (triggerCollider != null)
            triggerCollider.enabled = false;
    }

    private void CloseZone()
    {
        if (leftBlock != null)
        {
            leftBlock.SetActive(true);
        }

        if (rightBlock != null)
        {
            rightBlock.SetActive(true);
        }
    }

    private IEnumerator OpenZone()
    {
        if (rightBlock != null)
        {
            animRight.SetTrigger("Subir");

            yield return new WaitForSeconds(1f);

            rightBlock.SetActive(false);
        }
    }
}