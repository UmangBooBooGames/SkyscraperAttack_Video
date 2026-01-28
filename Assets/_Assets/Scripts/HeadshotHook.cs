using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Timeline;

public class HeadshotHook : MonoBehaviour
{
    public static HeadshotHook Instance;
    

    public GameObject timeline;
    [SerializeField] private ParticleSystem muzzle;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject aim;
    [SerializeField] private Enemy enemy;
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject enemyNearGorup;
    [SerializeField] private Volume volume;
    private MotionBlur MotionBlur;
    [SerializeField] private ParticleSystem explosion1;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowBulletCinematic()
    {
        muzzle.Play();
        CameraShake.instance.SwitchCamera(1);
        CameraShake.instance.Shake(0.25f,0.75f,0.5f,1);
        Time.timeScale = 0.5f;
        StartCoroutine(ActiveBullet());
    }

    public IEnumerator ActiveBullet()
    {
        yield return new WaitForSeconds(0.25f);
        bullet.SetActive(true);
        timeline.SetActive(true);
        aim.SetActive(false);
    }

    public void BullletHit()
    {
        enemy.enabled = true;
        enemy.Damage();
        bullet.SetActive(false);
        CameraShake.instance.Shake(0.25f, 1.5f, 1f, 1);
        explosion1.Play();
        StartCoroutine(SwitchToGameplayScene());
        gun.SetActive(false);
    }

    IEnumerator SwitchToGameplayScene()
    {
        yield return new WaitForSeconds(0.3f);
        Time.timeScale = 1f;
        CameraShake.instance.SwitchCamera(2);
        //enemyNearGorup.SetActive(true);
        PlayerController.instance.GetComponent<Shooting>().enabled = true;
        if (volume != null && volume.profile.TryGet(out MotionBlur))
        {
            MotionBlur.intensity.value = 0.1f; // Initial value
        }
        else
        {
            Debug.LogWarning("Color Adjustments not found in the profile!");
        }
        yield return new WaitForSeconds(0.1f);
        EnemyActivator.Instance.enabled = true;
    }
    
}
