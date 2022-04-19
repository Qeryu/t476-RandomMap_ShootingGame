using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[RequireComponent(typeof(Rigidbody
public class PlayerController : LivingEntity
    //这里不用考虑会不会不继承monobehavior了，因为我们的livingE是继承他的
{
    private Rigidbody rb;
    private Vector3 moveInput;
    //[SerializeField] private 
    public float moveSpeed;
   /* [Header("player呆住不动的时间")]
    public static Transform playerStayTrans;
    public static bool isStay;
    public float maxStayTime = 0.5f;
    public float tillNowStayTime = 0;
    public float StartStayTime = 0;*/
    //通过关键字override重写父类的方法
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
        
    }
    
    private void Update()
    {
        //如果掉下去了也死亡
        if (transform.position.y < -3f)
        {
            Die();
        }
        moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
       /* 我写的判断是否静止一段时间if (moveInput==new Vector3(0,0,0))
        {
            Debug.Log("111");
            if (tillNowStayTime == 0)
            {   
                StartStayTime = Time.time;
                tillNowStayTime = 0.1f;
                Debug.Log(StartStayTime);
            }
            else
            {
                tillNowStayTime = Time.time - StartStayTime;
                if (tillNowStayTime >= maxStayTime)
                {
                    Debug.Log("多比是自由的小精灵");
                    isStay = true;
                    playerStayTrans = transform;
                    
                }
            }
        }
        else
        {
            tillNowStayTime = 0;
            isStay = false;
        }*/

        LookAtCursor();

    }
    //移动一般都用fixedupdate，0.2s一更新
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);//这是违背真是物理世界的,但下面这个太卡顿了
      // rb.velocity = new Vector3(moveInput.normalized.x * moveSpeed, rb.velocity.y, moveInput.normalized.z * moveSpeed);
    }
    private void LookAtCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        float distoGround;
        if(plane.Raycast(ray,out distoGround))
        {
            Vector3 point = ray.GetPoint(distoGround);
            //这里要纠正成正确的y轴，和player平行，否则使正方体各个方向转动
           Vector3 rightPoint = new Vector3(point.x, transform.position.y, point.z);

            transform.LookAt(rightPoint);
        }
    }
    public override void Die()
    {
        AudioManager.instance.PlaySound("PlayerDeath", transform.position);
        base.Die();
    }
}
