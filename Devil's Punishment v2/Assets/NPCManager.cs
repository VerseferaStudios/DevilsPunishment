﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{

    public List<NPC_Stats> NPClist = new List<NPC_Stats>();
    public static NPCManager instance;
    public List<FactionRelations> Faction_Relations = new List<FactionRelations>();

    public GameObject waypointHolder;
    public List<GameObject> MapWaypoints;

    

    public List<Vector3> MapWaypointsPosition;

    public void addMapWaypoint(Vector3 point)
    {
        GameObject n = new GameObject("Point " + MapWaypoints.Count + point);
        n.transform.position = point;
        n.transform.parent = waypointHolder.transform;
        MapWaypoints.Add(n);
        MapWaypointsPosition.Add(point);
    }

    public bool spawnGenParts = true;
    public List<GameObject> genParts = new List<GameObject>();

    //TODO: Use refs instead of hardcode
    public GameObject genPartAPrefab;
    public GameObject genPartBPrefab;
    public GameObject genPartCPrefab;


    public void OnMapGenerationDone()
    {
        //Assign the randomly generated waypoints to the parasites
        foreach (NPC_Stats s in NPClist)
        {
            //  s.npc.allWaypoints.AddRange(MapWaypointsPosition);
            //Overwriting
            s.npc.allWaypoints = MapWaypointsPosition;
        }

        // Let's pick some locations and spawn map gen parts
        // TODO: Change location of map gen spawning..
        GameObject genA = Instantiate(genPartAPrefab,MapWaypointsPosition[Random.Range(0,MapWaypointsPosition.Count-1)]+Vector3.up, Quaternion.identity);
        GameObject genB = Instantiate(genPartBPrefab, MapWaypointsPosition[Random.Range(0, MapWaypointsPosition.Count-1)]+Vector3.up, Quaternion.identity);
        GameObject genC = Instantiate(genPartCPrefab, MapWaypointsPosition[Random.Range(0, MapWaypointsPosition.Count-1)]+ Vector3.up, Quaternion.identity);

        genParts.Add(genA);
        genParts.Add(genB);
        genParts.Add(genC);


    }



    void Awake()
    {
        
        Faction_Relations.Add(new FactionRelations(Faction.Horde, Faction.NightCrawlers, Relation.Enemy));

        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
    }


    public void addNPC(NPC_Stats info)
    {
        NPClist.Add(info);
        Debug.Log("NPC " + name + " awoke!");
    }

    public Relation getRelation(Faction me, Faction stranger)
    {
        foreach (FactionRelations FR in Faction_Relations)
        {
            if (FR.Faction_A == me || FR.Faction_B == me)
            {
                return FR.Relation_Factions;
            }
        }
        return Relation.Neutral; //if they dont care bout each other
    }


    public void alertClosestNPCs(float range, Transform who)
    {
        foreach (NPC_Stats info in NPClist)
        {
            if (Vector3.Distance(who.position, info.npc.transform.position) < info.alertRange)
            {

                info.npc.alertNPC(who);
                Debug.Log("NPC " + info.Name + " alerted!");
            }
        }
    }



}

