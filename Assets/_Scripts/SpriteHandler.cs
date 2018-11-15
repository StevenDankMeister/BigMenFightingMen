using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteHandler : MonoBehaviour {

    public static SpriteHandler spriteHandler;

	void Start () {
        spriteHandler = GetComponent<SpriteHandler>();
	}
	
	public void characterDirection(bool facingLeft, bool facingRight, GameObject ch)
    {
        SpriteRenderer sprite = ch.GetComponent<SpriteRenderer>();

        if (facingLeft)
        {
            sprite.flipX = true;
        }
        else if (facingRight)
        {
            sprite.flipX = false;
        }
    }

    public void setHitbox(GameObject ch)
    {
        BoxCollider2D hitbox = ch.GetComponent<BoxCollider2D>();
        SpriteRenderer sprite = ch.GetComponent<SpriteRenderer>();

        Vector3 hitbox_size = sprite.sprite.bounds.size;
        hitbox.size = hitbox_size;
    }

    public void setSprite(Sprite sprite, GameObject ch)
    {
        SpriteRenderer s = ch.GetComponent<SpriteRenderer>();
        s.sprite = sprite;
    }

    public void SetAnimation(Animator animator, int animation)
    {
        animator.SetInteger("States", animation);
        //print(animator.GetInteger("States"));
    }
}
