using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    private Transform target;
    private SpriteRenderer sprite;
    public float health;
    public float damage;
    private AudioSource hitSound;

    // Start is called before the first frame update
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = Color.white;
        hitSound = GameObject.FindWithTag("Hit").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        sprite.flipX = transform.position.x - target.position.x > 0;
    }

    public void SetTarget(GameObject targetObject)
    {
        target = targetObject.transform;
    }

    public void ApplyDamage(float damage)
    {
        StartCoroutine(DamageIndicator());
        hitSound.Play();
        health = health - damage;
        if (health <= 0)
        {
            Destroy(gameObject);
            return;
        }
    }

    public IEnumerator DamageIndicator()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = Color.white;
    }
}
