using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    //variables
    public float speed;
    public float maxHealth;

    public enum PlayerNumber
    {
        Player1,
        Player2
    }

    public PlayerNumber PlayerNum;

    public Animator chrAnimation;

    Rigidbody2D rb;
    BoxCollider2D bound;
        
    //attacks
    public GameObject attack1;
    public GameObject attack2;
    public GameObject attack3;

    AttackParameters atk1;
    AttackParameters atk2;
    AttackParameters atk3;

    AttackParameters attackParams;

    SoundDB soundDB;
    
    //states
    [SerializeField] private bool attacking = false;
    [SerializeField] private bool stunned = false;
    [SerializeField] private bool crouched = false;
    [SerializeField] private bool grounded = true;
    [SerializeField] private bool moving = false;
    [SerializeField] private bool dead = false;

    //facing
    [SerializeField] private bool facingRight;
    [SerializeField] private bool facingLeft;

    private float frame = 0;
    private float crouchSpeed;
    private float runSpeed;
    private float currentHP;
    private float moveHorizontal;

    private Slider healthBar;

    //used when instantiating an attack
    private GameObject move;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bound = GetComponent<BoxCollider2D>();       
        chrAnimation = GetComponent<Animator>();

        soundDB = GetComponent<SoundDB>();

        crouchSpeed = speed / 2;
        runSpeed = speed;
        currentHP = maxHealth;

        healthBar = GameObject.Find(PlayerNum + "HP").GetComponent<Slider>();

        atk1 = attack1.GetComponent<AttackParameters>();
        atk2 = attack2.GetComponent<AttackParameters>();        
        atk3 = attack3.GetComponent<AttackParameters>();
    }

    void Update()
    {
        frame += 1;

        healthBar.value = currentHP / maxHealth;

        //check if character is facing left or right
        SpriteHandler.spriteHandler.characterDirection(facingLeft, facingRight, this.gameObject);

        //set sprite hitbox
        if (!dead)
        {
            SpriteHandler.spriteHandler.setHitbox(this.gameObject);
        }
        else{
            bound.enabled = false;
            rb.gravityScale = 0.01f;
        }
        

        //if stunned, character can't move or attack
        if (stunned || dead)
        {
            return;
        }

        Death();

        if (rb.velocity.x == 0 && rb.velocity.y == 0 && !crouched && !dead)
        {
            SpriteHandler.spriteHandler.SetAnimation(chrAnimation, 0);
        }

        Crouch();
        Jump();
        MoveHorizontal();
        StopHorizontal();
        Fire1();
        Fire2();
    }

    private void Death()
    {
        if (currentHP <= 0)
        {
            dead = true;
            SpriteHandler.spriteHandler.SetAnimation(chrAnimation, 9);
            SoundHandler.soundHandler.PlaySound(SoundHandler.soundHandler.RandomSound(soundDB.deathSounds));
        }
    }

    private void Fire1()
    {
        if (Input.GetButton(PlayerNum + "Fire1"))
        {
            //can't attack if already attacking
            if (!attacking)
            {
                attacking = true;
                StartCoroutine(fireAttack(attack1));
                // formula:
                // chrAnimation.speed = 1 = 100% speed
                // chrAnimation.speed = s * y
                // s = (frames in animation/attack.waitFrames)
                // we want the attack to fire halfway through the animation thus y = 2
                // slow down the animation to account for how long the wait is to find s
                chrAnimation.speed = (10 / atk1.waitFrames) * 2;
                SpriteHandler.spriteHandler.SetAnimation(chrAnimation, 3);
            }
        }
    }

    private void Fire2()
    {
        if (Input.GetButton(PlayerNum + "Fire2"))
        {
            if (!attacking)
            {
                attacking = true;
                chrAnimation.speed = (8/ atk2.waitFrames) * 2;
                SpriteHandler.spriteHandler.SetAnimation(chrAnimation, 8);
                StartCoroutine(fireAttack(attack2));
            }
        }
    }

    private void StopHorizontal()
    {
        if (Input.GetButtonUp(PlayerNum + "Horizontal"))
        {
            rb.velocity = new Vector3(0.0f, rb.velocity.y, 0.0f);
        }
    }

    private void MoveHorizontal()
    {
        if (Input.GetButton(PlayerNum + "Horizontal"))
        {
            moving = true;
            moveHorizontal = Input.GetAxis(PlayerNum + "Horizontal");
            if (grounded && !attacking && !crouched)
            {
                SpriteHandler.spriteHandler.SetAnimation(chrAnimation, 1);
            }

            if (crouched && !attacking)
            {
                SpriteHandler.spriteHandler.SetAnimation(chrAnimation, 5);
            }
           

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

            Vector3 movement = new Vector3(moveHorizontal * runSpeed, rb.velocity.y, 0.0f);

            rb.velocity = movement;
        }
        else
        {
            moving = false;
        }
    }

    private void Jump()
    {
        if (Input.GetButton(PlayerNum + "Jump") && grounded)
        {
            grounded = false;
            SpriteHandler.spriteHandler.SetAnimation(chrAnimation, 2);
            Vector3 jump = new Vector3(rb.velocity.x, 4f, 0.0f);
            rb.velocity = jump;
        }
    }

    private void Crouch()
    {
        if (Input.GetButton(PlayerNum + "Crouch"))
        {
            crouched = true;
            runSpeed = crouchSpeed;

            if (Input.GetButton(PlayerNum + "Fire1"))
            {
                attacking = true;                
                chrAnimation.speed = (8 / atk3.waitFrames) * 2;
                SpriteHandler.spriteHandler.SetAnimation(chrAnimation, 7);
                StartCoroutine(fireAttack(attack3));
            }

            if (!moving && !attacking)
            {
                SpriteHandler.spriteHandler.SetAnimation(chrAnimation, 4);
            }                                   
        }
        else
        {
            crouched = false;
            runSpeed = speed;
        }
    }

    void FixedUpdate()
    {
        stopSlide();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != PlayerNum + "Attack")
        {
            SpriteHandler.spriteHandler.SetAnimation(chrAnimation, 6);
            AttackParameters collisionParams = collision.GetComponent<AttackParameters>();
            //get frame character was hit on
            float hitFrame = frame;
            float framesToWait = collisionParams.enemyStunFrames;

            stunned = true;
            attacking = false;

            //remove all velocity to prevent sliding in air
            rb.velocity = Vector3.zero;

            currentHP = currentHP - collisionParams.damage;
            float hit = collision.transform.position.x;

            
            SoundHandler.soundHandler.PlaySound(SoundHandler.soundHandler.RandomSound(soundDB.hitSounds));

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
            //print("stopped slide");
        }
    }

    public IEnumerator resetStates(float frameStart, float stunnedFrames)
    {        
        yield return new WaitWhile(() => frameStart > frame - stunnedFrames);
        chrAnimation.speed = 1;
     
        if (crouched)
        {
            //crouch sprite
            SpriteHandler.spriteHandler.SetAnimation(chrAnimation, 4);
        }
        else
        {
            //idle sprite
            SpriteHandler.spriteHandler.SetAnimation(chrAnimation, 0);
        }

        stunned = false;
        attacking = false;
    }

    public IEnumerator fireAttack(GameObject attack)
    {
        //prevent slide attack
        rb.velocity = new Vector3(0, rb.velocity.y, 0.0f);
        stunned = true;
        attackParams = attack.GetComponent<AttackParameters>();

        float attackTime = attackParams.waitFrames;
        float stunTime = attackParams.selfStunFrames;
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
        SoundHandler.soundHandler.PlaySound(SoundHandler.soundHandler.RandomSound(soundDB.attackSounds));

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
        rb.velocity = new Vector3(0.0f, rb.velocity.y, 0.0f);
        //set new start frame
        startFrame = frame;
        
        /*handled by autodestroy.cs
            all moves destroyed after 10 frames
            yield return new WaitWhile(() => startFrame > frame - 10);
            Destroy(move);
        */

        //wait before stun is removed
        //yield return new WaitWhile(() => startFrame > frame - stunTime);
        StartCoroutine(resetStates(startFrame, stunTime));
    }
}