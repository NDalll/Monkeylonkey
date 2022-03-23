using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called before the first frame update
    [System.NonSerialized]
    public float initProjSpeed;
    [System.NonSerialized]
    public float projAccelation;
    [System.NonSerialized]
    public bool isHoming;
    [System.NonSerialized]
    public int damage;



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().dealDamage(damage);
        }
    }

    private void Death()
    {
        GameObject.Destroy(gameObject);
    }
}
