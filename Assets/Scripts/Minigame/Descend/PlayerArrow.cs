using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    public GameObject targetPlayer;

    private void Update()
    {
        if (targetPlayer == null)
        {
            Destroy(gameObject);
            return;
        }
        if (targetPlayer != null)
            gameObject.transform.position = new Vector2(targetPlayer.transform.position.x, targetPlayer.transform.position.y + 1);
        
    }
}
