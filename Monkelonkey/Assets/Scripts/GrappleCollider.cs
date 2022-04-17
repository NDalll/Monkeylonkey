using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleCollider : MonoBehaviour
{
    private bool isStopped;
    private Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();//definere player objecket som f�es fra player gameobjektet
    }
    void Update()
    {
        transform.position = player.transform.position;//opdatere positonen p� grappelcollideren til at v�re den samme som playeren hvert frame
    }
    private void OnTriggerEnter2D(Collider2D collision)//funktion fra unitys monobehavior, der aktivere n�r en colider enter en trigger colider
    {
        if (collision.CompareTag("grapple"))//hvis at det er et grapplepoint der enter
        {
            player.nearGPoints.Add(collision.gameObject);//tilf�je den til en liste p� playeren der har er alle de grapplepoint der er t�t p�
        }
    }

    private void OnTriggerExit2D(Collider2D collision)//funktion fra unitys monobehavior, der aktivere n�r en colider exiter en trigger colider
    {
        if (collision.CompareTag("grapple"))//hvis det er grapplepoint der exiter
        {
            player.nearGPoints.RemoveAll(i => i.gameObject == collision.gameObject);//fjern det gameobject(grapple point) i listen, som er ens med det grapplepoint der exited collideren
        }
    }
}
