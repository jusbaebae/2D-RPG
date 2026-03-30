using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float moveSpeed;
    public float lifeTime;

    TextMeshProUGUI text;

    Vector3 moveDirection;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();

        float randomX = Random.Range(-0.5f, 0.5f); //방향을 랜덤으로
        moveDirection = new Vector3(randomX, 1f, 0f);
    }

    private void Update()
    {
        if (StatsManager.Instance.isCrit)
        {
            text.color = Color.magenta;
            transform.localScale += Vector3.one * Time.deltaTime * 0.25f; //살짝 커지게
        }
        else
        {
            transform.localScale += Vector3.one * Time.deltaTime * 0.15f; //살짝 커지게
        }
        transform.position += moveDirection * moveSpeed * Time.deltaTime; //위로 올라가게
        lifeTime -= Time.deltaTime;
        if(lifeTime < 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetDamage(int damage)
    {
        text.text = damage.ToString();
    }
}
