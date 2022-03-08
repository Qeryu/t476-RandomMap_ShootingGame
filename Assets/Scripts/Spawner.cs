using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
//负责进攻波数的管理
public class Spawner : MonoBehaviour
{
    public GameObject enemyprefab;
    public Wave[] waves;//或者使用list，dictionary

    private Wave currentWave;
    private int currentIndex;

    public int waitSpawnNum;//这一波还有多少敌人没有被生成
    public int spawnAliveNum;//这一波还有多少敌人活着d
    public float nextSpawnTime;//当前波数中每个敌人生成的时间间隔
    public event System.Action<int> OnNewWave;//神奇，和mapgenerator联动

    GameObject playerEntity;
    Transform playerT;
    MapGenerator map;

    //判断是是否静止过长时间，它是采用固定时间一判断。那用协程岂不是更好？
    
    public float timeBetweenCampingCheck = 2.5f;//露营
    float nextCampCheckTime;
    float campThresholdDistance = 1.5f;//阈值距离
    Vector3 campPositionOld;
    bool isCamping;


    private void Start()
    {
        //从自身开始查找，查找挂载的某类型
        //这也行？这是它挂的父类啊
        
       
        nextCampCheckTime = timeBetweenCampingCheck + Time.time;
        
        map = FindObjectOfType<MapGenerator>();
        Invoke("PlayPositionInvoke", 0.2f);
        
        NextWave();       
    }
    private void PlayPositionInvoke()
    {
        playerEntity = GameObject.FindGameObjectWithTag("Player");
        //想起来了，原因是他在start调用了，而我的player还需要实例化呢。太悲伤了吧，这我魔改一下用一下invoke吧
        playerT = playerEntity.transform;
        campPositionOld = playerT.position;
    }
    private void NextWave()
    {
        currentIndex++;
        //学到了这个写法String.Format (String, Object) 将指定的 String 中的格式项替换为指定的 Object 实例的值的文本等效项
        Debug.Log(string.Format("current wave:{0}", currentIndex));
        if (currentIndex <= waves.Length)
        {
            currentWave = waves[currentIndex - 1];
            waitSpawnNum = currentWave.enemyNum;
            spawnAliveNum = currentWave.enemyNum;
            if (OnNewWave != null)
            {
                OnNewWave(currentIndex);
            }
        }
        
    }


    private void Update()
    {
        if (Time.time > nextCampCheckTime)
        {
            nextCampCheckTime = timeBetweenCampingCheck + Time.time;

            isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);
            campPositionOld = playerT.position;
        }
        if (waitSpawnNum > 0&&nextSpawnTime<=Time.time)
        {
            waitSpawnNum--;
            StartCoroutine("SpawnEnemy");
            nextSpawnTime = Time.time + currentWave.timeBtwSpawn;
        }
    }

    //有关敌人出现时的延时的效果，采用协程函数的思路
    //为啥要用呢，没必要啊
    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1.0f;
        float tileFlashSpeed = 4;
        //HERE
        Transform randomTiile;
        /* 这段时我之前写那个 if (PlayerController.isStay == true)
          {
              randomTiile = PlayerController.playerStayTrans;
          }
          else randomTiile = MapGenerator.instance.GetRandomOpenTile();

          */
        randomTiile = MapGenerator.instance.GetRandomOpenTile();
        if (isCamping)
        {
            randomTiile = map.getTileFromPosition(playerT.position);
        }
        #region
        Material tileMat = randomTiile.GetComponent<MeshRenderer>().material;
        Color originalColor = Color.white;
        Color flashColor = Color.red;
        float spawnTimer = 0;
        #endregion
        //延时1s生成敌人
        while (spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(originalColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));
            spawnTimer += Time.deltaTime;
            yield return null;
        }


        
        GameObject spawnEnemy = Instantiate(enemyprefab, randomTiile.position+Vector3.up, Quaternion.identity);
        //每当生成新的敌人，就要将这个敌人的阵亡事件处理器，订阅到事件onDeath上
        spawnEnemy.GetComponent<Enemy>().onDeath += EnemyDeath;
    }

    //当敌人阵亡时
    private void EnemyDeath()
    {
        spawnAliveNum--;
        if (spawnAliveNum <= 0&&waitSpawnNum<=0)
        {
            NextWave();
        }
    }
}
