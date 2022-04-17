using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour //sciptet ansvarlig for stationære forhindringer såsom pikkende
{
    public float damage;
    private Player player;
    // Start is called before the first frame update
    private void OnTriggerStay2D(Collider2D collision) //kaldes når noget går ind i collideren
    {
        if (collision.CompareTag("Player")) //checker om denne collision var spilleren
        {
            player = collision.GetComponent<Player>(); //reference til player scriptet
            player.dealDamage(damage); //skader spilleren
        }
    }
}
