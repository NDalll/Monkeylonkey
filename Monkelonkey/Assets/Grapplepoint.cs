using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapplepoint : MonoBehaviour
{
    public bool isThrown;
    public Rigidbody2D body;
    private void OnTriggerEnter2D(Collider2D collision)
    {
       if (collision.CompareTag("Ground"))
        {
            if (isThrown)
            {
                body.velocity = Vector3.zero;
            }
            isThrown = false;
        }
    }
}
