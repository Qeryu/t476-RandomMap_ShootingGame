using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy : LivingEntity
{
    public enum State {Idle,Chasing,Attacking};
    State currentState;
    //配合navmeshagent使用
    private NavMeshAgent navMeshAgent;
    private Transform target;//追击player
    LivingEntity targetEntity;
    //写一下敌人攻击player
    float attackDistanceThreshold = 1.5f;
    float nextAttackTime;
    float AttackTime=1f;
    float damage = 1f;
    //攻击后player变色
    Material playerSkinMaterial;
    Color originalColor;
    bool hasTarget;

    protected override void Start()
    {
        base.Start();
        navMeshAgent = GetComponent<NavMeshAgent>();
       
        //代码严谨性
        currentState = State.Chasing;
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        //StartCoroutine(updatePath());
        Invoke("PlayerMaterialInvoke", 0.2f);
    }

    private void PlayerMaterialInvoke()
    {
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;
            playerSkinMaterial = target.GetComponent<Renderer>().material;
            targetEntity = target.GetComponent<LivingEntity>();
            ////每当生成新的敌人，就要将这个player死亡后敌人的的阵亡事件处理器，订阅到事件onDeath上
            targetEntity.onDeath += OnTargetDeath;
            originalColor = playerSkinMaterial.color;
            Debug.Log(originalColor);
        }
    }
    
    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }
    bool state = false;
    void Update()
    {
        
        //if (!target)不能这么写的啦，target会更新的啦
        if(GameObject.FindGameObjectWithTag("Player") != null&&state==false)
        {
            state = true;
            //target = GameObject.FindGameObjectWithTag("Player").transform;
            StartCoroutine(updatePath());
            
        }

       
        //用vector3.distance有点费，只需要在平面判断距离
        float squareDstToTarget = (target.position - transform.position).sqrMagnitude;
        if (squareDstToTarget < Mathf.Pow(attackDistanceThreshold,2))
        {
            if (Time.time >= nextAttackTime)
            {
                //attack
                StartCoroutine(Attack());
                nextAttackTime = Time.time + AttackTime;
            }
        }


        //锲而不舍类敌人
       // navMeshAgent.SetDestination(target.position);
        //摸鱼类敌人
        
    }
    IEnumerator Attack()
    {
        currentState = State.Attacking;
        navMeshAgent.enabled = false;
        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position-(dirToTarget*0.3f);
        playerSkinMaterial.color = Color.red;//为什么这里时不时爆出一个找不到呢
        float attackSpeed = 3f;
        bool hasAppliedDamage = false;
        //lerp
        float percent = 0;
        while (percent <= 1)
        {
            if (percent >= 0.5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakenDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-percent * percent + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
            yield return null;//保證每次循环遍历都从上一次停止的地方开始执行
        }
        playerSkinMaterial.color = originalColor;
        navMeshAgent.enabled = true;
        currentState = State.Chasing;

    }

    IEnumerator updatePath()//迭代器（计数器）
    {
        float updateRate = 3.0f;
        //是不是这里一直while啊，这里一看就是。。。应该改成if吧，我真是宇宙无敌傻逼了
        //但是教程用了while也运行了，我不能理解，哦哦，人家吧yieldreturn放在while里，当然可以啦
        //而且人家的调用在start里
        while (target != null)
        {
            if (currentState == State.Chasing)
            {
                Vector3 preTargetPos = new Vector3(target.position.x, 0, target.position.z);
                navMeshAgent.SetDestination(target.position);
            }
            yield return new WaitForSeconds(updateRate);
            //这样就对了，这是为什么呢？
            //看来我对上面这个语句掌握的还是不到位啊
        }
        //有些许不理解在里面，但写都写了，先这样凑合一下
        //这明明只调用一次的东西，被我调用这么多次。。。
        //yield return new WaitForSeconds(updateRate);//过三秒再去遍历上边几行

    }
}
