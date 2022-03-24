using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    [System.NonSerialized]
    public float damage;
    private Player entity;

    private void Start()
    {
        entity = GameObject.FindWithTag("Player").GetComponent<Player>();
    }
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            entity.dealDamage(damage);
        }
    }

    
}
