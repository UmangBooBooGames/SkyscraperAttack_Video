using System;
using UnityEngine;
using DG.Tweening;

public class Barrier : MonoBehaviour
{
    private bool pulseRunning;
    private Sequence emissionSeq;
    [SerializeField] private MeshRenderer targetRenderer;
    [SerializeField] private string floatPropertyName = "_EmissiveIntensity"; // or your shader's float name
    private MaterialPropertyBlock mpb;
    private float currentValue;
    public float duration;
    public float maxValue;

    public int breakLimit;
    public int currentLimit;
    [SerializeField] private Color BaseColor;
    public Collider col;
    
    void Awake()
    {
        mpb = new MaterialPropertyBlock();
    }

    

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Enemy enemy))
        {
            if (pulseRunning) return;
            pulseRunning = true;

            // Enable emission on material at index 1
            targetRenderer.materials[1].EnableKeyword("_EMISSION");

            // Create looping sequence
            emissionSeq = DOTween.Sequence();
            emissionSeq.Append(DOTween.To(() => currentValue, SetEmissionValue, maxValue, duration));
            emissionSeq.Append(DOTween.To(() => currentValue, SetEmissionValue, 0f, duration));
            emissionSeq.OnComplete(() =>
            {
                pulseRunning = false;
            });

            currentLimit++;
            if (currentLimit == breakLimit)
            {
                BreakThis();
            }
        }
    }

    void Stop()
    {
        // Kill the sequence
        if (emissionSeq != null && emissionSeq.IsActive())
        {
            emissionSeq.Kill();
        }

        // Reset emission
        SetEmissionValue(0f);

        // Disable emission keyword
        targetRenderer.materials[1].DisableKeyword("_EMISSION");

        pulseRunning = false;
    }

    private void SetEmissionValue(float value)
    {
        currentValue = value;
        targetRenderer.GetPropertyBlock(mpb, 1);

        // Scale the emission color by intensity
        Color baseColor = BaseColor; // or expose this as [SerializeField]
        mpb.SetColor("_EmissionColor", baseColor * currentValue);

        targetRenderer.SetPropertyBlock(mpb, 1);
    }

    void BreakThis()
    {
        col.enabled = false;
        Quaternion targetQuat = Quaternion.Euler(-90f, transform.eulerAngles.y, transform.eulerAngles.z);
        transform.DORotateQuaternion(targetQuat, 0.5f).SetEase(Ease.Linear);
        Stop();
    }
}
