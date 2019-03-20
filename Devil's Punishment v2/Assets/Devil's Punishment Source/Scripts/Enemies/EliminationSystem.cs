using UnityEngine;
using System.Collections.Generic;

public class EliminationSystem : MonoBehaviour
{
    public static EliminationSystem instance = null;
    [SerializeField] List<CreatureHealthData> creatureHealthDatas = new List<CreatureHealthData>();
    public List<TestEnemy> enemyList = new List<TestEnemy>();

    private void Awake()
    {
        instance = this;
    }

    public void AddEnemyToList(TestEnemy enemy)
    {
        enemyList.Add(enemy);
    }

    public void RemoveEnemyFromList(TestEnemy enemy)
    {
        enemyList.Remove(enemy);
    }

    public void OnPlayerDeath()
    {
        foreach(CreatureHealthData data in creatureHealthDatas)
        {
            data.IncreaseMaxHealth();
        }
    }
}
