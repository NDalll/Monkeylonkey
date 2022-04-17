using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clouds : MonoBehaviour //dette script er ansvarlig for bagrunden
{
    public Rigidbody2D RB;
    public float speed;
    private Vector3 originalPos;

    void Start()
    {
        originalPos = transform.position; //gemmer den originale position
    }

    void FixedUpdate() //kaldes i en fixed framerate så at det ikke går hurtigere på en hurtig framerate
    {
        RB.velocity = new Vector2(speed, RB.velocity.y); //bævæger skyerne til siden
        if (Mathf.Abs(transform.position.x) > originalPos.x + 4.96 * transform.localScale.x) // Tjekker om de har passeret//4,96 er en værdi fundet igennem testing af hvor stort billedet er
        {
            transform.position = originalPos; //sætter dem tilbage til starten
        }
    }
    
}
