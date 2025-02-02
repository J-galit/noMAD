using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEnemyAttack : MonoBehaviour
{
    public AudioSource enemyAttackSound;
    void Start()
    {
        StartCoroutine(DestroyCoroutine());
    }

    IEnumerator DestroyCoroutine()
    {
        enemyAttackSound.Play();
        yield return new WaitForSeconds(0.15f);
        Destroy(this.gameObject);
    }
}
