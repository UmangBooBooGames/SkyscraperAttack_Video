using UnityEngine;
using DG.Tweening;
public class CameraFollows : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 5f, -10f);
    public bool cax;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!cax)
            {
                ChangeOffset(new Vector3(-6, 12, -2), 0.7f);
                transform.DORotate(new Vector3(53.94f, 75, 0), 0.7f);
                cax = true;
            }
            else
            {
                ChangeOffset(new Vector3(-6, 12, 0), 0.7f);
                transform.DORotate(new Vector3(53.94f, 90, 0), 0.7f);
                cax = false;

            }
        }

    }
    public void camturn()
    {
        ChangeOffset(new Vector3(-6, 12, -2), 0.7f);
        transform.DORotate(new Vector3(53.94f, 75, 0), 0.7f);
        cax = true;
    }  public void camnorm()
    {
        ChangeOffset(new Vector3(-6, 12, 0), 0.7f);
        transform.DORotate(new Vector3(53.94f, 90, 0), 0.7f);
        cax = false;
    }

    void LateUpdate()
    {
        if (!target) return;

        transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * 10f);
    }
    public void ChangeOffset(Vector3 newOffset, float duration)
    {
        DOTween.To(() => offset, x => offset = x, newOffset, duration)
               .SetEase(Ease.InOutSine);
    }
}
