using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    private bool isInCombat, isCoroutineActive;
    [SerializeField] GameObject[] healthDisplayArray;
    private ThirdPersonCharacterController thirdPersonCharacterController;
    private void Awake()
    {
        thirdPersonCharacterController = GetComponent<ThirdPersonCharacterController>();
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
            Debug.Log("getting hit");
            if (health > 0)
            {
                Debug.Log("still getting hit");
                isInCombat = true;
                healthDisplayArray[health].gameObject.SetActive(false);
                health--;
                Debug.Log("still getting hit after health --");
                if (isCoroutineActive == false)
                {
                    Debug.Log("still getting hit");
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
