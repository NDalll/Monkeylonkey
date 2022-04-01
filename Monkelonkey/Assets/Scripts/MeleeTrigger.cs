using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeTrigger : MonoBehaviour
{
    private Enemy entitet;
    // Start is called before the first frame update
    void Start()
    {
        entitet = transform.parent.GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("melle begin");
        if (collision.CompareTag("Player"))
        {
            
            entitet.StartMelee();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            entitet.StopMelee();
        }
    }
}
