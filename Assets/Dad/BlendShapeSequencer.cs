using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using Unity.VisualScripting;

public class BlendShapeSequencer : MonoBehaviour
{
    [Header("Blend Shape Settings")]
    public SkinnedMeshRenderer skinnedMesh;
    public float maxWeight = 100f;
    public float transitionSpeed = 3f;
    public float holdTime = 0.3f;

    [Header("Playback")]
    public bool loop = false;

    [Header("Start Delay")]
    public bool useRandomStartDelay = true;
    public Vector2 randomStartDelayRange = new Vector2(0.5f, 2f);

    [Header("Light Settings")]
    public Light targetLight;
    public float maxLightIntensity = 1000f;
    public float lightIncreaseSpeed = 1.5f;

    private int blendShapeCount;

    public Animator anim;
    [SerializeField] bool reverse;
    void Start()
    {
        transitionSpeed = 15;
        holdTime = 0;
        if (skinnedMesh == null)
        {
            Debug.LogError("SkinnedMeshRenderer not assigned!");
            return;
        }

        blendShapeCount = skinnedMesh.sharedMesh.blendShapeCount;

        //if (reverse)
        //{
        //    transitionSpeed = 15;
        //    ReversBlend();
        //    return;
        //}
        ResetAllBlendShapes();
        ResetLight();

        StartCoroutine(StartWithDelay());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            if (!reverse)
            {
                if (anim)
                    anim.enabled = true;
                transitionSpeed = 15;
                ReversBlend();
            }

        }
    }

    IEnumerator StartWithDelay()
    {
        if (useRandomStartDelay)
        {
            float delay = Random.Range(
                randomStartDelayRange.x,
                randomStartDelayRange.y
            );
            yield return new WaitForSeconds(delay);
        }

        // Start light animation together
        if (targetLight != null)
            StartCoroutine(IncreaseLightIntensity());

        yield return StartCoroutine(PlayBlendShapes());
    }

    public void ResetAllBlendShapes()
    {
        for (int i = 0; i < blendShapeCount; i++)
        {
            skinnedMesh.SetBlendShapeWeight(i, 0f);
        }
    }

    void ResetLight()
    {
        if (targetLight != null)
            targetLight.intensity = 0f;
    }

    IEnumerator PlayBlendShapes()
    {
        int previousIndex = -1;

        do
        {
            for (int currentIndex = 0; currentIndex < blendShapeCount; currentIndex++)
            {
                yield return StartCoroutine(
                    TransitionBlendShape(previousIndex, currentIndex)
                );

                previousIndex = currentIndex;
                yield return new WaitForSeconds(holdTime);
            }

            if (loop)
            {
                ResetAllBlendShapes();
                ResetLight();

                if (targetLight != null)
                    StartCoroutine(IncreaseLightIntensity());

                previousIndex = -1;
            }

        } while (loop);
    }

    IEnumerator TransitionBlendShape(int prev, int current)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;

            if (current >= 0)
            {
                float currentWeight = Mathf.Lerp(0f, maxWeight, t);
                skinnedMesh.SetBlendShapeWeight(current, currentWeight);
            }

            if (prev >= 0)
            {
                float prevWeight = Mathf.Lerp(maxWeight, 0f, t);
                skinnedMesh.SetBlendShapeWeight(prev, prevWeight);
            }

            yield return null;
        }

        if (prev >= 0)
            skinnedMesh.SetBlendShapeWeight(prev, 0f);

        if (current >= 0)
            skinnedMesh.SetBlendShapeWeight(current, maxWeight);
    }

    public void ReversBlend()
    {

        StartCoroutine(PlayBlendShapesReverse());

        if (targetLight != null)
            StartCoroutine(DecreaseLightIntensity());

    }

    IEnumerator PlayBlendShapesReverse()
    {
        int previousIndex = -1;

        do
        {
            for (int currentIndex = blendShapeCount - 1; currentIndex >= 0; currentIndex--)
            {
                yield return StartCoroutine(
                    TransitionBlendShape(previousIndex, currentIndex)
                );

                previousIndex = currentIndex;
                yield return new WaitForSeconds(holdTime);
            }

        } while (loop);
    }

    IEnumerator IncreaseLightIntensity()
    {
        float t = 0f;

        while (targetLight.intensity < maxLightIntensity)
        {
            t += Time.deltaTime * lightIncreaseSpeed;
            targetLight.intensity = Mathf.Lerp(0f, maxLightIntensity, t);
            yield return null;
        }

        targetLight.intensity = maxLightIntensity;
    }

    IEnumerator DecreaseLightIntensity()
    {
        float startIntensity = targetLight.intensity;
        float t = 0f;

        while (targetLight.intensity > 0f)
        {
            t += Time.deltaTime * lightIncreaseSpeed;
            targetLight.intensity = Mathf.Lerp(startIntensity, 0f, t);
            yield return null;
        }

        targetLight.intensity = 0f;
    }

 
}
