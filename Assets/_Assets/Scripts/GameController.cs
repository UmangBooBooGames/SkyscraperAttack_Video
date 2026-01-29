using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public List<GameObject> allCameras;
    public Transform startPoint, endPoint;

    private void Awake()
    {
        instance = this;
    }
    IEnumerator Start()
    {
        PlayerController.instance.enabled = false;
        yield return new WaitForSeconds(1f);
        PlayerController.instance.PlayHook(startPoint, endPoint);
        // StartCoroutine(ReturnToBase());
    }
    IEnumerator ReturnToBase()
    {
        allCameras[0].SetActive(false);
        // allCameras[1].SetActive(true);
        yield return new WaitForSeconds(2f);
        allCameras[1].SetActive(false);
        yield return new WaitForSeconds(1);
        // baseController.Init();
    }
}
