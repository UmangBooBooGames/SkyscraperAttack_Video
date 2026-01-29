using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceMover : MonoBehaviour
{
    public List<MovementData> movementList;

    [Header("Jump")]
    public float jumpHeight = 2f;

    [Header("Rotation")]
    public float smoothRotateSpeed = 6f; // 🔥 higher = faster turn

    [Header("Animator")]
    public Animator animator;
    public string fw = "fw";
    public string bw = "bw";
    public string jmp = "jmp";

    private Coroutine moveRoutine;
    private void Start()
    {

        PlaySequence();
    }
    public void PlaySequence()
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveSequence());
    }

    IEnumerator MoveSequence()
    {
        for (int i = 0; i < movementList.Count; i++)
        {
            MovementData data = movementList[i];

            PlayAnimation(data);

            Vector3 startPos = transform.position;
            Vector3 endPos = data.pos.position;

            float duration = Mathf.Max(0.01f, data.duration);
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / duration;

                // 🔹 Movement
                if (data.jump)
                {
                    float yOffset = Mathf.Sin(t * Mathf.PI) * jumpHeight;
                    transform.position = Vector3.Lerp(startPos, endPos, t) + Vector3.up * yOffset;
                }
                else
                {
                    transform.position = Vector3.Lerp(startPos, endPos, t);
                }

                // 👀 Smooth look WHILE moving
                SmoothLookAt(i, data);

                yield return null;
            }

            transform.position = endPos;
        }
    }

    // 🎭 ANIMATION
    void PlayAnimation(MovementData data)
    {
        if (!animator) return;

        if (data.jump)
            animator.CrossFade(jmp, 0.05f);
        else if (data.fwalk)
            animator.CrossFade(fw, 0.05f);
        else if (data.bwalk)
            animator.CrossFade(bw, 0.05f);
    }

    // 👀 SMOOTH ROTATION (no snap)
    void SmoothLookAt(int index, MovementData data)
    {
        Transform lookTarget = null;

        if (data.jump || data.fwalk)
            lookTarget = data.pos;
        else if (data.bwalk && index > 0)
            lookTarget = movementList[index - 1].pos;

        if (!lookTarget) return;

        Vector3 dir = lookTarget.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(dir.normalized);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * smoothRotateSpeed
        );
    }
}
