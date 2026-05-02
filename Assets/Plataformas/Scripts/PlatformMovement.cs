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

    void FixedUpdate()
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

  
}