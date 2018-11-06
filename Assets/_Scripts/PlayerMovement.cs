using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float speed;
    public float health;

    public enum PlayerNumber{
        Player1,
        Player2
    }

    public PlayerNumber PlayerNum;

    public GameObject attack1;
    public Sprite idle;
    public Sprite punch1;

    Rigidbody2D rb;
    Collider2D bound;
    SpriteRenderer spriteRenderer;

    private bool ground = true;
    private bool attacking;
    private bool facingRight = true;
    private bool facingLeft;

    private GameObject move;

    void Start () {
        rb = GetComponent<Rigidbody2D>();
        bound = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void FixedUpdate () {
        mirrorSprite();

        if (Input.GetButton(PlayerNum + "Jump"))
        {
            if (ground)
            {
                ground = false;
                Vector3 jump = new Vector3(rb.velocity.x, 5f, 0.0f);

                rb.velocity = jump;
            }          
        }

        if (Input.GetButton(PlayerNum + "Horizontal"))
        {
            float moveHorizontal = Input.GetAxis(PlayerNum + "Horizontal");

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

            Vector3 movement = new Vector3(moveHorizontal * speed, rb.velocity.y, 0.0f);

            rb.velocity = movement;
        }

        if(Input.GetButtonUp(PlayerNum + "Horizontal"))
        {
            rb.velocity = new Vector3(0.0f, rb.velocity.y, 0.0f);
        }

        if (Input.GetButton(PlayerNum + "Fire1"))
        {
            if (!attacking)
            {
                attacking = true;
                spriteRenderer.sprite = punch1;
                StartCoroutine(fireAttack(attack1));
            }
        }
	}

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != PlayerNum+"Attack")
        {
            health = health - 1;
            double hit = collision.transform.position.x;

            if(hit > rb.transform.position.x)
            {
                rb.AddForce(new Vector2(-2f, 5f)*50f);
            }
            else if(hit < rb.transform.position.y)
            {
                rb.AddForce(new Vector2(2f,5f)*-50f);
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

    void createAttack(Vector3 pos, GameObject attack)
    {
        move = Instantiate(attack, pos, Quaternion.identity);
        move.tag = PlayerNum + "Attack";
    }

    public IEnumerator fireAttack(GameObject attack)
    {
        Vector3 playerpos = this.transform.position;
        Vector3 spawnPos;

        if (facingRight)
        {
            spawnPos = new Vector3(playerpos.x + bound.bounds.size.x, playerpos.y, playerpos.z);
            createAttack(spawnPos, attack);
            
        }
        else if (facingLeft)
        {
            spawnPos = new Vector3(playerpos.x + -bound.bounds.size.x, playerpos.y, playerpos.z);
            createAttack(spawnPos, attack);
        }

        yield return new WaitForSeconds(0.2f);
        Destroy(move);
        spriteRenderer.sprite = idle;
        attacking = false;
    }
}