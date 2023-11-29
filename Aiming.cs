using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aiming : MonoBehaviour
{
    public GameObject player;
    public LineRenderer aim;

    // Update is called once per frame
    void Update()
    {
        Vector3 screenPosition = new Vector3( Input.mousePosition.x, Input.mousePosition.y, 0.0f );
        Vector2 target = Camera.main.ScreenToWorldPoint(screenPosition);
        
        Vector2 path = target - new Vector2(player.transform.position.x, player.transform.position.y);
        var distance = path.magnitude;
        Vector2 direction = path / distance;
 
        Vector2 playerPosition = new Vector2 (player.transform.position.x, player.transform.position.y);
        aim.SetPosition(0, playerPosition + direction);
        aim.SetPosition(1, playerPosition + (direction * 3));
    }

    public void HideLine()
    {
        aim.enabled = false;
    }

    public void ShowLine()
    {
        aim.enabled = true;
    }
}
