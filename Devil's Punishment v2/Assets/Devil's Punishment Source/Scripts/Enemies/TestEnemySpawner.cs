using UnityEngine;
using System.Collections;

public class TestEnemySpawner : MonoBehaviour
{
    [Header("General")]
    [SerializeField] int amount = 2;
    [SerializeField] TestEnemy prefab = null;
    Vector3 startPos = new Vector3(0f,0f,0f);
    [SerializeField] Transform parent = null;
    [Header("Elimination System")]
    [SerializeField] EnemyList enemyList = null; //when creating a new spawner, make sure to include this!
    public void SpawnEnemies()
    {
        for (int i = 0; i < amount; ++i)
        {
            startPos.x += i + 2;
            TestEnemy newEnemy = Instantiate(prefab, startPos, Quaternion.identity);
            newEnemy.transform.SetParent(parent);
            enemyList.AddEnemyToList(newEnemy); //when creating a new spawner, make sure to include this!
        }
    }
}
