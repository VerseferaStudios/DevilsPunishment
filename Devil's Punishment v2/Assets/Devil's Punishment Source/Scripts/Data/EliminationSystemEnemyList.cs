using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Data/EnemyList")]
public class EliminationSystemEnemyList : ScriptableObject
{   

    [SerializeField] List<CreatureHealthData> creatureHealthDatas = new List<CreatureHealthData>();
    [System.NonSerialized]
    List<TestEnemy> enemyList = new List<TestEnemy>();

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
        IncreaseHealthOfAliveUndamagedCreatures();
    }

    private void IncreaseHealthOfAliveUndamagedCreatures()
    {
        foreach(TestEnemy enemy in enemyList)
        {
            var health = enemy.GetComponent<CreatureHealth>();
            Debug.Log(health.GetCurrentHealth());
            Debug.Log(health.GetMaxHealth());

            if(health.GetCurrentHealth() == health.GetMaxHealth())
            {
                health.SetCurrentHealth(health.GetCreatureHealthData().currentMaxHealth);
                health.SetMaxHealth(health.GetCreatureHealthData().currentMaxHealth);
            }
        }
    }
}
