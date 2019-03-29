using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class TestEnemy : MonoBehaviour
{
    CreatureHealth health = null;

    private void Start()
    {
        health = GetComponent<CreatureHealth>();
    }

    public void TakeDamage(int amount)
    {
        health.TakeDamage(amount);
    }


}
