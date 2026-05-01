using System.Collections;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    public float waitTime = 1f;

    private Transform currentTarget;
    private bool waiting = false;

    void Start()
    {
        currentTarget = pointB;
    }

    void Update()
    {
        if (waiting) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            currentTarget.position,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, currentTarget.position) < 0.05f)
        {
            StartCoroutine(WaitAndSwitch());
        }
    }

    IEnumerator WaitAndSwitch()
    {
        waiting = true;

        yield return new WaitForSeconds(waitTime);

        currentTarget = currentTarget == pointA ? pointB : pointA;

        waiting = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") &&
            collision.contacts[0].normal.y < -0.5f)
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}