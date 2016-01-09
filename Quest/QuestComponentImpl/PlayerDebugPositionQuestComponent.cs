using UnityEngine;
using System.Collections;
[QuestCategory("Player Debug Position", QuestCategory.Special)]
public class PlayerDebugPositionQuestComponent : QuestComponent
{
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
