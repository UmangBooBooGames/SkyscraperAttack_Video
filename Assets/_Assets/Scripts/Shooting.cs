using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Animations.Rigging;

public class Shooting : MonoBehaviour
{
    public Rig rightHandRig, leftHandRig, spineRig;
    [SerializeField] private Transform rightHandTarget, leftHandTarget, centerSpineTarget;
    public float aimRange = 15f;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private float rightOffset, leftOffset, forwardOffset;
    private float currentRightWeight, currentLeftWieght, currentSpineWieght;
    private Transform rightTarget, leftTarget, centerTarget;
    [SerializeField] private GunSet[] gunSets;
    private int currentSelectedGunSet;
    public bool hasRocketLauncher;
    public bool hasgattLauncher;
    [SerializeField] Bullet bulletPrefab;
    public bool lft;
    public GameObject effect, upp;
    public bool sut;
    [Serializable]
    public class GunSet
    {
        public Gun[] Guns;

        public void EnableGuns(bool enable)
        {
            foreach (var gun in Guns)
            {
                gun.gameObject.SetActive(enable);
            }
        }
    }

    private void Start()
    {
        SelectGunSet(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!sut)
            {
                aimRange = 9;
                rightOffset = 9;
                leftOffset = -9;
                sut = true;
            }
            else
            {
                aimRange = 5;
                rightOffset = 5;
                leftOffset = -5;
                sut = false;
            }
        }
        SwitchGun();
        if (hasRocketLauncher || hasgattLauncher)
        {
            rightTarget = null;
            leftTarget = null;
            DetectCenter();
            SetSpineWeight();
        }
        else
        {
            DetectRightSide();
            if (lft)
            {
                DetectLeftSide();

            }
            HandelHandAim();
        }
        CheckForShoot();
    }

    void DetectRightSide()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position + transform.right * rightOffset + transform.forward * forwardOffset, aimRange, enemyLayerMask);
        rightTarget = enemies.Length > 0
            ? enemies.Select(c => c.transform)
                .OrderBy(t => Vector3.Distance(transform.position, t.position))
                .FirstOrDefault()
            : null;

        if (rightTarget != null)
        {
            rightHandTarget.position = Vector3.Lerp(rightHandTarget.position, rightTarget.position + new Vector3(0, 0.75f, 0), Time.deltaTime * 10);
        }
    }

    void DetectLeftSide()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position + transform.right * leftOffset + transform.forward * forwardOffset, aimRange, enemyLayerMask);
        leftTarget = enemies.Length > 0
            ? enemies.Select(c => c.transform)
                .OrderBy(t => Vector3.Distance(transform.position, t.position))
                .FirstOrDefault()
            : null;

        if (leftTarget != null)
        {
            leftHandTarget.position = Vector3.Lerp(leftHandTarget.position, leftTarget.position + new Vector3(0, 0.75f, 0), Time.deltaTime * 10);
        }
    }

    void DetectCenter()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, aimRange, enemyLayerMask);
        centerTarget = enemies.Length > 0
            ? enemies.Select(c => c.transform)
                .OrderBy(t => Vector3.Distance(transform.position, t.position))
                .FirstOrDefault()
            : null;

        if (centerTarget != null)
        {
            centerSpineTarget.position = Vector3.Lerp(centerSpineTarget.position, centerTarget.position + new Vector3(0, 0.5f, 0), Time.deltaTime * 10);
        }
    }


    private float fireDelay;
    private float elaspedTime;
    void CheckForShoot()
    {
        elaspedTime += Time.deltaTime;
        if (elaspedTime >= fireDelay)
        {
            elaspedTime = 0;
            Shoot();
        }
    }

    void Shoot()
    {
        if (leftTarget != null)
        {
            gunSets[currentSelectedGunSet].Guns[0].Shoot(leftTarget, currentSelectedGunSet);
        }
        if (rightTarget != null)
        {
            gunSets[currentSelectedGunSet].Guns[1].Shoot(rightTarget, currentSelectedGunSet);
        }
        if (centerTarget != null)
        {
            gunSets[currentSelectedGunSet].Guns[0].Shoot(centerTarget, currentSelectedGunSet);
        }
    }


    void HandelHandAim()
    {
        SetRightHandWeight();
        SetLeftHandWeight();
    }

    void SetRightHandWeight()
    {
        float targetWeight1 = rightTarget != null ? 1f : 0f;
        currentRightWeight = Mathf.Lerp(currentRightWeight, targetWeight1, Time.deltaTime * 10f);
        rightHandRig.weight = currentRightWeight;
    }

    void SetLeftHandWeight()
    {
        float targetWeight2 = leftTarget != null ? 1f : 0f;
        currentLeftWieght = Mathf.Lerp(currentLeftWieght, targetWeight2, Time.deltaTime * 10f);
        leftHandRig.weight = currentLeftWieght;
    }

    void SetSpineWeight()
    {
        rightHandRig.weight = 0f;
        leftHandRig.weight = 0f;
        float targetWeight3 = centerTarget != null ? 1f : 0f;
        currentSpineWieght = Mathf.Lerp(currentSpineWieght, targetWeight3, Time.deltaTime * 10f);
        spineRig.weight = currentSpineWieght;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + transform.right * rightOffset + transform.forward * forwardOffset, aimRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position + transform.right * leftOffset + transform.forward * forwardOffset, aimRange);
    }

    void SwitchGun()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            SelectGunSet(0);
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            SelectGunSet(1);
            upp.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            SelectGunSet(2);
            effect.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            SelectGunSet(3);
            effect.SetActive(false);
        }
    }

    public void SwitchGun(int gunIndex)
    {
        SelectGunSet(gunIndex);
    }

    public void SelectGunSet(int gunIndex)
    {
        currentSelectedGunSet = gunIndex;
        if (gunIndex == 2)
        {
            hasRocketLauncher = true;
            hasgattLauncher = false;
            aimRange = 9;
            rightOffset = 9;
            leftOffset = -9;
        }
        if (gunIndex == 1)
        {

            aimRange = 9;
            rightOffset = 9;
            leftOffset = -9;
        }
        if (gunIndex == 3)
        {
            hasRocketLauncher = false;
            hasgattLauncher = true;
            aimRange = 9;
            rightOffset = 9;
            leftOffset = -9;
        }
        fireDelay = gunSets[gunIndex].Guns[0].fireRate;
        for (int i = 0; i < gunSets.Length; i++)
        {
            gunSets[i].EnableGuns(false);
        }
        gunSets[currentSelectedGunSet].EnableGuns(true);
    }
}
