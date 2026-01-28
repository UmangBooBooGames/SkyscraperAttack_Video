using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class TruckEnabler : MonoBehaviour
{
    [SerializeField] Truck[] trucks;
    [SerializeField] float minimumDistanceToActivate;
    [SerializeField] private Transform player;
    
    
    private void Start()
    {
        StartCoroutine(CheckForActivateGroup());
    }
    
    IEnumerator CheckForActivateGroup()
    {
        while (true)
        {
            foreach (Truck truck in trucks)
            {
                if (truck.isActive == true)
                {
                    continue;
                }
                if (Vector3.Distance(player.position, truck.transform.position) < minimumDistanceToActivate)
                {
                    truck.MoveTruck();
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
