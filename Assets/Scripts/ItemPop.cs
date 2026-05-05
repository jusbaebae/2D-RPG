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
    public LayerMask obstacleLayer; //벽 레이어 선택

    private float timer = 0f;
    private Vector2 velocity;
    private bool isMoving = false;

    public void Pop()
    {
        Vector2 dir = GetSafeDirection();

        velocity = new Vector2(dir.x * power, upForce);

        timer = 0f;
        isMoving = true;
    }

    private Vector2 GetSafeDirection()
    {
        float checkRadius = 0.3f; //아이템의 크기에 맞춰 조절

        //최대 10번까지 새로운 방향을 시도해봄
        for (int i = 0; i < 10; i++)
        {
            Vector2 testDir = Random.insideUnitCircle.normalized;

            //예상되는 도착 지점 계산
            Vector2 expectedPos = (Vector2)transform.position + (testDir * power * 0.5f);

            //해당 지점에 장애물 있는지 체크
            Collider2D hit = Physics2D.OverlapCircle(expectedPos, checkRadius, obstacleLayer);

            if (hit == null)
            {
                return testDir; //장애물이 없으면 이 방향 채택
            }
        }

        if (PlayerMovement.Instance != null)
        {
            return (PlayerMovement.Instance.transform.position - transform.position).normalized; //만약 10번 다실패하면 플레이어 방향쪽으로 던지기
        }

        return Vector2.up; //최악의 경우엔 그냥 위로 던지기 (에러 방지)
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
