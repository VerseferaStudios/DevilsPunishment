using UnityEngine;
using System.Collections;

[CreateAssetMenu (menuName = "Data/CreatureHealth")]
public class CreatureHealthData : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] int initMaxHealth = 10;
    [System.NonSerialized]
    public int currentMaxHealth = 0;

    [SerializeField] int maxHealthCap = 20;
    [Tooltip("This * currentMaxHealth = new max health on player death")]
    [SerializeField] float healthGrowth = 1.33f;

    public void OnBeforeSerialize() {  }
    public void OnAfterDeserialize() { currentMaxHealth = initMaxHealth; }

    public void IncreaseMaxHealth()
    {
        if(currentMaxHealth < maxHealthCap)
        {
            Debug.Log("Inside IncreaseMaxHealth");
            float newMaxHealth = currentMaxHealth * healthGrowth;
            int newMaxHealthRounded = Mathf.FloorToInt(newMaxHealth);
            if (newMaxHealthRounded <= maxHealthCap) { currentMaxHealth = newMaxHealthRounded; }
            else { currentMaxHealth = maxHealthCap; }
        }       
    }
}
