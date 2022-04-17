using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour //dette script er ansvarlig for at styre kameraets bev�gelse
{
    public Transform player; //defineres i inspektoren
    public Transform cam; //defineres i inspektoren
    public Vector3 offset; //defineres i inspektoren
    public float maxOffset; //defineres i inspektoren
    public float cameraSpeed; //defineres i inspektoren
    private PlayerControls playerControls;
    public float cameraDeceleration; //defineres i inspektoren
    private Player playerscript;

    private void Start() //kaldes p� den f�rste frame
    {
        playerscript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>(); //finder playeren
    }
    void Update()
    {
        Vector2 input = playerControls.Default.Move.ReadValue<Vector2>(); //l�ser om spilleren inputter mod h�jre eller venstre
        if (input.x > 0) //hvis det er mod h�jre
        {
            if (offset.x < maxOffset) //hvis offsettet ikke har passeret maxet
            {
                if (offset.x < 0) //hvis offsettet er i den modsatte retning bruger vi deceleration
                {
                    offset.x += Time.deltaTime * cameraDeceleration;
                }
                else //ellers er bruges speed istedet
                {
                    offset.x += Time.deltaTime * cameraSpeed;
                }
            }
            else //hvis offsettet er over maximummet bliver det sat til maximummet 
            {
                offset.x = maxOffset;
            }
        } 
        else if (input.x < 0) //hvis det er mod venstre
        {
            if (offset.x > maxOffset * -1) //hvis offsettet ikke har passeret maxet i den negative retning
            {
                if (offset.x > 0) //hvis offsettet er i den modsatte retning bruger vi deceleration
                {
                    offset.x -= Time.deltaTime * cameraDeceleration;
                }
                else //ellers er bruges speed istedet
                {
                    offset.x -= Time.deltaTime * cameraSpeed;
                }
                
            }
            else //hvis offsettet er over maximummet i den modsatte retning bliver det sat til maximummet i den negative retning 
            {
                offset.x = maxOffset * -1;
            }
        }
        else //hvis ikke der er et input bliver kameraet sendt mod centrum
        {
            if (offset.x > 0.05)
            {
                offset.x -= Time.deltaTime * cameraDeceleration;
            }
            else if (offset.x < -0.05)
            {
                offset.x += Time.deltaTime * cameraDeceleration;
            }
            else //hvis den er t�t nok p� midten bliver den sat til midten
            {
                offset.x = 0;
            }
        }
        cam.position = new Vector3(player.position.x + offset.x, player.position.y + offset.y, -10); //s�tter positionen

        if (playerscript.isDead) //hvis spilleren er d�d disables inputsystemet
        {
            playerControls.Disable();
        }
    }
    private void Awake() //bliver kaldt f�r den f�rste frame hvor objectet er aktiv
    {
        playerControls = new PlayerControls(); //definere inputcontrolleren
    }
    private void OnEnable() //bliver kaldt n�r den bliver enablelet
    {
        playerControls.Enable(); //enabler inputcontrolleren
    }
    private void OnDisable() //bliver kaldt n�r den bliver disablelet
    {
        playerControls.Disable(); //disabler inputcontrolleren
    }
}
