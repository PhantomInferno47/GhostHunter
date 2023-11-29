using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    // enemy to spawn
    public GameObject enemyPrefab;
    public GameObject enemyTarget;
    private int maxLevel = 2;
    private int healthLevel = 0;
    private List<float> healthList = new List<float> { 20f, 30f, 40f };
    private int damageLevel = 0;
    private List<float> damageList = new List<float> { 5f, 7f, 10f };
    private int speedLevel = 0;
    private List<float> speedList = new List<float> { 3.5f, 4f, 5f };

    
    public void SpawnEnemy()
    {
        float x = 0f;
        if (UnityEngine.Random.Range(0,2) == 1)
        {
            x = Screen.width;
        }
        float y = UnityEngine.Random.Range(100f,Screen.height);
        Vector3 p = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 0f));
        GameObject projectile = (GameObject) Instantiate(enemyPrefab, p, Quaternion.identity);
        projectile.GetComponent<Enemy>().SetTarget(enemyTarget);
        projectile.GetComponent<Enemy>().speed = speedList[speedLevel];
        projectile.GetComponent<Enemy>().damage = damageList[damageLevel];
        projectile.GetComponent<Enemy>().health = healthList[healthLevel];
    }

    public void UpgradeRandom()
    {
        int r = UnityEngine.Random.Range(0,3);
        if (r == 0)
        {
            UpgradeEnemyDamage();
        }
        if (r == 1)
        {
            UpgradeEnemyHealth();
        }
        if (r == 2)
        {
            UpgradeEnemySpeed();
        }
    }

    public void UpgradeEnemyHealth()
    {
        if (healthLevel < maxLevel)
        {
            healthLevel++;
        }
    }

    public void UpgradeEnemyDamage()
    {
        if (damageLevel < maxLevel)
        {
            damageLevel++;
        }
    }

    public void UpgradeEnemySpeed()
    {
        if (speedLevel < maxLevel)
        {
            speedLevel++;
        }
    }

    public void ResetEnemyStats()
    {
        healthLevel = 0;
        speedLevel = 0;
        damageLevel = 0;
    }

}
