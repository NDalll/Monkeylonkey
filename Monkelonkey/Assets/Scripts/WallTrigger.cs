using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTrigger : MonoBehaviour //dette er scriptet på collideren foran fjenden
{
    private Enemy entity;
    // Start is called before the first frame update
    void Start()
    {
        entity = transform.parent.GetComponent<Enemy>(); //reference til fjenden
    }
    private void OnTriggerEnter2D(Collider2D collision) //kaldes ved collision
    {
        if (collision.CompareTag("Ground")) //hvis det er jorden
        {
            entity.LedgeJump();//kalder fjendens ledgejump funktion
        }
    }
    private void OnTriggerExit2D(Collider2D collision)//kaldes når den forlader jorden
    {
        if (collision.CompareTag("Ground"))
        {
            entity.runSpeed = entity.ogRunSpeed; //sætter dens hastighed til det originale (bliver sat til 0 når den hopper)
            entity.walkSpeed = entity.ogWalkSpeed; //sætter dens hastighed til det originale (bliver sat til 0 når den hopper)
            entity.RB.velocity = new Vector2 (entity.RB.velocity.x, 0.2f);//stopper dens hastighed opad (derfor hopper den kun præcist så højt den har brug for)
        }
    }
}
