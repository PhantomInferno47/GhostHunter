using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGameManager : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject HowToPanel;
    public GameObject timerPanel;
    public GameObject statPanels;
    public GameObject upgradePanel;
    public GameObject pauseMenu;
    public EnemyManager enemyManager;
    public PlayerMovement player;
    public Aiming aimLine;
    public Timer gameTimer;

    // Start is called before the first frame update
    void Start()
    {
        timerPanel.SetActive(false);
        HowToPanel.SetActive(false);
        startPanel.SetActive(true);
        pauseMenu.SetActive(false);
        statPanels.SetActive(false);
        upgradePanel.SetActive(false);
        aimLine.HideLine();
        Time.timeScale = 0;
    }
    
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            PauseGame();
        }
    }

    public void StartGame()
    {
        startPanel.SetActive(false);
        HowToPanel.SetActive(false);
        statPanels.SetActive(true);
        timerPanel.SetActive(true);
        player.StartGame();
        gameTimer.ResetTimer();
        pauseMenu.SetActive(false);
        upgradePanel.SetActive(false);
        aimLine.ShowLine();
        Time.timeScale = 1;
    }

    public void PauseGame()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
            player.UnpausePlayer();
            aimLine.ShowLine();
            return;
        }
        player.PausePlayer();
        aimLine.HideLine();
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void StartUpgrade()
    {
        PauseGame();
        startPanel.SetActive(false);
        HowToPanel.SetActive(false);
        statPanels.SetActive(true);
        upgradePanel.SetActive(true);
        timerPanel.SetActive(true);
        pauseMenu.SetActive(false);
        aimLine.HideLine();
    }

    public void EndUpgrade()
    {
        startPanel.SetActive(false);
        HowToPanel.SetActive(false);
        statPanels.SetActive(true);
        upgradePanel.SetActive(false);
        timerPanel.SetActive(true);
        pauseMenu.SetActive(false);
        aimLine.ShowLine();
        PauseGame();
    }

    public void QuitGame()
    {
        PauseGame();
        player.EndGame();
        EndGame();
    }

    public void HowToPlay()
    {
        startPanel.SetActive(false);
        HowToPanel.SetActive(true);
        statPanels.SetActive(false);
        upgradePanel.SetActive(false);
        timerPanel.SetActive(false);
        pauseMenu.SetActive(false);
        aimLine.HideLine();
    }

    public void BackToStart()
    {
        startPanel.SetActive(true);
        HowToPanel.SetActive(false);
        statPanels.SetActive(false);
        upgradePanel.SetActive(false);
        timerPanel.SetActive(false);
        pauseMenu.SetActive(false);
        aimLine.HideLine();
    }
    
    public void EndGame()
    {
        startPanel.SetActive(true);
        HowToPanel.SetActive(false);
        statPanels.SetActive(false);
        upgradePanel.SetActive(false);
        timerPanel.SetActive(false);
        pauseMenu.SetActive(false);
        aimLine.HideLine();
        gameTimer.CheckRecord();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject projectile in projectiles)
        {
            Destroy(projectile);
        }
        
        Time.timeScale = 0;
    }
}
