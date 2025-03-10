using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehindInFront : MonoBehaviour
{
    [SerializeField] private Transform playerTransform, enemyTransform;

    // Update is called once per frame
    void Update()
    {
        Vector3 playerForward = playerTransform.forward;
        Vector3 playerToEnemy = enemyTransform.position - playerTransform.position;

        if (MathLibrary.DotProduct(playerForward, playerToEnemy) < 0f)
        {
            Debug.Log("BEHIND");
        }
        else
        {
            Debug.Log("FRONT");
        }
    }
}
