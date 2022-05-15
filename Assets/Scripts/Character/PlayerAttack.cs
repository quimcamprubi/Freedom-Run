using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private float timeBtwAttack;

    public float startTimeBtwAttack = 0.3f;
    public Transform attackPos;
    public float attackRange;

    public int damage = 1;

    private void Update()
    {
        if (timeBtwAttack <= 0)
        {
            if (Input.GetKey(KeyCode.Return) && attackPos != null)
            {
                Debug.Log("Attack Done!");
                var enemiesToDamage =
                    Physics2D.OverlapCircleAll(attackPos.position, attackRange, LayerMask.GetMask("Enemy"));
                if (enemiesToDamage != null)
                {
                    for (var i = 0; i < enemiesToDamage.Length; i++)
                        if (enemiesToDamage[i].GetType() != typeof(BoxCollider2D))
                        {
                            Debug.Log("Enemy Hurt!");
                            enemiesToDamage[i].GetComponent<AIPatrol>().takeDamage(damage);
                        }

                    timeBtwAttack = startTimeBtwAttack;
                }
            }
        }
        else
        {
            timeBtwAttack -= Time.deltaTime;
            Debug.Log(timeBtwAttack);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }

    // Update is called once per frame
}