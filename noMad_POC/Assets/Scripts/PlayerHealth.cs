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
        //need this script to access newMaxHealth variable and isResistAttack bool
        thirdPersonCharacterController = GetComponent<ThirdPersonCharacterController>();
        isCoroutineActive = false;
    }

    private void Start()
    {
        HealthCheck(health);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if hit by an enemy attack
        if (collision.gameObject.CompareTag("EnemyAttack"))
        {

            if (health > 0)
            {
                isInCombat = true;
                //remove heart UI
                healthDisplayArray[health].SetActive(false);
                if(thirdPersonCharacterController.isResistAttackActive == false) //if they don't have resist attack
                {
                    healthDisplayArray[health - 1].SetActive(false); //take extra damage
                    health-= 2;
                }
                else
                {
                    health--;
                }

                if(health<=0) //if player runs out of health
                {
                    Destroy(gameObject); //dies
                }

                if (isCoroutineActive == false)
                {
                    //start coroutine that will slowly heal the player
                    StartCoroutine(OutOfCombatCoroutine());
                }
            }
        }
    }

    IEnumerator OutOfCombatCoroutine()
    {
        isCoroutineActive = true;
        yield return new WaitForSeconds(5f / thirdPersonCharacterController.healingSpeedMultiplier);
        isCoroutineActive = false;
        isInCombat = false;
        //checks if the player is out of combat
        if (isInCombat == false)
        {
            //if health is less than max
            if (health < maxHealth)
            {
                //health increase
                health++;
                //heart displayed in UI
                healthDisplayArray[health].SetActive(true);
            }
            if (health == healthDisplayArray.Length - 1) //if health equals the array length
            {
                yield break; //stop the coroutine
            }
            else
            {
                //restart coroutine if player still has health
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
        //newHealth is a temporary variable that gets changed by whatever float is sent to this method
        maxHealth += newHealth; //adds that health to the maxHealth
        health = maxHealth; //and sets health equal to maxHealth

        //this removes all hearts from the UI, so we can setup the appropriate amount to display in the next loop
        for (int i = 0; i < healthDisplayArray.Length; i++)
        {
            healthDisplayArray[i].gameObject.SetActive(false);
        }

        //displays the amount of hearts equal to the amount of health the player has.
        for (int i = 0; i < health + 1; i++)
        {
            healthDisplayArray[i].gameObject.SetActive(true);
        }
    }
}
