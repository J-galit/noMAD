using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPlayerAttack : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DestroyCoroutine());
    }

    IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(0.15f);
        Destroy(this.gameObject);
    }
}
