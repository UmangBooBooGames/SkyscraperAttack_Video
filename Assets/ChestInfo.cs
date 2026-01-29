using DG.Tweening;
using UnityEngine;

public class ChestInfo : MonoBehaviour
{
    public Transform door;
    public GameObject tresure;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChestOpen()
    {
          
        door.DOLocalRotate(Vector3.right * -100,.3f).OnUpdate(() =>
        {
            if(tresure)
        {
            tresure.SetActive(true);
          

        }
        });
        transform.DOShakeRotation(
    0.5f,
    new Vector3(20f, 0f, 0f), // strength
    10,                      // vibrato (how many shakes)
    90f,                     // randomness
    true                     // fade out
);
    }
}
