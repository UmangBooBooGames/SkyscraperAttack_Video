using UnityEngine;
using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;

public class WeaponDropEffect : MonoBehaviour
{
    [Header("Float Settings")]
    [SerializeField] float floatHeight = 0.25f;
    [SerializeField] float floatDuration = 1.2f;

    [Header("Blink Settings")]
    [SerializeField] float blinkScale = 1.08f;
    [SerializeField] float blinkDuration = 0.6f;

    Vector3 startPos;
    Vector3 startScale;

    Tween floatTween;
    Tween blinkTween;

    public int weponIndex;
    public BlendShapeSequencer[] roots;
    public GameObject destroyEffect;
    public ChestInfo chestInfo;
    public int count = 5;
    public GameObject fillerObj;
    public Image filler;
    public int mCount;
    public TextMeshProUGUI moneyCount;
    public UpgradeUi upgradeUi;
    [SerializeField] bool ischest;
    void OnEnable()
    {
        startPos = transform.position;
        startScale = transform.localScale;
        mCount = 20;
        moneyCount.text = mCount.ToString();
        // StartFloat();
        // StartBlink();
    }

    void StartFloat()
    {
        floatTween = transform.DOMoveY(
                startPos.y + floatHeight,
                floatDuration
            )
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    void StartBlink()
    {
        blinkTween = transform.DOScale(
                startScale * blinkScale,
                blinkDuration
            )
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void Update()
    {
        if (fillerObj.activeInHierarchy)
        {
            if (scaleCo == null)
            {
                scaleCo = StartCoroutine(parentScaleEffect());
            }
        }

        // FillAmmount();
    }
    public float scaleAmount = 1.2f;   // how much bigger it gets
    public float duration = 0.2f;      // speed of wobble
    public int vibrato = 2;            // number of shakes
    public float elasticity = 0.5f;    // bounciness (0�1)
    Coroutine scaleCo;
    IEnumerator parentScaleEffect()
    {
        Vector3 originalScale = parent.transform.localScale;
        parent.transform.localScale = originalScale;
        // Apply punch scale (wobble effect)
        parent.transform.DOPunchScale(
            new Vector3(scaleAmount, scaleAmount, scaleAmount),
            duration,
            vibrato,
            elasticity
        ).OnComplete(() =>
        {
            // Ensure it snaps back perfectly
            // skm.materials = new Material[] { matToChange, matToChange };
            parent.transform.localScale = originalScale + new Vector3(0f, 0f, 0f);
            scaleCo = null;

        });
        yield return null;
    }

    void OnDisable()
    {
        floatTween?.Kill();
        blinkTween?.Kill();

        transform.position = startPos;
        transform.localScale = startScale;
    }

    public GameObject parent;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // CameraShake.instance.GenerateMoney(other.transform.position, true);

            if (inco == null)
                inco = StartCoroutine(InvestCo(other.transform));
            //Time.timeScale = .1f;
            //panel.SetActive(true);
            //parent.SetActive(false);
        }
    }
    Coroutine inco;
    IEnumerator DealyCall(float t, Action action)
    {

        yield return new WaitForSeconds(t);
        action?.Invoke();
    }


