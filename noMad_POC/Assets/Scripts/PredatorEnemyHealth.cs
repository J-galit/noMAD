using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorEnemyHealth : MonoBehaviour
{
    public int health;
    [SerializeField] GameObject[] healthDisplayArray;

    private bool isIFramesActive;

    private ThirdPersonCharacterController thirdPersonCharacterController;


    private void Awake()
    {
        //need this to access isMoreDamageActive bool
        thirdPersonCharacterController = GameObject.Find("Player").GetComponent<ThirdPersonCharacterController>();

        isIFramesActive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Attack"))
        {
            if (health > 0 && isIFramesActive == false)
            {
                //remove a health signifier
                healthDisplayArray[health - 1].SetActive(false);
                health--; //lose health
                if (thirdPersonCharacterController.isMoreDamageActive == true) //if more damage adaptation is active
                {
                    healthDisplayArray[health - 1].SetActive(false); //lose extra health
                    health--;
                    isIFramesActive = true;
                }

                //give invincibility
                isIFramesActive = true;
                StartCoroutine(InvincibilityCoroutine());
                //dies when health drops to 0 or below
                if (health <= 0)
                {
                    Destroy(gameObject);
                }
            }
            else if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator InvincibilityCoroutine()
    {
        //prevents enemy getting hit twice from entering triggers more than once
        yield return new WaitForSeconds(0.2f);
        isIFramesActive = false;
    }
}
