using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    // Start is called before the first frame update
    [System.NonSerialized]
    public float damage;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")){//hvis at et v�ben collidere med en player, skal den skade spilleren iforhold til v�bnets skade
            collision.GetComponent<Player>().dealDamage(damage);
        }
    }
}
