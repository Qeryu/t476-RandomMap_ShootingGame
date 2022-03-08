using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float shootSpeed=50.0f;
    [SerializeField] private float damage = 1.0f;
    [SerializeField] private float lifetime;
    public LayerMask collisionMask;
    public float x, y, z;
    //public Transform originTransform;
    
    
    private void Start()
    {
        Destroy(gameObject, lifetime);
        x = transform.position.x;//这些是不会随时间而动的
        y = transform.position.y;
        z = transform.position.z;
        //originTransform = gameObject.transform;这个等于直接拖的transform组件，当然会也随时间变化而变化


       // Debug.Log(gameObject.transform.position);//这里的transform.position是对的

    }
    private void Update()
    {    //为何我的子弹无尽下落,他又没有rb

       
        transform.Translate(-transform.forward * shootSpeed * Time.deltaTime);//这是一个移动
        //transform.Translate(Vector3.up* shootSpeed * Time.deltaTime);
        //transform.Translate(Vector3.forward* shootSpeed * Time.deltaTime);//他不行
        
        //会不会因为transform一直在动啊


        CheckCollision();
    }

    private void CheckCollision()
    {
        //Debug.Log("111");这个方向不一定对啊，毕竟我的这个子弹她。第21行的发射方向就有点问题
       //而且，要碰上，这得一般高啊虽然确实差不多高了。。。。
        Ray ray = new Ray(new Vector3(x, y, z), transform.up);
        //Debug.DrawRay(new Vector3(x,y,z), transform.up*100,Color.red,10,false);//咋没用啊
        //Debug.Log("@222");
        //声明一个raycasthit结构体类型：hitInfo
        RaycastHit hitInfo;
        
        //意思是physic.Raycast方法返回布尔值，还返回了一饿out修饰的raycasthit类型的数据，包含检测得到的点的详细数据
        //最后一个参数是询问是否要忽略射线方向collider组件中的isTriggr，这里选择不忽略
        if (Physics.Raycast(ray, out hitInfo, shootSpeed * Time.deltaTime, collisionMask, QueryTriggerInteraction.Collide))
            //;找到报空原因了。类目
        {
            
            //击中敌人了，敌人受伤
            HitEnemy(hitInfo);
        }
    }

    private void HitEnemy(RaycastHit _hitInfo)
    {
        //获取敌人的IDamageable接口中的TakenDamage方法来对敌人造成伤害
       
        IDamageable damageable = _hitInfo.collider.GetComponent<IDamageable>();
        if (damageable != null)
        {
            
            damageable.TakenDamage(damage);
            //Destroy(gameObject);注意销毁位置
        }
        Destroy(gameObject);
    }
}
