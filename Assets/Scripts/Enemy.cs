using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int curHP;
    public int maxHP;
    public int damage;
    public float moveSpeed;
    public int xpToGive;

    [Header("Target")]
    public float chaseRange;
    public float attackRange;
    private Player player;

    [Header("Attack")]
    public float attackRate;
    private float lastAttackTime;


    //components
    private Rigidbody2D rig;

    void Awake()
    {
        //this will get the player target
        //this will try to find anything with the player script. used so that we can set the enemies target
        player = FindObjectOfType<Player>();

        rig = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float playerDist = Vector2.Distance(transform.position, player.transform.position);

        if(playerDist <= attackRange)
        {
            if(Time.time - lastAttackTime >= attackRate)
                Attack();

            rig.velocity = Vector2.zero;
        }
        else if(playerDist <= chaseRange)
        {
            Chase();
        }
        else
        {
            rig.velocity = Vector2.zero;
        }

    }

    void Attack()
    {
        lastAttackTime = Time.time;

        player.TakeDamage(damage);
    }

    void Chase()
    {
        Vector2 dir = (player.transform.position - transform.position).normalized;

        rig.velocity = dir * moveSpeed;
    }

    public void TakeDamage(int damageTaken)
    {
        curHP -= damageTaken;

        if(curHP <= 0)
            Die();
    }

    void Die()
    {
        player.AddXP(xpToGive);

        Destroy(gameObject);
    }
}
