using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapons : MonoBehaviour
{
    public GameObject enemy;
    public GameObject player;
    public Health hp;
    
    public void OnTriggerEnter(Collider claw)
    {
        hp.curHealth -= Random.Range(0, 100);
        hp.InfectChance();    
    }
}