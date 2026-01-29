using UnityEngine;
[System.Serializable]

public class MovementData
{
    public Transform pos;

    public bool fwalk;
    public bool bwalk;
    public bool jump;
    public bool slerp;
    public bool lerp;

    [Header("Timing")]
    public float duration = 1f;

    [Header("Rotation")]
    public float rotation = 0.15f;
}
