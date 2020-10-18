using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Stats : MonoBehaviour
{
    public string Name;
    public Faction faction;
    public List<Abilitys> abilitys = new List<Abilitys>();
    public NpcStateController npc;
    public float alertRange = 3f;

    public NPC_Stats(string name, Faction fac, NpcStateController npcc, float range)
    {
        Name = name;
        faction = fac;
        npc = npcc;
        alertRange = range;
    }
}

public enum Abilitys
{
    Attack,
    Defend,
    Flee,
    Run,
    Dig,
    Shoot,
    Protect,
    Farm,
    HealOther,
    HealSelf,
    PlayDead,
    Loot,
    Rampage,
    Walk,
    Sneak,

}

public enum Faction
{
    NightCrawlers,
    Nature,
    Player,
    Horde
}

