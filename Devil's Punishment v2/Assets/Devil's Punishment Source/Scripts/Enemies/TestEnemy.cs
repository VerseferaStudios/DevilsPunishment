using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class TestEnemy : MonoBehaviour
{

    [SerializeField] CreatureHealthData HealthData = null;
    int maxHealth = 2;
    int currentHealth = 10;
    [SerializeField] TextMesh text = null;

    public UnityEvent onPlayerDeath = null; //This was just for testing Elimination system. Should be on player instead / SkitzFist

    private void Start()
    {
        maxHealth = HealthData.currentMaxHealth;
        currentHealth = maxHealth;
        RefreshText();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        RefreshText();
        if (currentHealth <= 0)
        {
            EliminationSystem.instance.RemoveEnemyFromList(this);
            onPlayerDeath.Invoke();
            Destroy(gameObject);
        }
    }

    private void RefreshText()
    {
        text.text = currentHealth.ToString();
    }
}
