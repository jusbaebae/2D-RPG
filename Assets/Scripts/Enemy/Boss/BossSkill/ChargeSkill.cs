using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeSkill : BossSkill
{
    public GameObject telegraphObject;

    public float chargeSpeed;
    public float chargeTime; //해당 시간만큼 돌진
    public float skillTime; //범위 표기후 돌진발동시간
    public float maxDistance;
    public float moved = 0;

    bool isCharging = false;

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (!isCharging) return;

        if (coll.gameObject.CompareTag("Player"))
        {
            coll.gameObject.GetComponent<PlayerHealth>().ChangeHealth(-10);
            coll.gameObject.GetComponent<PlayerMovement>().Knockback(transform, 15, 0.3f);
            //Debug.Log("돌진중 맞았다!");
        }
    }

    public override IEnumerator UseSkill(Boss_Movement boss)
    {
        boss.isImmuneToKnockback = true;
        boss.isUsingSkill = true;

        boss.rb.velocity = Vector2.zero;

        Vector2 dir = (boss.player.position - boss.transform.position).normalized;

        ShowTelegraph(dir);

        yield return new WaitForSeconds(skillTime);

        float timer = 0;
        float maxDistance = 10f;
        float moved = 0;

        HideTelegraph();

        isCharging = true;
        
        while (timer < chargeTime && moved < maxDistance)
        {
            Vector2 move = dir * chargeSpeed * Time.deltaTime;
            boss.rb.velocity = dir * chargeSpeed;

            moved += move.magnitude;
            timer += Time.deltaTime;

            yield return null;
        }

        boss.rb.velocity = Vector2.zero;

        boss.isUsingSkill = false;
        boss.isImmuneToKnockback = false;
        isCharging = false;
    }

    float offsetDistance = 5f; //Telegraph 위치 조정
    void ShowTelegraph(Vector2 dir) //돌진 범위 나타내기
    {
        telegraphObject.SetActive(true);

        Vector3 offset = (Vector3)(dir * offsetDistance) + new Vector3(0, -1f, 0); //y축 위치도 조정
        telegraphObject.transform.position = transform.position + offset;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        telegraphObject.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void HideTelegraph()
    {
        telegraphObject.SetActive(false);
    }
}
