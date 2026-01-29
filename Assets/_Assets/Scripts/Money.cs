using System.Collections;
using DG.Tweening;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;

public class Money : MonoBehaviour
{
    public bool unlok;
    public bool canTake;
    public Transform unlockPlace;
    public WeaponDropEffect dropEffect;
    bool letFall;
    private void Start()
    {
        if (unlockPlace)
        {
            // StartCoroutine(Move(unlockPlace));
            return;
        }

        GetComponent<Rigidbody>().AddForce(Vector3.up * 2.5f, ForceMode.Impulse);
        GetComponent<Rigidbody>().AddTorque(Vector3.right * 2.5f, ForceMode.Impulse);

        // MoveToPlayer();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            letFall = true;
            StartCoroutine(InIt());
        }
    }
    IEnumerator InIt()
    {
        yield return new WaitForSeconds(2);
        canTake = true;
        if (!unlok)
            StartCoroutine(Move(PlayerController.instance.transform));
    }

    public void MoveToPlayer()
    {
        transform.DOJump(PlayerController.instance.transform.position,
            .4f, 1, 1f).OnComplete(() =>
            {
                Destroy(gameObject);
            });
    }
    public void MoveTo(Transform pos)
    {
        transform.DOJump(pos.position,
            2f, 1, 1f).OnComplete(() =>
            {
                Destroy(gameObject);
            });
    }

    public IEnumerator Move(Transform posToGo)
    {
        float t = 0;
        float tt = .8f; // total travel time
        Vector3 startPos = transform.position;
        Vector3 targetPos = posToGo.position;

        float distance = Vector3.Distance(startPos, targetPos);
        float arcHeight = distance * 0.5f;   // dynamic jump arc
        float sideCurve = 3f * (Random.value > 0.5f ? 1 : -1); // kabhi left, kabhi right
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<BoxCollider>().enabled = false;

        while (t < tt)
        {
            Vector3 pos = Vector3.Lerp(startPos + new Vector3(0, 1, 0), PlayerController.instance.transform.position, t / tt);
            Vector3 scale = Vector3.Lerp(transform.localScale, Vector3.zero, t / (tt * 4));
            transform.position = pos;
            //transform.localScale = scale;
            t += Time.deltaTime;
            yield return null;
        }
        if (dropEffect != null)
            dropEffect.Invest();
        //while (t < tt)
        //{
        //    float normalizedTime = t / tt;

        //    // Base linear movement
        //    Vector3 pos = Vector3.Lerp(startPos, PlayerController.instance.transform.position + new Vector3(0,1,0), normalizedTime);

        //    // Jump arc (upar-niche)
        //    pos.y += arcHeight * Mathf.Sin(normalizedTime * Mathf.PI);

        //    // Side arc (ek bar, random left/right)
        //    pos.z += sideCurve * Mathf.Sin(normalizedTime * Mathf.PI);

        //    // Apply position
        //    transform.position = pos;

        //    // Spin
        //    transform.Rotate(new Vector3(15f, 0, 720f) * Time.deltaTime);

        //    t += Time.deltaTime;
        //    yield return null;
        //}

        Destroy(gameObject);
    }



    public void InC(Transform posToGo)
    {
        transform.DOJump(posToGo.position, 4, 1, 1.5f).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
    private void OnTriggerStay(Collider other)
    {
        if (unlockPlace)
            return;

        if (!canTake)
        {
            if (other.tag == "Dn" && letFall)
            {
                Destroy(gameObject);
            }
            return;
        }
        if (other.tag == "GetCoins")
        {
            //if (!unlok)
            //    StartCoroutine(Move(other.transform));
        }
    }


}
