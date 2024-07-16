using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorReset : StateMachineBehaviour
{
    [SerializeField] private string triggerName;

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        animator.ResetTrigger(triggerName);
    }
}
