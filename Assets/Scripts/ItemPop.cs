using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPop : MonoBehaviour
{
    public float power = 5f;      //튀어나가는 힘
    public float upForce = 6f;    //위로 뜨는 힘
    public float gravity = -15f;  //중력
    public float drag = 2f;       //마찰
    public float stopTime = 1f; //1초 후 멈춤
    private float timer = 0f;

    private Vector2 velocity;
    private bool isMoving = false;

    public void Pop()
    {
        Vector2 dir = Random.insideUnitCircle.normalized;

        velocity = new Vector2(dir.x * power, upForce);

        timer = 0f;
        isMoving = true;
    }

    void Update()
    {
        if (!isMoving) return;
        timer += Time.deltaTime;

        //중력 적용
        velocity.y += gravity * Time.deltaTime;

        //이동
        transform.position += (Vector3)(velocity * Time.deltaTime);

        //감속 (x축)
        velocity.x = Mathf.Lerp(velocity.x, 0, drag * Time.deltaTime);

        if (timer >= stopTime)
        {
            velocity = Vector2.zero;
            isMoving = false;
        }
    }
}
