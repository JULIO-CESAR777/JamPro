using UnityEngine;

public class TransitionTrigger : MonoBehaviour
{
    public int targetIndex;

    public Collider2D colliderBlock;

    private void Start()
    {
        if (colliderBlock != null)
            colliderBlock.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if (colliderBlock != null)
                colliderBlock.enabled = true;
        }

       
    }
}