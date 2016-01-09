using UnityEngine;
using System.Collections;

public interface IQuestComponent
{
    Quest Quest { get; set; }

    void OnQuestAdded(Quest quest);
    void OnQuestCompleted(Quest quest);
    void OnQuestUpdate(Quest quest);
    void OnQuestSelected(Quest quest);
    void OnQuestReset(Quest quest);

}
