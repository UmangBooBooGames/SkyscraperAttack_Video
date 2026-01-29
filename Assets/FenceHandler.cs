using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class FenceHandler : MonoBehaviour
{

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bunchOutline2 = GetComponent<Outline>();
    }
    bool scale;
    bool canScale;
    public Outline[] bunchOutline;
    public Outline bunchOutline2;
    public Animator fParent;
    // Update is called once per frame
    void Update()
    {
       

        if(end)
            return;

         if(Input.GetKeyDown(KeyCode.Alpha9))
        {
            end = true;
            transform.DOPause();
            transform.localScale = new Vector3(1917.793f,1917.793f,1917.793f);
            ot = true;
            if(bunchOutline2)
             bunchOutline2.enabled = false;
            OutLineManager();
            fParent.enabled = true;
        }
        if(scale)
        {
            scale = false;
            if(bunchOutline2)
                 bunchOutline2.enabled = true;
            OutLineManager();
           //transform.DOLocalRotate(new Vector3(10,transform.localEulerAngles.y,0),.3f);
            transform.DOScale(new Vector3(2081.449f,2081.449f,2081.449f),.2f) .OnComplete(() =>
                {
                   //  transform.DOLocalRotate(new Vector3(0,transform.localEulerAngles.y,0),.3f);
                      if(bunchOutline2)
                        bunchOutline2.enabled = false;
                  OutLineManager();
                  transform.DOScale(new Vector3(1917.793f,1917.793f,1917.793f),.2f).OnComplete(() =>
                {
                  scale = true;
                  
                });
                  
                });
        }
    }

 public IEnumerator ShakeLocalPosition(
        Transform target,
        float duration = 0.3f,
        float strength = 10f,
        float frequency = 25f
    )
    {
        Vector3 startLocalPos = target.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float damper = 1f - (elapsed / duration);

            Vector3 offset = Random.insideUnitSphere * strength * damper;

            target.localPosition = startLocalPos + offset;

            yield return null;
        }

        target.localPosition = startLocalPos; // reset
    }
bool ot;
    public void OutLineManager()
    {
        ot = !ot;
        for(int i = 0; i < bunchOutline.Length;i++)
        {
            bunchOutline[i].enabled = ot;
        }
    }

    IEnumerator CanScaleCo()
    {
        yield return new  WaitForSeconds(1);
         scale = true;
    }

public bool end;
   private void OnTriggerEnter(Collider other)
    {
        if(end)
             return;
        if(other.tag == "Enemy")
        {
            if(!canScale)
            {
                canScale = true;
                StartCoroutine(CanScaleCo());
            }
                
        }

    }
    

}
