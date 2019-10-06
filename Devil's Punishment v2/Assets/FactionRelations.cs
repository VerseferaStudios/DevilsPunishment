using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FactionRelations : MonoBehaviour
{

    public Faction Faction_A;
    public Faction Faction_B;
    public Relation Relation_Factions;

    public FactionRelations(Faction factionA, Faction factionB, Relation relation)
    {
        Faction_A = factionA;
        Faction_B = factionB;
        Relation_Factions = relation;
    }
}


public enum Relation
{
    Friendly,
    Neutral,
    Enemy
}

