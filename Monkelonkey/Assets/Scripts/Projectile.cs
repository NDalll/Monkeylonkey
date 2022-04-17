using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called before the first frame update
    [System.NonSerialized]
    public float turnforce;
    [System.NonSerialized]
    public float initProjSpeed;
    [System.NonSerialized]
    public bool isHoming;
    [System.NonSerialized]
    public float damage;
    private GameObject player;
    private bool isCollided = false;
    

    private Animator animator;
    private Rigidbody2D rb;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").gameObject;//definere spilleren
        animator = GetComponent<Animator>();//definere animatoren
        rb = GetComponent<Rigidbody2D>(); //definere rigidbody
    }

    // Update is called once per frame
    private void FixedUpdate()//Update funktion der køre ved fixed framerate, dette gør at mængden af kræft der bliver tilføjet ikke er afhængig af framerate
    {
        if (isHoming)//hvis det er et homeing skyd
        {
            rb.AddForce(GetVectorToPlayer());//tilføje force på projektilet mod spilleren
            rb.velocity = rb.velocity.normalized * initProjSpeed;//normaliser farten og gang den med initProj speed, så at farten på skydet ikke ændre sig
            transform.eulerAngles = GetOrientation();//sætter rotationen på skudet så den vender mod spilleren
        }
    }

    private Vector3 GetOrientation()
    {
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;//bruger tangens til at udregning at vinklen til spiller i grader
        return new Vector3(0, 0, angle);//retunere en vektor som at z roation til vinklen
    }

    private Vector3 GetVectorToPlayer()
    {
        Vector3 vector = player.transform.position - transform.position;//udregner vektor til spilleren
        return vector.normalized * turnforce;//normalisere vektoren og ganger den med turnforce der bestemme hvor godt skydet følger efter
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isCollided)//hvis at den colidere med en spiller, og den ikke lige har colidieret med ground(fikser en fejl med du kan blive ramt gennem tynde platforme)
        {
            collision.GetComponent<Player>().dealDamage(damage);//skad spillere
        }
        if (collision.CompareTag("Ground"))
        {
            isCollided = true;
        }
        if (collision.gameObject.CompareTag("Enemy") == false && collision.isTrigger == false)//kan kun gå i stykker til collideres der ikke er triggers heller ikke tilhøre en enemy
        {
            rb.velocity = Vector3.zero;
            animator.SetTrigger("ColliderHit");
        }
    }

    private void Death()//bliver kladt når colliderhit animation er slut
    {
        GameObject.Destroy(gameObject);
    }
}
