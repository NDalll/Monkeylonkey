using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaPickUp : MonoBehaviour
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

    void Awake()
    {
        isPickUp = true;
    }
    private void Start()
    {
        startPostion = transform.position;
        endPostion = transform.position + Vector3.up * floatingDis;
        Invoke("DisableIFrames", 0.05f);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPickUp)
        {
            if (fraction < 1)
            {
                fraction += Time.deltaTime * speed;
                transform.position = Vector3.Lerp(startPostion, endPostion, fraction);
            }
            if (fraction >= 1)
            {
                Vector3 mPos = startPostion;
                startPostion = endPostion;
                endPostion = mPos;
                fraction = 0;
            }
        }
        else
        {
            gameObject.transform.Rotate(new Vector3(0, 0, Time.deltaTime*rotateSpeed));
        }
        
    }
    private void DisableIFrames()
    {
        iFrames = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPickUp)
        {
            if (collision.CompareTag("Player"))
            {
                ScoreManager.bananaScore++;
                ScoreManager.totalBananas++;
                Destroy(gameObject);
            }
        }
        else
        {
            if (collision.CompareTag("Enemy"))
            {
                collision.GetComponent<Enemy>().dealDamage(1);
                Destroy(gameObject);
            }
            else if (collision.CompareTag("Ground") && !iFrames)
            {
                Destroy(gameObject);
            }
        }
        
    }
}
