using System;
using DG.Tweening;
using UnityEngine;

public class Crane : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Transform hingePos;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.instance.SwingOnCrane(hingePos.position);
            other.transform.parent = hingePos;
            DOVirtual.DelayedCall(0.2f,() =>
            {
                anim.enabled = true;
            });
        }
    }

    public void DropPlayer()
    {
        PlayerController.instance.DropPlayerToBoat();
    }
}
