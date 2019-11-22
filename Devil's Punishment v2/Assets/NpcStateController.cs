
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class NpcStateController : MonoBehaviour
{
    //NPC INFO


    public NPC_Stats npc_info;

    private NavMeshAgent agent;
    private Animator animo;

    public List<GameObject> WaypointList;
    public List<Vector3> allWaypoints;
    public bool patroling = true;

    public int currentWaypoint = 0;
    public int maxWaypoints;

    bool configured = false;


    public GameObject name_GUI;


    public Transform target;
    public float alertRange = 25f;

    // Start is called before the first frame update
    void Start()
    {
        npc_info = transform.GetComponent<NPC_Stats>();
        npc_info.npc = this;

        NPCManager.instance.addNPC(npc_info);
        configured = true;

        agent = transform.GetComponent<NavMeshAgent>();
        animo = transform.GetComponent<Animator>();


        GameObject nameTag = Instantiate(name_GUI, transform);
        if(transform.name == "night terror")
        {
            nameTag.GetComponent<TextMeshProUGUI>().fontSize = .25f;
        }
        nameTag.GetComponent<TextMeshProUGUI>().text = npc_info.Name;
        nameTag.transform.position = transform.position + new Vector3(0, 2, 0);
        //   nameTag.transform.LookAt(PlayerManager.instance.Player.transform.position);
        nameTag.GetComponent<TextMeshProUGUI>().color = Color.green;

        //  nameTag.GetComponent<TextMeshPro>().color =
        if (npc_info.faction == Faction.NightCrawlers)
        {
            nameTag.GetComponent<TextMeshProUGUI>().color = Color.blue;
        }
        else if (npc_info.faction == Faction.Horde)
        {
            nameTag.GetComponent<TextMeshProUGUI>().color = Color.red;
        }


        
        int count = 0;
        foreach (GameObject List in WaypointList)
        {
            foreach (Transform waypoint in List.transform)
            {
                Debug.Log(waypoint.position);
                allWaypoints.Add(waypoint.position);
                count++;
            }
        }

        maxWaypoints = count;
        //    NPC_Stats info = new NPC_Stats(name, npc_info.faction, this, alertRange);

        //   npc_info = info;



    }



    public int shootRange = 0;
    void OnTriggerEnter(Collider other)
    {
        //  Debug.Log(other.name);
        // Debug.Log(other.gameObject.name);
        if (other.gameObject.tag == "NPC")
        {


           
        }
    }







    public void alertNPC(Transform targetS)
    {
        patroling = false;
       // following = true;
        animo.SetBool("patroling", false);
        animo.SetBool("following", true);
        following = true;
        target = targetS;
        Vector3 dir = target.transform.position - transform.position;
        agent.SetDestination(target.position - dir * followDistance);

        animo.SetLayerWeight(animo.GetLayerIndex("Base Layer"), 1);

        agent.speed = 4;


        // run = RunState.Run;
      //  animo.SetInteger("RunIntensity", 4);

        startFollowing(50f);
    }



    public Vector3 targetWaypoint;
    public float attackRange = .1f;
    public void startPatroling(float time)
    {
        if (!following)
        {
            animo.ResetTrigger(0);
            patroling = true;
            following = false;

            //  animo.SetInteger("RunIntensity", 2);


            int randomPatrol = Random.Range(0, maxWaypoints - 1);
            targetWaypoint = allWaypoints[randomPatrol];
            agent.SetDestination(targetWaypoint);

            IEnumerator c = Patroling(time);
            StartCoroutine(c);

        }
        else if(Vector3.Distance(transform.position,target.position) < attackRange )
        {
            patroling = false;
            animo.SetTrigger(0); // ATTACK
        }
    }

    public bool following = false;

    public void startFollowing(float time)
    {

        following = true;
        if (target)
        {
            Vector3 dir = target.transform.position - transform.position;

            agent.SetDestination(target.position - dir.normalized * shootRange);

          //  animo.SetInteger("RunIntensity", 1);
        }

        IEnumerator c = Following(time);
        StartCoroutine(c);

    }

    private IEnumerator Patroling(float time)
    {
        // print("refollowing");
        if (!animo.GetBool(0))
        {

            if (agent.remainingDistance < 2f)
            {
                int randomPatrol = Random.Range(0, maxWaypoints - 1);
                targetWaypoint = allWaypoints[randomPatrol];
                agent.SetDestination(targetWaypoint);


            }
            else
            {
                //  animo.SetInteger("RunIntensity", );
                //     animo.SetInteger("RunIntensity", 1);
                agent.SetDestination(targetWaypoint);
            }

            yield return new WaitForSeconds(time);

            if (patroling)
            {

                IEnumerator c = Patroling(time);
                StartCoroutine(c);
            }

        }



    }

    public float followDistance = .01f;
    private IEnumerator Following(float time)
    {
        // print("refollowing");
        Vector3 dir = target.position - transform.position;

        if (agent.remainingDistance < attackRange)
        {
            Debug.Log("ATTACK!");
            animo.SetTrigger(0);
            agent.SetDestination(target.position - dir.normalized * followDistance);
           // animo.SetInteger("RunIntensity", 0);
        }
        else
        {
            //  animo.SetInteger("RunIntensity", 1);
            animo.ResetTrigger(0);
            agent.SetDestination(target.position - dir.normalized * followDistance);
        }

        yield return new WaitForSeconds(time);

        if (following)
        {
            IEnumerator c = Following(time);
            StartCoroutine(c);
        }





    }

    // Update is called once per frame
    void Update()
    {

    }
}
