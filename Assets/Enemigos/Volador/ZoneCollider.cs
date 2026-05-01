using UnityEngine;

public class ZoneCollider : MonoBehaviour
{
    public MovementVolador eye;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            eye.SetPlayerInsideZone(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            eye.SetPlayerInsideZone(false);
        }
    }
}