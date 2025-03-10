using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 2f, turnSpeed = 45f, flySpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W)) 
        {
            transform.localPosition += transform.forward * speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.localPosition -= transform.forward * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(transform.up, Time.deltaTime * -turnSpeed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(transform.up, Time.deltaTime * turnSpeed);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(transform.right, Time.deltaTime * -turnSpeed);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(transform.right, Time.deltaTime * turnSpeed);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            transform.localPosition += transform.up * flySpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.localPosition -= transform.up * flySpeed * Time.deltaTime;
        }
    }
}
