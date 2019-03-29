using UnityEngine;
using UnityEngine.Events;
public class CreatureHealth : MonoBehaviour
{
    [SerializeField] EliminationSystemEnemyList eliminationSystem = null;
    private TestEnemy thisEnemy = null;
    [SerializeField] CreatureHealthData HealthData = null;
    int maxHealth = 0;
    int currentHealth = 0;

    [SerializeField] TextMesh text = null;

    public UnityEvent onPlayerDeath = null; //This was just for testing Elimination system. Should be on player instead / SkitzFist

    #region Setters & Getters

    public int GetCurrentHealth() { return currentHealth;  }
    public void SetCurrentHealth(int value) { currentHealth = value; RefreshText(); }

    public int GetMaxHealth() { return maxHealth; }
    public void SetMaxHealth(int value) { maxHealth = value; RefreshText(); }

    public CreatureHealthData GetCreatureHealthData() { return HealthData; }
    #endregion Setters & Getters

    private void Start()
    {
        thisEnemy = GetComponent<TestEnemy>();
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
            eliminationSystem.RemoveEnemyFromList(thisEnemy);
            onPlayerDeath.Invoke();
            Destroy(gameObject);
        }
    }

    private void RefreshText()
    {
        text.text = currentHealth.ToString();
    }
}
