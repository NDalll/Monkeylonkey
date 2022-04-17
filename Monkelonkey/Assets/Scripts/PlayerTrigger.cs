using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour //dette er scriptet p� fjendens collider til at skade spilleren
{
    [System.NonSerialized]
    public float damage;
    private Player entity;

    private void Start() //kaldes p� den f�rste frame
    {
        entity = GameObject.FindWithTag("Player").GetComponent<Player>();//finder spilleren
    }
    private void OnTriggerEnter2D(Collider2D collision) //kaldes n�r et objekt kommer ind i collideren
    {
        if (collision.CompareTag("Player")) //hvis det er spilleren skaden de
        {
            entity.dealDamage(damage);
        }
    }
}
