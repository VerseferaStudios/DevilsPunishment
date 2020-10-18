using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBehaviour : StateMachineBehaviour
{
    NpcStateController npc;

    bool setup = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        npc = animator.transform.GetComponent<NpcStateController>();
        if (npc != null)
        {
            float t = Random.Range(5, 120);
            npc.startFollowing(t);
            setup = true;
        }


        //  base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // base.OnStateUpdate(animator, stateInfo, layerIndex);    
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (setup)
        {
            npc.following = false;
        }

        // base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
