using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Data/EnemyList")]
public class EnemyList : ScriptableObject, ISerializationCallbackReceiver
{   

    [SerializeField] List<CreatureHealthData> creatureHealthDatas = new List<CreatureHealthData>();
    [SerializeField] List<TestEnemy> emptyList = new List<TestEnemy>();
    [System.NonSerialized]
    List<TestEnemy> enemyList = new List<TestEnemy>();

    public void OnBeforeSerialize() {  }
    public void OnAfterDeserialize() { enemyList = emptyList; }

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
