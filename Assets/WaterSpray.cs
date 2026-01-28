using System;
using UnityEngine;

public class WaterSpray : MonoBehaviour
{
    public Transform startPos;
    [SerializeField] private Shooting shooting;
    [SerializeField] private float angle;
    [SerializeField] private Transform target;
    private void Update()
    {
        /*if (shooting.centerTarget != null)
        {
            Vector3 target = shooting.centerTarget.position + new Vector3(0, 1.5f, 0);
            
            Vector3 direction = (target - startPos.position).normalized;

            if (direction.sqrMagnitude > 0.001f) // avoid zero direction
            {
                // Create target rotation
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

                // Smooth rotate
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
            }
        }
        else
        {
            transform.forward = startPos.forward;
        }*/
        transform.position = startPos.position;
        Vector3 dir = (target.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.Damage(1000);
        }
    }
}
