using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    private bool isInCombat, isCoroutineActive;
    [SerializeField] GameObject[] healthDisplayArray;
    private void Awake()
    {
        isCoroutineActive = false;
    }

    private void Update()
    {
        Debug.Log("player health: " + health);
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("EnemyAttack"))
        {
            if (health > 0)
            {
                isInCombat = true;
                healthDisplayArray[health].gameObject.SetActive(false);
                health--;
                if(isCoroutineActive == false)
                {
                    StartCoroutine(OutOfCombatCoroutine());
                }
            }
            else if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator OutOfCombatCoroutine()
    {
        isCoroutineActive = true;
        yield return new WaitForSeconds(5f);
        isCoroutineActive = false;
        isInCombat = false;
        if (isInCombat == false)
        {
            if (health < healthDisplayArray.Length - 1)
            {
                health++;
                healthDisplayArray[health].gameObject.SetActive(true);
            }
            if (health == healthDisplayArray.Length - 1)
            {
                yield break;
            }
            else
            {
                StartCoroutine(OutOfCombatCoroutine()); 
            }
        }
        else
        {
            yield break;
        }
    }
}