    IEnumerator FillMoneyCo()
    {
        while (mCount > 0)
        {
            mCount--;
            moneyCount.text = mCount.ToString();
            filler.fillAmount += 0.05f;
            yield return new WaitForSeconds(0.1f);
        }
    }
    IEnumerator InvestCo(Transform other)
    {
        if (upgradeUi)
        {
            upgradeUi.gameObject.SetActive(true);
            yield return new WaitForSeconds(.1f);
            upgradeUi.ScaleEff();
            yield return new WaitForSeconds(.5f);
            upgradeUi.gameObject.SetActive(false);
        }

        if (fillerObj)
        {
            fillerObj.gameObject.SetActive(true);
            //filler.DOFillAmount(1, 2.8f);

            StartCoroutine(FillMoneyCo());


        }
        if (count >= 0)
        {

            for (int i = 0; i < count; i++)
            {
                CameraShake.instance.InvestC(other.position, transform, this);
                yield return new WaitForSeconds(.05f);

            }

            yield return new WaitForSeconds(1f);

        }
        if (fillerObj)
        {
            fillerObj.gameObject.SetActive(false);
        }
        if (!ischest)
        {

            if (destroyEffect)
            {
                destroyEffect.SetActive(true);

                for (int i = 0; i < roots.Length; i++)
                {
                    roots[i].transitionSpeed = 5;
                    roots[i].ReversBlend();
                    yield return null;
                }
                if (!ischest)
                {
                    transform.DOLocalMoveY(transform.localPosition.y + 2, .17f).OnComplete(() =>
                   {
                       StartCoroutine(DealyCall(1f, () => PlayerController.instance.GetComponent<Shooting>().SwitchGun(weponIndex)));

                       transform.DOScale(Vector3.one * .2f, 1.5f).SetEase(Ease.InOutBack);
                       transform.DOJump(other.transform.position + new Vector3(0f, 1.5f, 0), 3, 1, 1.3f).SetEase(Ease.InOutBack)
                       .OnComplete(() =>
                   {

                       gameObject.tag = "F";
                       gameObject.SetActive(false);
                   });
                   });
                    //SawMill----
                    //  transform.DOLocalMoveY(transform.localPosition.y +2,.5f).OnComplete(() =>
                    // {
                    //    StartCoroutine( DealyCall(1.3f,()=>CameraShake.instance.player.GetComponent<Shooting>().SwitchGun(weponIndex)));

                    //     transform.DOScale(Vector3.one * .2f,2f).SetEase(Ease.InOutBack);
                    //     transform.DOJump(other.transform.position + new Vector3(0f,1.5f,0),3,1,2f).SetEase(Ease.InOutBack)
                    //     .OnComplete(() =>
                    // {

                    //      gameObject.tag = "F";
                    //      gameObject.SetActive(false);
                    // });
                    // });
                    //------

                    // transform.DOLocalMoveY(transform.localPosition.y +2,1.5f).OnComplete(() =>
                    // {
                    //    StartCoroutine( DealyCall(1.3f,()=>CameraShake.instance.player.GetComponent<Shooting>().SwitchGun(weponIndex)));

                    //     transform.DOScale(Vector3.one * .2f,2f).SetEase(Ease.InOutBack);
                    //     transform.DOJump(other.transform.position + new Vector3(0f,1.5f,0),3,1,2f).SetEase(Ease.InOutBack)
                    //     .OnComplete(() =>
                    // {

                    //      gameObject.tag = "F";
                    //      gameObject.SetActive(false);
                    // });
                    // });
                    // Vector3 ss = transform.localScale;
                    //  transform.localScale = Vector3.zero;
                    //transform.DOScale(ss,1f);

                    transform.DORotate(
                    new Vector3(0, 360f, 0),
                      1.5f,
                     RotateMode.LocalAxisAdd   // IMPORTANT for spinning
                     ).SetEase(Ease.Linear);

                    chestInfo.ChestOpen();

                    if (count >= 0)
                    {
                        yield return new WaitForSeconds(1.5f);
                    }
                    else
                        PlayerController.instance.UpgradePlayer();
                }

            }
        }

        if (!chestInfo)
        {
            PlayerController.instance.GetComponent<Shooting>().SwitchGun(weponIndex);
            gameObject.SetActive(false);
        }


    }
    public float fillSpeed = 5f;

    private float targetFill = 0f;
    public void FillAmmount()
    {
        if (filler)
        {
            filler.fillAmount = Mathf.Lerp(
          filler.fillAmount,
          targetFill,
          Time.deltaTime * fillSpeed
      );
        }
    }
    public int maxCoins = 20;
    public int currentCoins = 0;
    public void Invest()
    {

        if (currentCoins >= maxCoins)
            return;

        currentCoins++;

        // Calculate fill percentage
        targetFill = (float)currentCoins / (float)maxCoins;

        Debug.Log("Coin Added: " + currentCoins);
    }
}
