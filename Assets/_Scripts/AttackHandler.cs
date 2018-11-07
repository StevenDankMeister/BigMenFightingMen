using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHandler : MonoBehaviour {

    public static AttackHandler attackHandler;

	// Use this for initialization
	void Start () {
        attackHandler = GetComponent<AttackHandler>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void createAttack(Vector3 attackPos, GameObject attackPrefab, string player)
    {
        GameObject attack = Instantiate(attackPrefab, attackPos, Quaternion.identity);
        attack.tag = player;
    }
}
