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
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();//definere player objecket som fåes fra player gameobjektet
    }
    void Update()
    {
        transform.position = player.transform.position;//opdatere positonen på grappelcollideren til at være den samme som playeren hvert frame
    }
    private void OnTriggerEnter2D(Collider2D collision)//funktion fra unitys monobehavior, der aktivere når en colider enter en trigger colider
    {
        if (collision.CompareTag("grapple"))//hvis at det er et grapplepoint der enter
        {
            player.nearGPoints.Add(collision.gameObject);//tilføje den til en liste på playeren der har er alle de grapplepoint der er tæt på
        }
    }

    private void OnTriggerExit2D(Collider2D collision)//funktion fra unitys monobehavior, der aktivere når en colider exiter en trigger colider
    {
        if (collision.CompareTag("grapple"))//hvis det er grapplepoint der exiter
        {
            player.nearGPoints.RemoveAll(i => i.gameObject == collision.gameObject);//fjern det gameobject(grapple point) i listen, som er ens med det grapplepoint der exited collideren
        }
    }
}
