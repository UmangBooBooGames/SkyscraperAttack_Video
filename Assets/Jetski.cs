using DG.Tweening;
using UnityEngine;

public class Jetski : MonoBehaviour
{
    public float moveSpeed = 10f;     // Forward/backward speed
    public float turnSpeed = 60f;     // Rotation speed
    public float tiltAmount = 10f;    // Tilt angle when turning
    public float bobAmount = 0.2f;    // Up/down bobbing height
    public float bobSpeed = 2f;       // Bobbing speed

    private float baseY;
    
    [SerializeField] ParticleSystem[] waterParticles;

    void Start()
    {
        baseY = transform.position.y;
        DOVirtual.DelayedCall(0.8f, () =>
        {
            EnableParticles();
        }); // Save starting height for bobbing
    }

    void Update()
    {
        // Forward/Backward (W/S)
        float move = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        transform.Translate(Vector3.forward * move);

        // Turn Left/Right (A/D)
        float turn = Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up * turn);

        // Tilt when turning (roll effect)
        float targetTilt = -Input.GetAxis("Horizontal") * tiltAmount;
        Quaternion targetRotation = Quaternion.Euler(
            transform.eulerAngles.x,
            transform.eulerAngles.y,
            targetTilt
        );
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 2f);

        // Bobbing (simulate waves)
        float newY = baseY + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    public void EnableParticles()
    {
        foreach (ParticleSystem p in waterParticles)
        {
            p.Play();
        }
    }
}
