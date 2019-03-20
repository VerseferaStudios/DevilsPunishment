using UnityEngine;
using System.Collections;

public class EliminationSystem : MonoBehaviour
{
    [SerializeField] EnemyList enemyList = null;


    public void EngageSystem()
    {
        enemyList.OnPlayerDeath();
    }
}
