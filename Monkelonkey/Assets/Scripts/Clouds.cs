using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    public Rigidbody2D RB;
    public float speed;
    private Vector3 originalPos;

    void Start()
    {
        originalPos = transform.position;
    }

    void FixedUpdate()
    {
        RB.velocity = new Vector2(speed, RB.velocity.y);
        if (Mathf.Abs(transform.position.x) > originalPos.x + 4.96 * transform.localScale.x) //4,96 er en værdi fundet igennem testing af hvor stort billedet er
        {
            transform.position = originalPos;
        }
    }
    
}
