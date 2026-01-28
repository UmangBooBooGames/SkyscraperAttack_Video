using System;
using DG.Tweening;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
    [SerializeField] ParticleSystem particle;
    [SerializeField] private Transform particle1, particle2;
    [SerializeField] GameObject gun;
    [SerializeField] Barrier[]  barrels;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;
            // gun.SetActive(false);
            // particle.Play();
            // DisableParticles();
            // PlayerController.instance.GetComponent<Shooting>().SelectGunSet(2);
            foreach (Barrier barrel in barrels)
            {
                barrel.enabled = true;
                barrel.col.isTrigger = true;
            }
        }
    }

    void DisableParticles()
    {
        particle1.DOScale(Vector3.zero, 1f);
        particle2.DOScale(Vector3.zero, 1f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
