using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogEnemyScript : MonoBehaviour
{
    private GameObject Player;
    private Animator anim;
    private Rigidbody2D rb;
    private bool PlayerDetected;
    public float maxPatrolingDistance;
    private Vector2 defaultPos;
    public Transform groundCheck;
    public LayerMask WhatIsGround;

    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        defaultPos = transform.position;
    }

    void Update()
    {
       var PlayerXDistance = Vector2.Distance(new Vector2(transform.position.x, 0), new Vector2(Player.transform.position.x, 0));
       var PlayerYDistance = Vector2.Distance(new Vector2(0,transform.position.y), new Vector2(0,Player.transform.position.y));

        PlayerDetected = (PlayerXDistance < 15 && PlayerYDistance < 2) ? true: false;  

        if (PlayerDetected) TriggerState();
        else PatrolingState();

    }

    void PatrolingState()
    {
        if(IsGrounded())
        {

        }
    }

    
    void TriggerState()
    {
        int xScale = (Player.transform.position.x > transform.position.x) ? -1 : 1;
        transform.localScale = new Vector2(xScale, transform.localScale.y);
    }

    bool IsGrounded()
    {

        if (!Physics2D.CircleCast(groundCheck.position, 0.2f, Vector2.down, 0.1f, WhatIsGround)) return false;
        else return true;
    }


}
