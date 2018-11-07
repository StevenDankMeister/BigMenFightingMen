using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{


    //variables
    public float speed;
    public float health;

    public enum PlayerNumber
    {
        Player1,
        Player2
    }

    public PlayerNumber PlayerNum;

    
    public Sprite idle;
    public Sprite punch1;
    public Sprite crouch;

    Rigidbody2D rb;
    BoxCollider2D bound;
    SpriteRenderer spriteRenderer;
    
    //attacks
    public GameObject attack1;
    public GameObject attack2;

    AttackParameters atk1;
    AttackParameters atk2;

    private bool grounded = true;

    //states
    public bool attacking = false;
    public bool stunned = false;
    public bool crouched = false;

    //facing
    public bool facingRight;
    public bool facingLeft;

    private float frame = 0;

    //used when instantiating an attack
    private GameObject move;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bound = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        //Initialize all attacks
        atk1 = attack1.GetComponent<AttackParameters>();
        atk2 = attack2.GetComponent<AttackParameters>();
    }

    void Update()
    {
        frame += 1;
    }

    void FixedUpdate()
    {
        //check if character is facing left or right
        SpriteHandler.spriteHandler.characterDirection(facingLeft, facingRight, this.gameObject);
        
        //set sprite hitbox
        SpriteHandler.spriteHandler.setHitbox(this.gameObject);

        stopSlide();

        //if stunned, character can't move or attack
        if (stunned)
        {
            return;
        }

        if (Input.GetButton(PlayerNum + "Crouch"))
        {
            crouched = true;
            spriteRenderer.sprite = crouch;
        }
        else
        {
            crouched = false;
            spriteRenderer.sprite = idle;
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

            if (moveHorizontal > 0)
            {
                facingRight = true;
                facingLeft = false;
            }
            else if (moveHorizontal < 0)
            {
                facingRight = false;
                facingLeft = true;
            }

            Vector3 movement = new Vector3(moveHorizontal * speed, rb.velocity.y, 0.0f);

            rb.velocity = movement;
        }      

        if (Input.GetButtonUp(PlayerNum + "Horizontal"))
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

        if(Input.GetButton(PlayerNum + "Fire2"))
        {
            if (!attacking)
            {
                attacking = true;
                StartCoroutine(fireAttack(attack2));
                spriteRenderer.sprite = punch1;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != PlayerNum + "Attack")
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

            if (hit > this.transform.position.x)
            {
                rb.AddForce(collisionParams.attackForceLeft * 50f);
            }
            else if (hit < this.transform.position.x)
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
        if (collision.gameObject.tag == "Ground")
        {
            grounded = false;
        }
    }

    void stopSlide()
    {
        //todo: grounded
        if (!Input.GetButton(PlayerNum + "Horizontal") && !stunned)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0.0f);
            print("stopped slide");
        }
    }

    public IEnumerator resetStates(float frameStart, float stunnedFrames)
    {
        print("test");
        yield return new WaitWhile(() => frameStart > frame - stunnedFrames);

        stunned = false;
        if (crouched)
        {
            SpriteHandler.spriteHandler.setSprite(crouch, this.gameObject);
        }
        else
        {
            spriteRenderer.sprite = idle;
        }
    }

    public IEnumerator fireAttack(GameObject attack)
    {
        //prevent slide attack
        rb.velocity = new Vector3(0, rb.velocity.y, 0.0f);
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
        //Vector3 spawnPos = new Vector3(playerpos.x + bound.bounds.extents.x, playerpos.y, playerpos.z);
        Vector3 spawnPos = AttackHandler.attackHandler.SetAttackPos(attack, this.gameObject);

        if (facingLeft)
        {
            //spawn pos at other side
            spawnPos.x = playerpos.x - bound.bounds.extents.x;        
        }
        AttackHandler.attackHandler.CreateAttack(spawnPos, attack, PlayerNum + "Attack");
        //set new start frame
        startFrame = frame;
        
        /*handled by autodestroy.cs
            all moves destroyed after 10 frames
            yield return new WaitWhile(() => startFrame > frame - 10);
            Destroy(move);
        */

        //wait before stun is removed
        yield return new WaitWhile(() => startFrame > frame - stunTime);
        stunned = false;
        attacking = false;
        spriteRenderer.sprite = idle;
    }
}