using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHandler : MonoBehaviour {

    public static AttackHandler attackHandler;
    Vector3 attackPos;

    // Use this for initialization
    void Start () {
        attackHandler = GetComponent<AttackHandler>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CreateAttack(Vector3 attackPos, GameObject attackPrefab, string player)
    {
        GameObject attack = Instantiate(attackPrefab, attackPos, Quaternion.identity);
        attack.tag = player;
    }

    public Vector3 SetAttackPos(GameObject attackPreFab, GameObject ch)
    {
        AttackParameters attackParams = attackPreFab.GetComponent<AttackParameters>();
        string attackType = attackParams.attackType.ToString();
        Vector3 playerPos = ch.transform.position;
        BoxCollider2D bound = ch.GetComponent<BoxCollider2D>();

        if(attackType == "High")
        {
            attackPos = new Vector3(playerPos.x + bound.bounds.extents.x, playerPos.y + bound.bounds.extents.y, playerPos.z);
        }
        else if (attackType == "Mid")
        {
            attackPos = new Vector3(playerPos.x + bound.bounds.extents.x, playerPos.y, playerPos.z);
        }
        else if (attackType == "Low")
        {
            attackPos = new Vector3(playerPos.x + bound.bounds.extents.x, playerPos.y - bound.bounds.extents.y, playerPos.z);
        }
        
        return attackPos;
    }
}
