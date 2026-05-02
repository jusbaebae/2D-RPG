using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeHitbox : MonoBehaviour
{
    public int damage = 10;
    public Transform boss;

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("Player"))
        {
            //Debug.Log("맞았다!!");
            coll.GetComponent<PlayerHealth>().ChangeHealth(-damage);
            coll.GetComponent<PlayerMovement>().Knockback(boss, 15, 0.3f);
        }
        else
        {
            //Debug.Log("뭔가 충돌함" + coll.name);
            return;
        }
    }
}
