using System.Collections;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeTrigger : MonoBehaviour
{
    public static UpgradeTrigger Instance;
    [SerializeField] Transform rightOption, leftOption;
    [SerializeField] private Transform[] fences;
    [SerializeField] private GameObject fenceParent;
    [SerializeField] private GameObject canvas;
    [SerializeField] private ParticleSystem[] particle;
    [SerializeField] private ParticleSystem novaEffect;
    public Image filler;
    public GameObject flameT;
    public float speed;
    public GameObject uiBox;
    public GameObject fillerBox;
    private void Awake()
    {
        Instance = this;
    }
    public GameObject bt;
    private void Update()
    {
        if (flameT)
            flameT.transform.Rotate(Vector3.up, speed * Time.deltaTime);

        if (Input.GetKeyUp(KeyCode.Alpha9))
        {
            if (fenceParent != null)
            {
                fenceParent.GetComponent<Animator>().enabled = true;
            }

        }

        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            if (!fenceParent)
                gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;
            rightOption.DOScale(Vector3.one, 0.15f).OnComplete(() => { });
            leftOption.DOScale(Vector3.one, 0.15f);

            if (filler)
            {

                Time.timeScale = 1f;

                // StartCoroutine(InvestCo(PlayerController.instance.transform));
                filler.DOFillAmount(1, 2f).OnComplete(() =>
                {
                    //uiBox.SetActive(true);
                    //fillerBox.SetActive(false);
                    //Time.timeScale = .1f;

                    fenceParent.SetActive(true);
                    gameObject.SetActive(false);
                });


            }
            else
            {
                fenceParent.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }

    public CinemachineCamera virtualCamera;
    public void Build()
    {
        canvas.transform.DOScale(Vector3.zero, 0.25f);
        canvas.SetActive(false);
        novaEffect.gameObject.SetActive(true);
        rightOption.gameObject.SetActive(false);
        leftOption.gameObject.SetActive(false);
        fenceParent.SetActive(true);
        StartCoroutine(SpawnFence());
        CameraShake.instance.ChangeFov(70, 0.25f);
    }

    public void GunUpgrade(int gunindex)
    {
        canvas.transform.DOScale(Vector3.zero, 0.25f);
        canvas.SetActive(false);
        rightOption.gameObject.SetActive(false);
        novaEffect.gameObject.SetActive(true);
        leftOption.gameObject.SetActive(false);
        fenceParent.SetActive(true);
        PlayerController.instance.GetComponent<Shooting>().SelectGunSet(gunindex);
        if (gunindex == 2)
        {
            CameraShake.instance.ChangeFov(60);
        }

        if (!CameraShake.instance.hook1)
        {
            EnemyActivator.Instance.EnableEnemy();
        }
        //CameraShake.instance.ChangeFov(70,0.25f);
    }

    IEnumerator SpawnFence()
    {
        yield return new WaitForSeconds(0.25f);
        for (int i = 0; i < fences.Length; i++)
        {
            fences[i].localScale = Vector3.zero;
            fences[i].gameObject.SetActive(true);
            fences[i].DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBounce);
            particle[i].Play();
            yield return new WaitForSeconds(0.15f);
        }
    }

    IEnumerator InvestMoney()
    {
        int c = 0;
        while (c < 10)
        {
            GameObject Go = Instantiate(CameraShake.instance.money.gameObject, transform.position, Quaternion.identity);
            Go.GetComponent<Money>().unlok = true;
            Go.GetComponent<Money>().MoveTo(transform);
            c++;
            yield return new WaitForSeconds(.1f);
        }


    }
    IEnumerator InvestCo(Transform other)
    {


        for (int i = 0; i < 20; i++)
        {
            CameraShake.instance.InvestC(other.position, transform, null);
            yield return new WaitForSeconds(.05f);

        }
    }

}
