using System;
using UnityEngine;

public class Zipline : MonoBehaviour
{
    [SerializeField] private Transform startPos, endPos, middlePos;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;
            PlayerController.instance.ZipLiner(startPos.position,middlePos.position,endPos.position);
            CameraShake.instance.SwitchCamera(1);
        }
    }
}
