using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackParameters : MonoBehaviour
{
    public enum AttackType{
        High,
        Mid,
        Low
    }

    public AttackType attackType;
    public float damage;
    public float waitFrames;
    public float selfStunFrames;
    public float enemyStunFrames;
    public Vector2 attackForceLeft;
    public Vector2 attackForceRight;
}