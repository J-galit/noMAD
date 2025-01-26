using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorEnemyHealth : MonoBehaviour
{
    public int health;
    [SerializeField] GameObject[] healthDisplayArray;

    private bool isIFramesActive;

    private void Awake()
    {
        isIFramesActive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Attack"))
        {
            if(health > 0 && isIFramesActive == false)
            {
                healthDisplayArray[health-1].gameObject.SetActive(false);
                health--;
                isIFramesActive = true;
                StartCoroutine(InvincibilityCoroutine());
            }
            else if(health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator InvincibilityCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        isIFramesActive = false;
    }
}
