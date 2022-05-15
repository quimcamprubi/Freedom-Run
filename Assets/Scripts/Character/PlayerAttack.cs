using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private float timeBtwAttack;

    public float startTimeBtwAttack = 0.4f;
    public Transform attackPos;
    public float attackRange;
    public float animationDelayAttack = 0.16f;
    public float animationDuration = 0.32f;
    public int damage = 1;
    public GameObject player;
    private Animator _animator;
    
    private void Start() {
        _animator = player.GetComponent<Animator>();
        timeBtwAttack = startTimeBtwAttack;
    }

    void Update() {GetComponent<Animator>();
        if (!player.GetComponent<PlayerController>().IsArmed()) {
            return;
        }
        if (timeBtwAttack <= 0.0f) {
            if (Input.GetKey(KeyCode.Return) && attackPos != null) {
                Debug.Log("Attack Done!");
                CancelInvoke(nameof(stopAttack));
                _animator.SetBool("isAttacking", true);
                Invoke(nameof(attackEnemies), animationDelayAttack);
                Invoke(nameof(stopAttack), animationDuration);
                timeBtwAttack = startTimeBtwAttack;
            }
        } else {
            timeBtwAttack -= Time.deltaTime;
            Debug.Log(timeBtwAttack);
        }
    }

    private void stopAttack() {
        _animator.SetBool("isAttacking", false);
    }

    private void attackEnemies() {
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, LayerMask.GetMask("Enemy"));
        if (enemiesToDamage.Length == 0) {
            return;
        }
        foreach (var t in enemiesToDamage) {
            if (t.GetType() != typeof(BoxCollider2D))  {
                Debug.Log("Enemy Hurt!");
                t.GetComponent<AIPatrol>().takeDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}
