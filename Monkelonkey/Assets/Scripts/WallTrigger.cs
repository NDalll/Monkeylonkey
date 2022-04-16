using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTrigger : MonoBehaviour
{
    private Enemy entity;
    // Start is called before the first frame update
    void Start()
    {
        entity = transform.parent.GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            entity.LedgeJump();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            entity.runSpeed = entity.ogRunSpeed;
            entity.walkSpeed = entity.ogWalkSpeed;
            entity.RB.velocity = new Vector2 (entity.RB.velocity.x, 0.2f);
        }
    }
}
