using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class Enemy : PoolableObject
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float speedMultiplier = 1f;
    public float rotationSpeed = 10f;
    public float jumpForce = 6f;
    public float stoppingDistance = 2f;
    public float gravity = -20f;
    public float climbSpeed = 3f;
    public float climbCheckDistance = 1f;
    public LayerMask wallLayer;
    public LayerMask enemyWallLayer;
    
    public bool isDead;
    public float rollSpeed;
    [SerializeField] Transform childTransform;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform target;
    
    private readonly int idleHash = Animator.StringToHash("Idle");
    private readonly int runHash = Animator.StringToHash("Running");
    private readonly int runHash2 = Animator.StringToHash("Running2");
    private int finalRunHash;
    private readonly int jumpHash = Animator.StringToHash("Jump");
    private readonly int climbHash = Animator.StringToHash("Climb");
    private readonly int climbUpHash = Animator.StringToHash("ClimbUp");
    private readonly int deathHash = Animator.StringToHash("Death");
    private readonly int attackHash = Animator.StringToHash("Attack");
    private readonly int standingHash = Animator.StringToHash("Standing");
    private int currentState;
    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 moveDir;
    private Vector3 offset;
    private bool isJumping;
    private bool isMoving;
    private bool isClimbing;
    private bool isClimbingUp;
    private float buildingTopY;
    private bool canStop;
    public bool isBoss;
    public bool specialEnemy;
    
    private static readonly int BaseColorID = Shader.PropertyToID("_DiffuseColor");
    private static readonly int OutlineColorID = Shader.PropertyToID("_OutlineColor");
    private static readonly int OutlineThicknessID = Shader.PropertyToID("_Outline_Thickness");

    private float currenHealth;
    [SerializeField] private float maxHealth;
    
    [SerializeField] private GameObject headFireEffect;
    [SerializeField] private ParticleSystem switchFireEffect;
    void Start()
    {
        currenHealth = maxHealth;
        controller = GetComponent<CharacterController>();
        target = PlayerController.instance.transform;
        currentState = idleHash;
        animator.Play(idleHash);
        int rand = Random.Range(0, 2);
        finalRunHash = rand == 0 ? runHash : runHash2;
        if (!isBoss)
        {
            offset = new Vector3(Random.Range(-3,3),0,Random.Range(-1,1));
            moveSpeed = Random.Range(3.5f, 5.5f) * speedMultiplier;
        }
    }

    void Update()
    {
        if(isDead) return;
        DetectClimbable();
        HandleMovement();
        HandleJumpAndGravity();
        HandleAnimation();
    }

    // --------------------------------------
    void DetectClimbable()
    {
        if (isClimbingUp) return; // prevent detection while climb-up animation is playing
        int layerMask = (1 << LayerMask.NameToLayer("wall")) | (1 << LayerMask.NameToLayer("enemywall"));
        
        // Forward ray to detect climbable wall
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, climbCheckDistance,layerMask))
        {
            isClimbing = true;
            velocity = Vector3.zero;
            buildingTopY = hit.collider.bounds.max.y;
            // Check if there's no wall above (i.e., top reached)
            if (!isClimbingUp && !Physics.Raycast(transform.position + Vector3.up * 1.75f, transform.forward, 0.5f))
            {
                TriggerClimbUp();
            }
            return;
        }

        isClimbing = false;
    }
    
    void TriggerClimbUp()
    {
        isClimbing = false;
        isClimbingUp = true;
        velocity = Vector3.zero;
        Vector3 toPos = new Vector3(transform.position.x, buildingTopY, transform.position.z);
        transform.DOJump(toPos + transform.forward * 0.5f, 0.3f, 1, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            isClimbingUp = false;
        });
        PlayAnim(climbUpHash);
    }

    // --------------------------------------
    void HandleMovement()
    {
        if (isClimbingUp) return; // block input
        if (isClimbing)
        {
            float verticalInput = 1f;
            if (verticalInput > 0.1f)
            {
                Vector3 climbMove = Vector3.up * climbSpeed * Time.deltaTime;
                controller.Move(climbMove);
            }
            return;
        }
        
        moveDir = target.position - transform.position + offset;
        canStop = Vector3.Distance(target.position, transform.position) < stoppingDistance;
        if (canStop)
        {
            moveDir = Vector3.zero;
            PlayAnim(attackHash);
        }
        moveDir.y = 0;
        moveDir = moveDir.normalized;
        isMoving = moveDir.magnitude > 0.1f;

        if (isMoving)
        {
            controller.Move(moveDir * moveSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime * 100f
            );
        }
    }

    // --------------------------------------
    void HandleJumpAndGravity()
    {
        if (isClimbing || isClimbingUp) return; // disable gravity during climb or climb-up

        bool isGrounded = controller.isGrounded || (velocity.y < 0 && Physics.Raycast(transform.position, Vector3.down, 0.2f));
        
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
            isJumping = true;
            PlayAnim(jumpHash);
        }
        else if (velocity.y < 0)
        {
            velocity.y = -2f;
            isJumping = false;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    // --------------------------------------
    void HandleAnimation()
    {
        if (isClimbingUp)
        {
            PlayAnim(climbUpHash);
            return;
        }

        if (isClimbing)
        {
            PlayAnim(climbHash);
            return;
        }

        if (!isJumping && !canStop)
        {
            if (isMoving)
                PlayAnim(finalRunHash);
            else
                PlayAnim(idleHash);
        }
    }

    // --------------------------------------
    void PlayAnim(int targetHash)
    {
        if (currentState == targetHash) return;

        animator.CrossFadeInFixedTime(targetHash, 0.25f);
        currentState = targetHash;
    }

    private bool hitByCar;
    public void Damage(int damage = 10, bool hitbyCar = false)
    {
        currenHealth -= damage;
        //PulseEmission(0.4f,0.05f);
        hitByCar  = hitbyCar;
        if (currenHealth <= 0)
        {
            Dead();
        }
        UpdateHealthUi();
        Vector3 spawnPos = transform.position + new Vector3(0, 1f, 0);

        if (isBoss || fireZombie)
        {
            var bloodParticle = ObjectPooling.Instance.Spawn<BloodParticle>(PoolType.yellowBlood,spawnPos);
            bloodParticle.Play(-transform.forward);
        }
        else
        {
            var bloodParticle = ObjectPooling.Instance.Spawn<BloodParticle>(PoolType.GreenBlood,spawnPos);
            bloodParticle.Play(-transform.forward);
        }
        
    }
    
    [SerializeField] private Image fillBar;
    [SerializeField] private GameObject healthBar;
    
    void UpdateHealthUi()
    {
        if (healthBar != null)
        {
            fillBar.fillAmount = (float) currenHealth / maxHealth;
            if (currenHealth <= 0)
            {
                healthBar.SetActive(false);
            }
        }
    }

    public void Dead()
    {
        Vector3 spawnPos = transform.position + new Vector3(0, 1.75f, 0);
         var coin = ObjectPooling.Instance.Spawn<BloodParticle>(PoolType.goldCoin,spawnPos);
         coin.Play(Vector3.up);
       // Vector3 floatingTextSpawnPos = CameraShake.instance.cam.WorldToScreenPoint(spawnPos);
       // FloatingText floatingText = ObjectPooling.Instance.Spawn<FloatingText>(PoolType.ScoreText,floatingTextSpawnPos);
        //floatingText.ShowText(1);
        if (emissionSeq != null && emissionSeq.IsActive())
        {
            emissionSeq.Kill();
        }

        if (fireZombie)
        {
            
        }
        else
        {
            if (CameraShake.instance.hook1)
            {
                //SetEmissionValue(0.2f);
                ChangeToGreyMaterial();
            }
            else
            {
                SetPlayerBlackDead(Color.black, Color.red, 0.005f);
            }
        }
        
        GetComponent<Collider>().enabled = false;
        if (controller != null)
        {
            controller.detectCollisions = false;
        }
        gameObject.layer = 0;
        gameObject.tag = "Untagged";
        isDead = true;
        PlayAnim(deathHash);
        if (hitByCar)
        {
            Vector3 targetDeathPos = transform.position - transform.forward * Random.Range(9,15);
            Vector3 randomAxis = Random.onUnitSphere;
            transform.DOJump(targetDeathPos, Random.Range(2f,3.5f), 1, 1.5f).SetEase(Ease.Linear).OnUpdate(() =>
            {
                childTransform.Rotate(randomAxis * rollSpeed * Time.deltaTime, Space.Self);
            });
            StartCoroutine(DisableObject());
        }
        else
        {
            DOVirtual.DelayedCall(3f, () =>
            {
                gameObject.SetActive(false);
            });
        }
    }

    IEnumerator DisableObject()
    {
        yield return new WaitForSeconds(5);
        gameObject.SetActive(false);
    }

    public void Active(Vector3 spawnPos)
    {
        transform.position = spawnPos;
        gameObject.SetActive(true);
    }

    [SerializeField] private Material burnMaterialA, burnMaterialB;
    [SerializeField] private Material greyMaterialA, greyMaterialB;
    [SerializeField] private SkinnedMeshRenderer[] targetRenderers;
    [SerializeField] private string floatPropertyName = "_EmissiveIntensity"; // or your shader's float name
    private MaterialPropertyBlock mpb;
    private float currentValue;

    void Awake()
    {
        mpb = new MaterialPropertyBlock();
    }

    private bool pulseRunning;
    private Sequence emissionSeq;
    public void PulseEmission(float maxValue, float duration)
    {
        /*if (pulseRunning == true)
        {
            return;
        }
         // Kill old tween if running
        pulseRunning = true;
        // Tween up
        DOTween.To(() => currentValue, SetEmissionValue, maxValue, duration)
            .OnComplete(() =>
            {
                // Tween down
                DOTween.To(() => currentValue, SetEmissionValue, 0f, duration).OnComplete(() =>
                {
                    pulseRunning = false;
                });
            });*/
        
        if (emissionSeq != null && emissionSeq.IsActive())
        {
            emissionSeq.Kill();
        }

        // Create new sequence
        emissionSeq = DOTween.Sequence();

        emissionSeq.Append(
            DOTween.To(() => currentValue, SetEmissionValue, maxValue, duration)
        );
        emissionSeq.Append(
            DOTween.To(() => currentValue, SetEmissionValue, 0f, duration)
        );
    }

    private void SetEmissionValue(float value)
    {
        currentValue = value;
        //targetRenderer.GetPropertyBlock(mpb,0);
        mpb.SetFloat(floatPropertyName, currentValue);
        //targetRenderer.SetPropertyBlock(mpb,0);
    }

    private void ChangeToGreyMaterial()
    {
        Material[] mats = targetRenderers[0].sharedMaterials;
        mats[0] = greyMaterialB;
        mats[1] = greyMaterialA;// copy of array
        targetRenderers[0].sharedMaterials = mats;
        targetRenderers[1].sharedMaterial = greyMaterialB;
        targetRenderers[2].sharedMaterial = greyMaterialA;
    }
    
    public void SetPlayerBlackDead(Color baseColor, Color outlineColor, float outlineThickness)
    {
        // Update Base Material (index 0)
        // targetRenderer.GetPropertyBlock(mpb, 0);
        mpb.SetColor(BaseColorID, baseColor);
        // targetRenderer.SetPropertyBlock(mpb, 0);

        // Update Outline Material (index 1)
        if(isBoss) return;
        //targetRenderer.GetPropertyBlock(mpb, 1);
        mpb.SetColor(OutlineColorID, outlineColor);
        mpb.SetFloat(OutlineThicknessID, outlineThickness);
        // targetRenderer.SetPropertyBlock(mpb, 1);
    }

    public void SpawnFromGround()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
        gameObject.SetActive(true);
        PlayAnim(standingHash);
    }

    public void CompleteSpawnAnim()
    {
        this.enabled = true;
        gameObject.layer = LayerMask.NameToLayer("enemy");
    }

    private bool switched = false;
    public bool fireZombie;
    public void SwitchEnemy()
    {
        if(switched) return;
        
        switched = true;
        fireZombie = true;
        headFireEffect.SetActive(true);
        Material[] mats = targetRenderers[0].sharedMaterials;
        mats[0] = burnMaterialB;
        mats[1] = burnMaterialA;
        targetRenderers[0].sharedMaterials = mats;
        targetRenderers[1].sharedMaterial = burnMaterialB;
        targetRenderers[2].sharedMaterial = burnMaterialA;
        //targetRenderer.sharedMaterial = burnMaterial;
        // targetRenderer.GetPropertyBlock(mpb,0);
        // mpb.SetFloat(floatPropertyName, 0.2f);
        // targetRenderer.SetPropertyBlock(mpb,0);
        switchFireEffect.Play();
    }
}
