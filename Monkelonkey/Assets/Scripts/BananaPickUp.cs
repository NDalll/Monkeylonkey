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
    public bool isFire;
    public float rotateSpeed;

    void Awake()
    {
        startPostion = transform.position;
        endPostion = transform.position + Vector3.up*floatingDis;
        isFire = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFire)
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isFire)
        {
            if (collision.CompareTag("Player"))
            {
                ScoreManager.bananaScore++;
                Destroy(gameObject);
            }
        }
        
    }
}
