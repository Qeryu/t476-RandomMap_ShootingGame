using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunComponent : MonoBehaviour
{
    public Transform firePoint;
    public GameObject projectilePrefab;
    [SerializeField] private float fireRate = 0.1f;

    private float timer;

    private void Update()
    {
        if(Input.GetMouseButton(0))
        {
            Shot();
        }
    }
    //计时器限制发射频率
    public void Shot()
    {
        timer += Time.deltaTime;
        if (timer > fireRate)
        {
            timer = 0;
            GameObject spawnProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
           
        }
    }
}
