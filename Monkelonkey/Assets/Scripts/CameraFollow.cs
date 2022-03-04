using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Transform camera;

    public float previousOffset;
    public Vector3 offset;
    public Rigidbody2D body;
    public float maxOffset;
    public float cameraSpeed;
    public float maxSpeed;

    private bool lerping;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        /*previousOffset = offset.x;
        if (Mathf.Abs(body.velocity.x) / cameraSpeed > maxOffset)
        {
            if (body.velocity.x < 0)
            {
                offset.x = maxOffset * -1;
            }
            else
            {
                offset.x = maxOffset;
            }
        }
        if (Mathf.Abs(previousOffset) - Mathf.Abs(offset.x) < maxSpeed || Mathf.Abs(previousOffset) - Mathf.Abs(offset.x) > maxSpeed)
        {
            lerping = true;
        }
        else
        {
            offset.x = body.velocity.x / cameraSpeed;
        }
        camera.position = new Vector3(player.position.x + offset.x, player.position.y + offset.y, -10);*/

    }
}
