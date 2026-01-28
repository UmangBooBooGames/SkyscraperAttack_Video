using System;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

public class Car : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] explosions;
    [SerializeField] private Transform targetPos;
    
    Transform player;
    [SerializeField] private Vector3 offset;
    [SerializeField] private GameObject vfx;
    [SerializeField] private MeshRenderer carRender;
    [SerializeField] private PrometeoCarController carController;

    private void Start()
    {
        player = PlayerController.instance.transform;
    }

    private void Update()
    {
        if (player != null)
        {
            player.transform.position = transform.position + offset;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.Damage(1000, true);
            if (enemy.isBoss)
            {
                CameraShake.instance.Shake(1,2f,2f,1);
                Vector3 spawnPos = transform.position;
                var explosion = ObjectPooling.Instance.Spawn<BloodParticle>(PoolType.missileExplosion,spawnPos);
                explosion.Play(Vector3.zero);
            }
        }

        if (other.CompareTag("barrier"))
        {
            other.tag = "Untagged";
            StartCoroutine(BlastSequnce());
            player = null;
            carController.enabled = false;
            CameraShake.instance.Shake(1,4f,4f);
            DOVirtual.DelayedCall(0.2f, () =>
            {
                PlayerController.instance.JumpOutfromTrolley(targetPos.position);
            });
        }
    }

    IEnumerator BlastSequnce()
    {
        for (int i = 0; i < explosions.Length; i++)
        {
            explosions[i].Play();
            yield return new WaitForSeconds(0.5f);
            vfx.SetActive(true);
        }
        carRender.material.DisableKeyword("_EMISSION");
    }
}
