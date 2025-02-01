using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    public int maxHealth;
    private bool isInCombat, isCoroutineActive;
    [SerializeField] GameObject[] healthDisplayArray;
    private ThirdPersonCharacterController thirdPersonCharacterController;
    private void Awake()
    {
        thirdPersonCharacterController = GetComponent<ThirdPersonCharacterController>();
        isCoroutineActive = false;
    }

    private void Start()
    {
        HealthCheck(health);
       /* for (int i = 0; i < health + 1; i++)
        {
            healthDisplayArray[i].gameObject.SetActive(true);
        }
        maxHealth = health; */
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
                if (isCoroutineActive == false)
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
        yield return new WaitForSeconds(5f / thirdPersonCharacterController.healingSpeedMultiplier);
        isCoroutineActive = false;
        isInCombat = false;
        if (isInCombat == false)
        {
            if (health < maxHealth)
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

    public void HealthCheck(int newHealth)
    {
        maxHealth += newHealth;
        health = maxHealth;
        for (int i = 0; i < healthDisplayArray.Length; i++)
        {
            healthDisplayArray[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < health + 1; i++)
        {
            healthDisplayArray[i].gameObject.SetActive(true);
        }
        //maxHealth = newHealth;
    }
}
