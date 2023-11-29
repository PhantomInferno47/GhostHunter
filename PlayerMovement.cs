using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    // Player animator
    private Animator anim;
    
    private SpriteRenderer sprite;

    //PLayer Rigidbody
    private Rigidbody2D rb;
    
    // distance of collisions
    private float collisionOffset = 0.1f;
    // filter for collisions
    public ContactFilter2D movementFilter;
    // list of collisions when doing detection
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    
    //PLayer GameObject
    public GameObject playerGameObject;
    
    public GameObject playerStart;
    
    //Game Manager Script
    public MyGameManager myGameManager;

    // direction of movement
    private Vector2 movement;

    // direction the sprite is facing
    private Vector2 CurrentDirection = new Vector2(0, 0);

    // should player be frozen
    private bool freeze = false;

    // projectile to fire
    public GameObject projectilePrefab;

    // is next projectile ready to fire
    private bool canFire = true;

    public AudioSource fireSound;
    public AudioSource hitSound;

    //player stats
    private int maxLevel = 2;
    private int movementSpeedLevel = 0;
    private List<float> movementSpeedList = new List<float> { 2f, 3f, 4f };
    public TextMeshProUGUI movementText;
    private int firingSpeedLevel = 0;
    private List<float> firingSpeedList = new List<float> { .6f, .4f, .2f };
    public TextMeshProUGUI firingText;
    private int projectileDamageLevel = 0;
    private List<int> projectileDamageList = new List<int> { 10, 15, 20 };
    public TextMeshProUGUI damageText;
    private int projectileSpeedLevel = 0;
    private List<int> projectileSpeedList = new List<int> { 10, 12, 14 };
    public TextMeshProUGUI projSpeedText;
    private int reloadSpeedLevel = 0;
    private List<float> reloadSpeedList = new List<float> { 2f, 1f, .5f };
    public TextMeshProUGUI reloadText;
    private int magazineCapacityLevel = 0;
    private List<int> magazineCapacityList = new List<int> { 6, 9, 12 };
    public TextMeshProUGUI magText;
    private float projectilesRemaining;
    public TextMeshProUGUI ammoText;
    private int projectileAccuracyLevel = 0;
    private List<float> projectileAccuracyList = new List<float> { .15f, .1f, 0f };
    public TextMeshProUGUI accuracyText;
    private int maxHealthLevel = 0;
    private List<int> maxHealthList = new List<int> { 20, 25, 30 };
    public TextMeshProUGUI healthLevelText;
    private float currentHealth = 20f;
    public TextMeshProUGUI healthText;

    void Start()
    {
        // get player's animator and rigidbody
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        sprite.enabled = false;
        freeze = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Only do these actions if player is not frozen
        if (!freeze)
        {
            // Left movement
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                movement = new Vector2(-(movementSpeedList[movementSpeedLevel]), 0);
                CurrentDirection = new Vector2(-1, 0);
                anim.SetInteger("direction", 2);
                anim.SetBool("idle", false);
            }
            // Right movement
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                movement = new Vector2(movementSpeedList[movementSpeedLevel], 0);
                CurrentDirection = new Vector2(1, 0);
                anim.SetInteger("direction", 3);
                anim.SetBool("idle", false);
            }
            // No movement input
            else
            {
                movement = new Vector2(0, 0);
                anim.SetInteger("direction", 0);
                anim.SetBool("idle", true);
            }

            // attempt to move player
            MovePlayer(movement);


            if (Input.GetMouseButton(0))
            {
                if (canFire)
                {
                    Fire();
                }
            }
        }
    }

    public void MovePlayer(Vector2 direction)
    {
        // Check for potential collisions
        int count = rb.Cast(
            direction, // Direction of movement
            movementFilter, // Layers where interactions can occur
            castCollisions, // List of collisions to store the found collisions into after the Cast is finished
            Time.fixedDeltaTime + collisionOffset); // distance to check

        foreach (RaycastHit2D hit in castCollisions)
        {
            if (hit.collider.tag == "Border")
                return;
        }
        Vector2 moveVector = direction * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveVector);
    }

    public void Fire()
    {
        fireSound.Play();
        canFire = false;
        GameObject projectile = (GameObject)Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().SetStats(projectileDamageList[projectileDamageLevel], projectileSpeedList[projectileSpeedLevel], projectileAccuracyList[projectileAccuracyLevel]);
        StartCoroutine(NextProjectile());
    }

    public IEnumerator NextProjectile()
    {
        projectilesRemaining = projectilesRemaining - 1;
        ammoText.text = projectilesRemaining + "/" + magazineCapacityList[magazineCapacityLevel];
        if (projectilesRemaining <= 0)
        {
            StartCoroutine(Reload());

            yield return null;
        }
        else
        {
            yield return new WaitForSeconds(firingSpeedList[firingSpeedLevel]);

            canFire = true;
        }
    }

    public IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadSpeedList[reloadSpeedLevel]);

        projectilesRemaining = magazineCapacityList[magazineCapacityLevel];
        ammoText.text = projectilesRemaining + "/" + magazineCapacityList[magazineCapacityLevel];

        canFire = true;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            ApplyDamage(col.gameObject.GetComponent<Enemy>().damage);
            Destroy(col.gameObject);
        }
    }

    public void ApplyDamage(float damage)
    {
        currentHealth -= damage;
        healthText.text = currentHealth + "/" + maxHealthList[maxHealthLevel];
        if (currentHealth <= 0)
        {
            EndGame();
            myGameManager.EndGame();
            return;
        }
        StartCoroutine(DamageIndicator());
        hitSound.Play();
    }
    
    public IEnumerator DamageIndicator()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = Color.white;
    }

    public void PausePlayer()
    {
        anim.SetInteger("direction", 0);
        anim.SetBool("idle", true);
        freeze = true;
    }

    public void UnpausePlayer()
    {
        freeze = false;
    }
    
    public void EndGame()
    {
        anim.SetInteger("direction", 0);
        anim.SetBool("idle", true);
        freeze = true;
        sprite.enabled = false;
    }
    
    public void StartGame()
    {
        playerGameObject.transform.position = playerStart.transform.position;
        sprite.enabled = true;
        movementSpeedLevel = 0;
        movementText.text = "lv. 1";
        firingSpeedLevel = 0;
        firingText.text = "lv. 1";
        maxHealthLevel = 0;
        healthLevelText.text = "lv. 1";
        reloadSpeedLevel = 0;
        reloadText.text = "lv. 1";
        projectileSpeedLevel = 0;
        projSpeedText.text = "lv. 1";
        projectileDamageLevel = 0;
        damageText.text = "lv. 1";
        magazineCapacityLevel = 0;
        magText.text = "lv. 1";
        projectileAccuracyLevel = 0;
        accuracyText.text = "lv. 1";
        projectilesRemaining = magazineCapacityList[magazineCapacityLevel];
        currentHealth = maxHealthList[maxHealthLevel];
        healthText.text = currentHealth + "/" + maxHealthList[maxHealthLevel];
        ammoText.text = projectilesRemaining + "/" + magazineCapacityList[magazineCapacityLevel];
        freeze = false;
    }

    public void UpgradeHealth()
    {
        if (maxHealthLevel >= maxLevel)
        {
            return;
        }
        maxHealthLevel++;
        currentHealth = maxHealthList[maxHealthLevel];
        healthText.text = currentHealth + "/" + maxHealthList[maxHealthLevel];
        
        if (maxHealthLevel >= maxLevel)
        {
            healthLevelText.text = "MAX";
        }
        else 
        {
            healthLevelText.text = "lv." + (maxHealthLevel + 1);
        }

        myGameManager.EndUpgrade();
    }

    public void UpgradeMagazineCapacity()
    {
        if (magazineCapacityLevel >= maxLevel)
        {
            return;
        }
        magazineCapacityLevel++;
        projectilesRemaining = magazineCapacityList[magazineCapacityLevel];
        ammoText.text = projectilesRemaining + "/" + magazineCapacityList[magazineCapacityLevel];
        
        if (magazineCapacityLevel >= maxLevel)
        {
            magText.text = "MAX";
        }
        else 
        {
            magText.text = "lv." + (magazineCapacityLevel + 1);
        }

        myGameManager.EndUpgrade();
    }

    public void UpgradeMovementSpeed()
    {
        if (movementSpeedLevel >= maxLevel)
        {
            return;
        }
        movementSpeedLevel++;
        
        if (movementSpeedLevel >= maxLevel)
        {
            movementText.text = "MAX";
        }
        else 
        {
            movementText.text = "lv." + (movementSpeedLevel + 1);
        }

        myGameManager.EndUpgrade();
    }

    public void UpgradeFiringSpeed()
    {
        if (firingSpeedLevel >= maxLevel)
        {
            return;
        }
        firingSpeedLevel++;
        
        if (firingSpeedLevel >= maxLevel)
        {
            firingText.text = "MAX";
        }
        else 
        {
            firingText.text = "lv." + (firingSpeedLevel + 1);
        }

        myGameManager.EndUpgrade();
    }

    public void UpgradeReloadSpeed()
    {
        if (reloadSpeedLevel >= maxLevel)
        {
            return;
        }
        reloadSpeedLevel++;
        
        
        if (reloadSpeedLevel >= maxLevel)
        {
            reloadText.text = "MAX";
        }
        else 
        {
            reloadText.text = "lv." + (reloadSpeedLevel + 1);
        }

        myGameManager.EndUpgrade();
    }

    public void UpgradeProjectileSpeed()
    {
        if (projectileSpeedLevel >= maxLevel)
        {
            return;
        }
        projectileSpeedLevel++;
        
        if (projectileSpeedLevel >= maxLevel)
        {
            projSpeedText.text = "MAX";
        }
        else 
        {
            projSpeedText.text = "lv." + (projectileSpeedLevel + 1);
        }

        myGameManager.EndUpgrade();
    }

    public void UpgradeProjectileDamage()
    {
        if (projectileDamageLevel >= maxLevel)
        {
            return;
        }
        projectileDamageLevel++;
        
        if (projectileDamageLevel >= maxLevel)
        {
            damageText.text = "MAX";
        }
        else 
        {
            damageText.text = "lv." + (projectileDamageLevel + 1);
        }
        myGameManager.EndUpgrade();
    }

    public void UpgradeProjectileAccuracy()
    {
        if (projectileAccuracyLevel >= maxLevel)
        {
            return;
        }

        projectileAccuracyLevel++;

        if (projectileAccuracyLevel >= maxLevel)
        {
            accuracyText.text = "MAX";
        }
        else 
        {
            accuracyText.text = "lv." + (projectileAccuracyLevel + 1);
        }

        myGameManager.EndUpgrade();
    }

}