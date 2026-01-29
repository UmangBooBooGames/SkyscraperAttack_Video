using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;
using TMPro;
public class zipline : MonoBehaviour
{
    public Transform a, b, player, liner;
    //  public CameraFollow cf;
    public Animator anm;
    public GameObject hlts;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject fance, cam, CHOICE, ft, ea2, ea3, gates, c1, box;
    public TextMeshProUGUI textMeshPro;
    public Image fill;
    public EnemyActivator ea;
    void Start()
    {
        // play();
    }

    public void play()
    {
        // player.parent = liner;
        anm.enabled = true;
        a.gameObject.SetActive(true);
        player.GetComponent<CharacterController>().enabled = false;
        player.GetComponent<Shooting>().enabled = false;
        player.transform.GetChild(0).gameObject.SetActive(false);
        player.transform.position = liner.GetChild(0).position;
        player.parent = liner.GetChild(0).transform;
        hlts.SetActive(false);
        player.transform.GetChild(0).gameObject.SetActive(false);
        liner.gameObject.SetActive(true);


        // cf.target = liner;
        StartCoroutine(delay());
    }
    void updateset1()
    {
        ea.minimumDistanceToActivate = 5000;
        int startValue = 0;
        int endValue = 10;

        int currentValue = 0;

        fill.DOFillAmount(1, 1);
        DOTween.To(() => currentValue, x =>
        {
            currentValue = x;
            textMeshPro.text = currentValue + "/" + 10;
        },
        10, 1).SetEase(Ease.Linear);

    }
    void updateset2()
    {
        ea.minimumDistanceToActivate = 5000;
        int startValue = 0;
        int endValue = 10;

        int currentValue = 0;

        fill.DOFillAmount(1, 1);
        DOTween.To(() => currentValue, x =>
        {
            currentValue = x;
            textMeshPro.text = currentValue + "/" + 30;
        },
        30, 1).SetEase(Ease.Linear);

    }
    IEnumerator delay()
    {






        yield return new WaitForSeconds(3.5f);
        player.position = liner.GetChild(0).position;

        player.transform.parent = null;
        player.transform.GetChild(0).gameObject.SetActive(true);
        player.transform.GetChild(1).gameObject.SetActive(true);
        liner.gameObject.SetActive(false);
        player.position = liner.GetChild(0).position;
        //  cf.target = player;
        player.position = liner.GetChild(0).position;
        hlts.SetActive(true);
        player.GetComponent<CharacterController>().enabled = true;
        player.GetComponent<Shooting>().enabled = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            play();
            c1.SetActive(false);
            //CHOICE.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {

            updateset1();
            StartCoroutine(updaly());

        }
        if (Input.GetKeyDown(KeyCode.R))
        {

            updateset2();
            StartCoroutine(updaly2());

        }
        if (Input.GetKeyDown(KeyCode.T))
        {

            updateset2();
            StartCoroutine(updaly3());

        }
    }
    IEnumerator updaly()
    {
        yield return new WaitForSeconds(1);
        CHOICE.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        CHOICE.SetActive(false);
        fance.SetActive(true);
        cam.SetActive(false);
        textMeshPro.text = "0/30";
        fill.fillAmount = 0;
        ea2.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        box.SetActive(false);
    }
    IEnumerator updaly2()
    {
        yield return new WaitForSeconds(1);
        CHOICE.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        CHOICE.SetActive(false);
        fance.SetActive(false);
        ft.SetActive(true);
        cam.SetActive(false);
        textMeshPro.text = "0/100";
        fill.fillAmount = 0;
        ea3.SetActive(true);
    }
    IEnumerator updaly3()
    {
        yield return new WaitForSeconds(1);
        CHOICE.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        CHOICE.SetActive(false);
        // fance.SetActive(false);
        //  ft.SetActive(true);
        cam.SetActive(false);
        textMeshPro.text = "0/100";
        fill.fillAmount = 0;
        ea3.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        gates.SetActive(false);
        c1.SetActive(true);
    }
}
