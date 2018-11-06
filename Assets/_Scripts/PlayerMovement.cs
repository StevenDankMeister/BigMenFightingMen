using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public double speed;
    public double health;
    public GameObject attack1;

    Rigidbody2D rb;
    Collider2D bound;
    SpriteRenderer spriteRenderer;

    private bool ground = true;
    private bool attacking;
    private bool facingRight;
    private bool facingLeft;

    private GameObject move;

    void Start () {
        rb = GetComponent<Rigidbody2D>();
        bound = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void FixedUpdate () {
        mirrorSprite();

        if (Input.GetButton("Jump"))
        {
            if (ground)
            {
                ground = false;
                Vector3 jump = new Vector3(rb.velocity.x, 10f, 0.0f);

                rb.velocity = jump;
            }          
        }

        if (Input.GetButton("Horizontal"))
        {
            float moveHorizontal = Input.GetAxis("Horizontal");

            if(moveHorizontal > 0)
            {
                facingRight = true;
                facingLeft = false;
            }
            else if(moveHorizontal < 0)
            {
                facingRight = false;
                facingLeft = true;                
            }

            Vector3 movement = new Vector3(moveHorizontal, rb.velocity.y, 0.0f);

            rb.velocity = movement;
        }

        if(Input.GetButtonUp("Horizontal"))
        {
            rb.velocity = new Vector3(0.0f, rb.velocity.y, 0.0f);
        }

        if (Input.GetButton("Fire1"))
        {
            if (!attacking)
            {
                attacking = true;
                StartCoroutine(fireAttack(attack1));
            }
        }
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        ground = true;
    }

    void mirrorSprite()
    {
        if (facingRight)
        {
            spriteRenderer.flipX = false;
        }
        else if (facingLeft)
        {
            spriteRenderer.flipX = true;
        }
    }

    public IEnumerator fireAttack(GameObject attack)
    {
        Vector3 playerpos = this.transform.position;
        

        if (facingRight)
        {
            Vector3 spawnpos = new Vector3(playerpos.x + bound.bounds.size.x, playerpos.y, playerpos.z);
            move = Instantiate(attack, spawnpos, Quaternion.identity);
        }
        else if (facingLeft)
        {
            Vector3 spawnpos = new Vector3(playerpos.x + -bound.bounds.size.x, playerpos.y, playerpos.z);
            move = Instantiate(attack, spawnpos, Quaternion.identity);
        }

        yield return new WaitForSeconds(0.2f);
        Destroy(move);
        attacking = false;
    }
}