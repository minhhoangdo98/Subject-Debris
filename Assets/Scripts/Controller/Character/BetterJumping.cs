using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterJumping : MonoBehaviour
{
    float fallgravity = 2.5f, upgravity = 2f;

    Rigidbody2D r2d;

    void Start()
    {
        r2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (r2d.velocity.y < 0)
        {
            r2d.velocity += Vector2.up * Physics2D.gravity.y * (fallgravity - 1) * Time.deltaTime;
        }
        else if (r2d.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            r2d.velocity += Vector2.up * Physics2D.gravity.y * (upgravity - 1) * Time.deltaTime;
        }

    }
}
