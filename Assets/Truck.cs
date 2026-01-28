using DG.Tweening;
using UnityEngine;
using System.Collections;

public class Truck : MonoBehaviour
{
    [SerializeField] private Transform target;
    float distance;
    [SerializeField] private float speed;
    [SerializeField] private ParticleSystem[] explosions;
    public bool isActive;
    
    
    public void MoveTruck()
    {
        if(isActive) return;

        isActive = true;
        distance = Vector3.Distance(transform.position, target.position);
        transform.DOMove(target.position, distance/speed).SetEase(Ease.Linear).OnComplete(() =>
        {
            StartCoroutine(BlastSequnce());
            CameraShake.instance.Shake(1,1f,0.75f,2);
        });
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.Damage(1000);
        }
    }

    IEnumerator BlastSequnce()
    {
        for (int i = 0; i < explosions.Length; i++)
        {
            explosions[i].Play();
            yield return new WaitForSeconds(0.5f);
            //vfx.SetActive(true);
        }
        //carRender.material.DisableKeyword("_EMISSION");
    }
}
