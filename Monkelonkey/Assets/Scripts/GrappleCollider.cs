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
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    void Update()
    {
        transform.position = player.transform.position;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ThrownGrapple")){
            if (collision.GetComponent<Grapplepoint>().isThrown == false)
            {
                    player.nearGPoints.Add(collision.gameObject);
            }
        }
        if (collision.CompareTag("grapple"))
        {
            player.nearGPoints.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("grapple"))
        {
            foreach (GameObject x in player.nearGPoints)
            {
                if (x.transform.position.x == collision. transform.position.x && x.transform.position.y == collision.transform.position.y)
                {
                    player.nearGPoints.Remove(x);
                }
            }
        }
        if (collision.CompareTag("ThrownGrapple"))
        {
            if (collision.GetComponent<Grapplepoint>().isThrown == false)
            {
                foreach (GameObject x in player.nearGPoints)
                {
                    if (x.transform.position.x == collision.transform.position.x && x.transform.position.y == collision.transform.position.y)
                    {
                        player.nearGPoints.Remove(x);
                    }
                }
            }
        }
    }
}
