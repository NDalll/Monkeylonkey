using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapplepoint : MonoBehaviour//dette script er for at indecificere et grapplepoint som en trowable grapple eller normal grappe 
{
    public bool isThrown;
    public Rigidbody2D body;
    public bool trowable;
    private void OnTriggerEnter2D(Collider2D collision)
    {
       if (collision.CompareTag("Ground"))//hvis at et grapplepunkt collidere med jorden
        {
            if (isThrown)//hvis det er et kastede grapple punkt
            {
                body.velocity = Vector3.zero;//stop alt bevægelse på grapplepunktet
            }
            isThrown = false;//den er ikke længere kastede
        }
    }
}
