using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Make the Entity say something.")]
[TaskCategory("Spell Game/Basic")]
public class EntitySpeak : EntityAction
{
    public SharedString[] playTexts;

    public override void OnStart()
    {
        if (Entity != null)
        {
            foreach (var sharedString in playTexts)
            {
                Entity.SpeechBubbleProxy.QueueText(sharedString.Value);
            }
        }
    }

}
