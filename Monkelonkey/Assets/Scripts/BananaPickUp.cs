using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BananaPickUp : MonoBehaviour//obs navnet er lidt misleading, da dette script ogs� holder styr p� en kastede bananan da de bruger samme prefab
{
    // Start is called before the first frame update
    private Vector3 startPostion;
    private Vector3 endPostion;
    private float fraction = 0;
    public float floatingDis;
    public float speed;
    [System.NonSerialized]
    public bool isPickUp;
    public float rotateSpeed;
    public bool iFrames = true;
    public bool isFinalBanana;
    private Gamecontroller gamecontroller;
    public Player player;
    void Awake()//ved awake s�tter den ispickup er lig med true, bliver �ndre fra playerscipet lige efter hvis bananen er kastet
    {
        isPickUp = true;
    }
    private void Start()
    {
        //s�tter start postionen for pickup b�v�gelse op og ned
        startPostion = transform.position;
        endPostion = transform.position + Vector3.up * floatingDis;
        Invoke("DisableIFrames", 0.05f);//som standard kan den ikke collidere, med det bliver enablet 0.05 sekunder efter, dette er fordi at bananen kan g� igennem kanten hvis s� det virker bedste for spilleren
    }

    // Update is called once per frame
    void Update()
    {
        if (isPickUp)//hvis bannanen er en pickup
        {
            if (fraction < 1)//hvis at lerpen ikke er f�rdig endu(lerp kan smooth bev�ge et gameobeject fra et punkt til et andet)
            {
                fraction += Time.deltaTime * speed;//vi bev�ger os lidt frem i lerpen  
                transform.position = Vector3.Lerp(startPostion, endPostion, fraction); //vi opdatere lerpen 
            }
            if (fraction >= 1)//hvis lerpen er f�rdig
            {
                //bytter rundt p� start og slut positionen og s�tter lerpen til at starte forfra
                Vector3 mPos = startPostion;
                startPostion = endPostion;
                endPostion = mPos;
                fraction = 0;
            }
        }
        else
        {
            //hvis den er kastedet skal den rotere lidt hvertframe
            gameObject.transform.Rotate(new Vector3(0, 0, Time.deltaTime*rotateSpeed));
        }
        
    }
    private void DisableIFrames()//sl�r iframes fra s� den kan collidere
    {
        iFrames = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPickUp)//hvis det er et pickup
        {
            if (collision.CompareTag("Player"))//hvis det collidere med en spiller
            {
                if (isFinalBanana)//hvis det er en finalbanana(den banan der er p� toppen af tr�et, du gennemf�re rundet ved at tage)
                {
                    gamecontroller = GameObject.FindGameObjectWithTag("Gamecontroller").GetComponent<Gamecontroller>();//laver reffernce til gamecontroller objectet
                    player = collision.gameObject.GetComponent<Player>();//definere player obejcktet
                    //tilf�jer til scoresne
                    ScoreManager.bananaScore++;
                    ScoreManager.totalBananas++;
                    ScoreManager.stagesCleared++;

                    gamecontroller.gameWon = true;//siger at vi vundet i gamecontrolleren
                    player.LoadGameOverScene();//k�re gameover funktionen p� playeren
                }
                else
                {
                    //hvis det er en normal pickup bannan
                    //opdatere scores
                    ScoreManager.bananaScore++;
                    ScoreManager.totalBananas++;
                    Destroy(gameObject);//�del�gger bananen
                }
            }
        }
        else
        {//hvis det er en kastede banan
            if (collision.CompareTag("Enemy"))
            {
                //hvis man har ramt en enemy skal den skade enemyen 1 skade
                collision.GetComponent<Enemy>().dealDamage(1);
                Destroy(gameObject);//�del�g bananen
            }
            else if (collision.CompareTag("Ground") && !iFrames)
            {
                //hvis den bare ramme jorden og der ikke er iframes til �del�g bannanen
                Destroy(gameObject);
            }
        }
        
    }
}
