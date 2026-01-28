using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyCosmetic : MonoBehaviour
{
    public Transform myParent;
    //public SkinnedMeshRenderer source;
    public Transform rootBone;
    public bool isPrefabGroup;
    public List<GameObject> ApplyItem = new List<GameObject> ();
    public GameObject[] spawnedClothes = new GameObject[4];
    public Dictionary<string,Transform> list;
    SkinnedMeshRenderer targetSkin;

    [ContextMenu("Apply Item")]
    public void ApplyThisCosm()
    {
        GameObject go;
        for (int i=0; i<ApplyItem.Count; i++)
        {
            go = Instantiate(ApplyItem[i]);
            targetSkin = go.GetComponent<SkinnedMeshRenderer>();
            targetSkin.rootBone = rootBone;
            go.transform.SetParent(myParent);
            if (rootBone.childCount > 0)
            {
                var targetSkinBones = targetSkin.bones;
                Transform[] newBones = new Transform[targetSkinBones.Length];

                for (int j = 0; j < targetSkinBones.Length; j++)
                {
                    newBones[j] = list[targetSkinBones[j].name];
                }
                targetSkin.bones = newBones;
            }
            spawnedClothes[i] = go;
        }
    }
}

public enum ApplyOn
{
    FACE=0,
    BODY=1,
    LEG=2,
    SHOE=3,
    HAIR=4,
}

public enum ApplyType
{
    REPLACE=0,
    OVER_TOP=1
}
