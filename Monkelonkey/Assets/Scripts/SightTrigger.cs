using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightTrigger : MonoBehaviour
{
    Enemy entity;

    private void Start()
    {
        entity = transform.parent.GetComponent<Enemy>();//reference til fjenden
    }

    private void OnTriggerEnter2D(Collider2D collision)//kaldes ved collision
    {
        if (collision.CompareTag("Player"))//hvis det er spilleren
        {
            entity.PlayerEntered();//kalder playerEntered i enemy scriptet
        }
    }
    private void OnTriggerExit2D(Collider2D collision) //kaldes når noget forlader collideren
    {
        if (collision.CompareTag("Player"))//hvis det er spilleren
        {
            entity.PlayerExited();//kalder playerExited i enemy scriptet
        }
    }
}
