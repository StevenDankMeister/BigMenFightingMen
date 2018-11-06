using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {


    //variables
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
    AttackParameters atk1;
    
    private bool grounded = true;

    //states
    private bool attacking = false;
    private bool stunned = false;

    private bool facingRight = true;
    private bool facingLeft;

    private float frame = 0;

    //used when instantiating an attack
    private GameObject move;

    void Start () {
        rb = GetComponent<Rigidbody2D>();
        bound = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        //Initialize all attacks
        atk1 = attack1.GetComponent<AttackParameters>();
	}

    void Update()
    {
        frame += 1;
    }

    void FixedUpdate () {
        //check if character is facing left or right
        mirrorSprite();

        //if stunned, character can't move or attack
        if (stunned)
        {
            return;
        }

        if (Input.GetButton(PlayerNum + "Jump") && grounded)
        {
            grounded = false;
            Vector3 jump = new Vector3(rb.velocity.x, 3f, 0.0f);
            rb.velocity = jump;
        }

        if (Input.GetButton(PlayerNum + "Horizontal"))
        {
            print("moving");
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
            //can't attack if already attacking
            if (!attacking)
            {
                attacking = true;
                StartCoroutine(fireAttack(attack1));
                spriteRenderer.sprite = punch1;
            }
        }
	}
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != PlayerNum+"Attack")
        {
            AttackParameters collisionParams = collision.GetComponent<AttackParameters>();
            //get frame character was hit on
            float hitFrame = frame;
            float framesToWait = collisionParams.enemyStunFrames;

            stunned = true;
            attacking = false;

            //remove all velocity to prevent sliding in air
            rb.velocity = Vector3.zero;

            //get attack params

            health = health - collisionParams.damage;
            float hit = collision.transform.position.x;

            if (hit > rb.transform.position.x)
            {
                rb.AddForce(collisionParams.attackForceLeft * 50f);
            }
            else if (hit < rb.transform.position.x)
            {
                rb.AddForce(collisionParams.attackForceRight * 50f);
            }

            StartCoroutine(resetStates(hitFrame, framesToWait));
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = true;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            grounded = false;
        }
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

    public IEnumerator resetStates(float frameStart, float stunnedFrames)
    {
        print("test");
        yield return new WaitWhile(() => frameStart > frame - stunnedFrames);
        
        stunned = false;
    }

    public IEnumerator fireAttack(GameObject attack)
    {
        stunned = true;

        float attackTime = atk1.waitFrames;
        float stunTime = atk1.selfStunFrames;
        float startFrame = frame;

        //wait before attack is executed
        yield return new WaitWhile(() => startFrame > frame - attackTime);

        //make sure character is still eligible to attack
        //in the instance character has been attacked before own attack is executed
        //attack is cancelled
        if (!attacking)
        {
            print("attack cancelled");
            yield break;
        }

        print("starting attack");
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
        //set new start frame
        startFrame = frame;
        //all moves destroyed after 10 frames
        yield return new WaitWhile(() => startFrame > frame - 10);
        Destroy(move);

        //wait before stun is removed
        yield return new WaitWhile(() => startFrame > frame - stunTime);
        stunned = false;
        attacking = false;
        spriteRenderer.sprite = idle;
    }
}