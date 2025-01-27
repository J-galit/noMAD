using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugEnemyDeathDrop : MonoBehaviour
{
    [SerializeField] private GameObject currencyDrop;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Attack"))
        {
            Instantiate(currencyDrop, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
