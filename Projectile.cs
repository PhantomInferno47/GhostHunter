using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Random;

public class Projectile : MonoBehaviour
{
    public Vector2 direction;
    private float speed = 0f;
    private float damage = 0f;
    private float accuracy = 0f;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Vector3 screenPosition = new Vector3( Input.mousePosition.x, Input.mousePosition.y, 0.0f );
        Vector2 target = Camera.main.ScreenToWorldPoint(screenPosition);
        Vector2 path = target - new Vector2(transform.position.x, transform.position.y);
        var distance = path.magnitude;
        direction = path / distance;
    }

    public void SetStats(float setDamage, float setSpeed, float setAccuracy)
    {
        speed = setSpeed;
        damage = setDamage;
        accuracy = setAccuracy;
        float x = UnityEngine.Random.Range(-accuracy, accuracy);
        float y = UnityEngine.Random.Range(-accuracy, accuracy);
        direction = direction + new Vector2(x,y);
    }

    void FixedUpdate() {
        
        Vector2 moveVector = direction * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveVector);

        Vector3 currentScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        if (currentScreenPosition.y >= Screen.height || currentScreenPosition.y <= 0 || currentScreenPosition.x >= Screen.width - 20 || currentScreenPosition.x <= -20)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            col.gameObject.GetComponent<Enemy>().ApplyDamage(damage);
            Destroy(gameObject);
        }
    }
}
