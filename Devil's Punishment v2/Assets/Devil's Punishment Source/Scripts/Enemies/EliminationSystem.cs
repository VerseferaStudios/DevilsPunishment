using UnityEngine;
using System.Collections;

public class EliminationSystem : MonoBehaviour
{
    [SerializeField] EliminationSystemEnemyList enemyList = null;

    public void EngageSystem()
    {
        enemyList.OnPlayerDeath();
    }
}
