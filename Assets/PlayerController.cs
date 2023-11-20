using NUnit.Framework.Constraints;
using System;
using UnityEngine;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    public float Speed = 10f;
    private Animator anim;
    Rigidbody2D rb;
    Quaternion rotation;
    public float JumpForce; 
    private int AttackCount;
    public float AttackDmg;
    bool CanAttack;
    [SerializeField] private Transform GroundCheck;
    public LayerMask WhatIsGround;
    [SerializeField] private Transform AttackPoint;
    public float AttackRange;
    public LayerMask EnemyLayer;
    float BonusDamage = 0;
    float DynamicAttackRange = 0;
    public float DashForce;
    public bool isDashing = false;

    void Start()
    {
        rotation = transform.rotation;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        AttackCount = 0;
        CanAttack = true;
    }

    void Update()
    {
        IsGrounded();
        AttackCooldown();

        if (!isDashing)
        {
            Move(Speed);
            Attack(15f);
            Jump();
        }
        else { rb.velocity = new Vector3(rb.velocity.x, 0, 0); }

        Dash();
        DetectWalls();
        if ( transform.localScale.x > 1) AttackPoint.localPosition = new Vector2(Mathf.Abs(AttackPoint.localPosition.x) * -1, AttackPoint.localPosition.y);
        else AttackPoint.localPosition = new Vector2(Mathf.Abs(AttackPoint.localPosition.x), AttackPoint.localPosition.y);
       
    }

    public ParticleSystem WalkingParticles;

    private void Move(float speed)
    {
        float x = Input.GetAxis("Horizontal");
        float Y = Input.GetAxis("Vertical");


        //transform.position = new Vector3(transform.position.x + speed/5 * x * Time.deltaTime, transform.position.y, transform.position.z);
        var xScale = (x < -0.001f) ? -1 : (x > 0) ? 1: transform.localScale.x;
        transform.localScale = new Vector3(xScale, transform.localScale.y, transform.localScale.z);

        transform.rotation = rotation;

        if (rb.velocity.magnitude < 80)
        {
            if (!IsGrounded()) x /= 1.2f;
            Vector3 targetVel = new Vector3(speed * x * Time.fixedDeltaTime, rb.velocity.y, 0);
            rb.velocity = targetVel;
        }

        if (isJumping) return;

        anim.SetFloat("Walk",Mathf.Abs(x));
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) anim.SetBool("Move", true);
        else anim.SetBool("Move", false);

    }

    public void PlayStepParticles()
    {
        Instantiate(WalkingParticles, GroundCheck.position, GroundCheck.rotation);
    }


    void Attack(float StaminaValue)
    {
        if(StaminaManagerScript.Instance.currentStamina < StaminaValue) return;

        if(Input.GetMouseButtonDown(0) && CanAttack)
        {
            if(AttackCount >= 3) AttackCount = 0;
          
            switch(AttackCount)
            {
                case 0: 
                    anim.SetTrigger("Attack1");
                    BonusDamage = 0;
                    DynamicAttackRange = 0.05f;
                break;

                case 1: 
                    anim.SetTrigger("Attack2");
                    BonusDamage = 5f;
                    DynamicAttackRange = -0.05f;
                break;

                case 2: 
                    anim.SetTrigger("Attack3");
                    BonusDamage = 20f;
                    DynamicAttackRange = 0.1f;
                break;    
            }

            CanAttack=false;
            StaminaManagerScript.Instance.SubtractStamina(StaminaValue);       
        }
    }


    void HitEnemy(float dmg, float range)
    {
        var hitEnemies = Physics2D.OverlapCircleAll(AttackPoint.position, range, EnemyLayer);

        foreach(var enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyHp>().ReciveDamage(dmg);
            BloodScript.Instance.InvokeBlood(enemy.transform);
        }
    }

    private float jumpTimeCounter;
    public float jumpTime;
    bool isJumping;


    public ParticleSystem DashParticles;

    void Dash()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isDashing = true;
            anim.SetTrigger("Dash");
            int X = (int)transform.localScale.x;
            CameraShake();
            //rb.velocity = new Vector3(DashForce * X, 0 , 0);
            rb.velocity = Vector2.zero;
            Vector3 Target = new Vector3(DashForce * X * Time.fixedDeltaTime, 0, 0);
            //rb.velocity = Target; 
            if (IsGrounded()) Target *= 1.4f;
            rb.AddForce(Target, ForceMode2D.Impulse);
            DashParticles.transform.localScale = new Vector2(-X, 1);
            DashParticles.Play();
        }
    }

    public ParticleSystem JumpParticle;

    void Jump()
    {
        Vector2 JumpVector = new (0, 1);

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded()) 
        {
            anim.SetBool("Move", false);
            anim.SetTrigger("Jump");
            isJumping = true;
            jumpTimeCounter = jumpTime;
            PlayFallParticles();
            rb.AddForce(JumpVector * JumpForce * Time.fixedDeltaTime, ForceMode2D.Impulse);
        }

        if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.AddForce(JumpVector * JumpForce * 2 * Time.fixedDeltaTime);
                jumpTimeCounter -= Time.fixedDeltaTime;
            }
            else isJumping = false;
        }

        if (Input.GetKeyUp(KeyCode.Space)) isJumping = false;
    }

    public void CameraShake()
    {
        CinemaMachineShake.Instance.ShakeCamera(1.4f,0.15f);

        //rb.AddForce(Vector2.right * X * 140 * Time.fixedDeltaTime, ForceMode2D.Impulse);
    }

    public void AttackHit()
    {
        CameraShake();
        int X = (int)transform.localScale.x;
        rb.AddForce(Vector2.right * X * 500 * Time.fixedDeltaTime, ForceMode2D.Impulse);
        HitEnemy(AttackDmg + BonusDamage, AttackRange + DynamicAttackRange);
    }

    public void AttackCountIncrement()
    {
        AttackCount++;
    }


    private float AttackTimer = 1.6f;

    public void AttackCooldown()
    {
        if (CanAttack) return;

        if(AttackTimer > 0) AttackTimer -= Time.fixedDeltaTime;
        else
        {
            CanAttack = true;
            AttackTimer = 1.6f;
        }
    }

    public void PlayFallParticles()
    {
        Instantiate(JumpParticle, GroundCheck.position, GroundCheck.rotation);
    }

    bool IsGrounded()
    {
        if (!Physics2D.CircleCast(GroundCheck.position, 0.2f, Vector2.down, 0.1f, WhatIsGround))
        {

            anim.SetBool("isGrounded", false);
            return false;
        }
        else
        {
            anim.SetBool("isGrounded", true);
            return true;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(AttackPoint.position, AttackRange + DynamicAttackRange);
        Gizmos.DrawSphere(GroundCheck.position, 0.1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Vector2.left);
        Gizmos.DrawCube(new Vector2(transform.position.x, transform.position.y - 0.05f), new Vector2(0.5f, 0.45f));

    }

    void DetectWalls()
    {
        int X = (int)transform.localScale.x;

        //if (Physics2D.Raycast(transform.position, new Vector2(X,0), 0.25f, WhatIsGround)) { rb.velocityX = 0; Debug.Log("Wall detected"); }
        if (Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y - 0.05f),new Vector2(0.20f, 0.45f),0, new Vector2(X,0), 0.1f, WhatIsGround)) { rb.velocityX = 0; Debug.Log("Wall detected"); }
    }


    public void ResetDash() { isDashing = false; }

    public Animator DarkBackgroundAnim;
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Doors") DarkBackgroundAnim.SetBool("Inside", true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Doors") DarkBackgroundAnim.SetBool("Inside", false);
    }
}
