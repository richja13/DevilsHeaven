using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class DogEnemyScript : MonoBehaviour
{
    private GameObject Player;
    public float speed = 15;
    private Animator anim;
    private Rigidbody2D rb;
    private bool PlayerDetected;
    public float maxPatrolingDistance;
    private Vector2 defaultPos;
    public Transform groundCheck;
    public LayerMask WhatIsGround;
    public bool Hit;
    bool CanWalk = true;
    public bool CanTurn = false;
    public float Damage;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        Player = GameObject.FindWithTag("Player");
        defaultPos = transform.position;
    }

    void Update()
    {
        CheckHitStatus();

        if (Hit) return;
        var PlayerXDistance = Vector2.Distance(new Vector2(transform.position.x, 0), new Vector2(Player.transform.position.x, 0));
        var PlayerYDistance = Vector2.Distance(new Vector2(0, transform.position.y), new Vector2(0, Player.transform.position.y));

        PlayerDetected = (PlayerXDistance < 5 && PlayerYDistance < 0.2f) ? true : false;

        if (PlayerDetected) TriggerState();
        else PatrolingState();
    }

    void PatrolingState()
    {
        if (!CanTurn)
        {
            if (IsGrounded() && !DetectWalls())
            {
                Walk();
            }
        }
        else
        {
            anim.SetBool("Walking", false);
            CanWalk = false;
            //ChangeDirection();
            StartCoroutine("ChangeDirection");
        }
        anim.SetBool("Running", false);
    }

    public void Walk()
    {
        anim.SetBool("Walking", true);
        var xScale = transform.localScale.x;
        Vector3 targetVel = new Vector3(speed * -xScale * Time.fixedDeltaTime, rb.velocity.y, 0);
        if (Hit) return;
        rb.velocity = targetVel;
    }

    void TriggerState()
    {
        int xScale = (Player.transform.position.x > transform.position.x) ? -1 : 1;
        transform.localScale = new Vector2(xScale, transform.localScale.y);
        anim.SetBool("Walking", false);
        anim.SetBool("Running", true);
        Vector3 targetVel = new Vector3(speed * 3.5f * -xScale * Time.fixedDeltaTime, rb.velocity.y, 0);
        if (Hit) return;
        rb.velocity = targetVel;
    }

    bool DetectWalls()
    {
        int X = (int)transform.localScale.x;
        // if (Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y), new Vector2(0.20f, 0.15f), 0, new Vector2(X, 0), 0.2f, WhatIsGround))
        if (Physics2D.Raycast(transform.position, Vector2.left * X, 0.5f, WhatIsGround))
        { CanTurn = true; return true; }
        else return false;
    }

    private void OnDrawGizmos()
    {
        int X = (int)transform.localScale.x;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector2.left * X);
    }

    IEnumerator ChangeDirection()
    {
        yield return new WaitForSeconds(2f);
        if (CanTurn)
        {
            CanTurn = false;
            Debug.Log("before");
            Debug.Log("after");
            var currentXscale = transform.localScale.x;
            var x = (currentXscale == 1) ? -1 : 1;
            transform.localScale = new Vector2(x, transform.localScale.y);
            CanWalk = true;
        }
    }

    bool IsGrounded()
    {
        if (!Physics2D.Raycast(groundCheck.position, Vector2.down, 0.25f, WhatIsGround))
        { CanTurn = true; return false; }
        else return true;
    }

    public void CheckHitStatus()
    {
        Hit = (anim.GetCurrentAnimatorStateInfo(0).IsName("Hit")) ? true : false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        PlayerHealthController.Instance.ReciveDamage(Damage);
    }
}