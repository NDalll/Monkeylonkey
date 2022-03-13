using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Transform cam;
    public Vector3 offset;
    public float maxOffset;
    public float cameraSpeed;
    private PlayerControls playerControls;
    public float cameraDeceleration;

    // Update is called once per frame
    void Update()
    {
        Vector2 input = playerControls.Default.Move.ReadValue<Vector2>();
        if (input.x > 0)
        {
            if (offset.x < maxOffset)
            {
                if (offset.x < 0)
                {
                    offset.x += Time.deltaTime * cameraDeceleration;
                }
                else
                {
                    offset.x += Time.deltaTime * cameraSpeed;
                }
            }
            else
            {
                offset.x = maxOffset;
            }
        } 
        else if (input.x < 0)
        {
            if (offset.x > maxOffset * -1)
            {
                if (offset.x > 0)
                {
                    offset.x -= Time.deltaTime * cameraDeceleration;
                }
                else
                {
                    offset.x -= Time.deltaTime * cameraSpeed;
                }
                
            }
            else
            {
                offset.x = maxOffset * -1;
            }
        }
        else
        {
            if (offset.x > 0.05)
            {
                offset.x -= Time.deltaTime * cameraDeceleration;
            }
            else if (offset.x < -0.05)
            {
                offset.x += Time.deltaTime * cameraDeceleration;
            }
            else
            {
                offset.x = 0;
            }
        }
        cam.position = new Vector3(player.position.x + offset.x, player.position.y + offset.y, -10);
    }
    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }
}
