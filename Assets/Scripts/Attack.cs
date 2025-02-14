using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Weapon
{
    public GameObject explosion;
    public Rigidbody2D rb;
    public float speed;
    
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5f);
    }


    private void FixedUpdate()
    {
        Vector2 direction = transform.position + transform.right * speed * Time.deltaTime;

        rb.MovePosition(direction);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            GameObject exp = Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(exp, 1f);

            Destroy(collision.gameObject);
            Destroy(gameObject);

        }
    }
}
