using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    
    private float currentTime;
    private float bestTime;
    public TextMeshProUGUI bestTimeText;
    private int upgrades = 16;
    private float upgradeTime;
    private float upgradeTimeMax = 20f;
    private float enemyUpgradeTime;
    private float enemyUpgradeTimeMax = 40f;
    private float enemyCooldown;
    private float enemyCooldownMax;
    private float enemyCooldownStart = 2f;
    public TextMeshProUGUI timerText;
    public MyGameManager myGameManager;
    public EnemyManager myEnemyManager;

    void Awake()
    {
        bestTime = PlayerPrefs.GetFloat("Record", 0);
        float minutes = Mathf.FloorToInt(bestTime / 60);
        float seconds = Mathf.FloorToInt(bestTime % 60);
        bestTimeText.text = "Best Time: " + string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void ResetTimer()
    {
        myEnemyManager.ResetEnemyStats();
        currentTime = 0f;
        upgradeTime = upgradeTimeMax;
        upgrades = 16;
        enemyUpgradeTime = enemyUpgradeTimeMax;
        enemyCooldownMax = enemyCooldownStart;
        enemyCooldown = enemyCooldownMax;
    }

    // Update is called once per frame
    void Update()
    {   
        currentTime += Time.deltaTime;
        upgradeTime -= Time.deltaTime;
        enemyCooldown -= Time.deltaTime;
        enemyUpgradeTime -= Time.deltaTime;
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        if (upgradeTime <= 0f && upgrades != 0)
        {
            upgradeTime = upgradeTimeMax; 
            upgrades--;
            if (enemyCooldownMax >= .5f)
            {
                enemyCooldownMax -= 0.1f;
            }
            myGameManager.StartUpgrade();
        }
        if (enemyUpgradeTime <= 0f)
        {
            enemyUpgradeTime = enemyUpgradeTimeMax;
            myEnemyManager.UpgradeRandom();
        }
        if (enemyCooldown <= 0f)
        {
            enemyCooldown = enemyCooldownMax;
            myEnemyManager.SpawnEnemy();
        }
    }

    public void CheckRecord()
    {
        bestTime = PlayerPrefs.GetFloat("Record", 0);
        if (bestTime > currentTime)
        {
            return;
        }
        bestTime = currentTime;
        PlayerPrefs.SetFloat("Record", bestTime);
        float minutes = Mathf.FloorToInt(bestTime / 60);
        float seconds = Mathf.FloorToInt(bestTime % 60);
        bestTimeText.text = "Best Time: " + string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
