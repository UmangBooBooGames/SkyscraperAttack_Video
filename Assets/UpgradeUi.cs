using UnityEngine;
using DG.Tweening;

public class UpgradeUi : MonoBehaviour
{
    public GameObject buttonToScale; 
     public float scaleAmount = 1.2f;   // how much bigger it gets
    public float duration = 0.3f;      // speed of wobble
    public int vibrato = 2;            // number of shakes
    public float elasticity = 0.5f;    // bounciness (0�1)
    private Vector3 originalScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         originalScale = transform.localScale;
         //ScaleEff();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ScaleEff()
    {
          buttonToScale.transform.DOScale(
            new Vector3(scaleAmount, scaleAmount, scaleAmount),
            .2f
        ).OnComplete(() =>
        {
            buttonToScale.transform.DOScale(
            originalScale,
            .2f);
        });
        ;
    }
}
