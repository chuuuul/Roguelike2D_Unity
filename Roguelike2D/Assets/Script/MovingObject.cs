using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    public float moveTime = 0.1f;
    public LayerMask blockingLayer;

    private BoxCollider2D boxcollider;
    private Rigidbody2D rg2D;
    private float inverseMoveTime ;

    protected virtual void Start()
    {
        rg2D = GetComponent<Rigidbody2D>();
        boxcollider = GetComponent<BoxCollider2D>();
        inverseMoveTime = 1.0f / moveTime;

    }


    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = rg2D.position;                          // 시작지점 저장
        Vector2 end = new Vector2(xDir, yDir) + start;          // 움직일 위치 저장

        boxcollider.enabled = false;
        hit = Physics2D.Linecast(start, end,blockingLayer);     // 움직일 위치에 blockingLayer가 null이라면 움직여도됨
        boxcollider.enabled = true;

        if (hit.transform == null)                              // 움직인다
        {
            StartCoroutine(SmoothingMoving(end));
            return true;
        }
        return false;                                           // 움직일수 없다

    }

    protected IEnumerator SmoothingMoving(Vector3 end)
    {
        float sqrRemainDistance = (transform.position - end).sqrMagnitude;      // 자신의 위치와 목적위치 거리 계산
        
        while(float.Epsilon < sqrRemainDistance)                // 거리가 없을때까지 움직인다
        {
            Vector2 newPosition = Vector2.MoveTowards(transform.position, end, Time.deltaTime * inverseMoveTime);
            rg2D.MovePosition(newPosition);
            sqrRemainDistance = (transform.position - end).sqrMagnitude;      // 자신의 위치와 목적위치 거리 계산
            yield return null;   
        }
    }

    protected virtual void AttemptMove<T>(int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir , out hit);          //움직일 수 있는지 확인


        if (hit.transform == null)                          // 움직일수 없으면 아무것도 하지 않는다.
            return;

        T hitComponent = hit.transform.GetComponent<T>();

        if (canMove == false && hitComponent != null)
            OnCantMove(hitComponent);
    }

    protected abstract void OnCantMove<T>(T component)
        where T : Component;
    


}
