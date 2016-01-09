using UnityEngine;
using System.Collections;

public abstract class QuestTaskOverlayItem : MonoBehaviour, IPoolable
{

    protected Camera MinimapCamera { get; set; }
    protected Camera MainGameCamera { get; set; }

    public Quest Quest { get; set; }

    public virtual void Initialise()
    {
        MinimapCamera = QuestTasksOverlayCamera.Instance.GetComponent<Camera>();
        MainGameCamera = GameMainReferences.Instance.RTSCamera.GetComponent<Camera>();
    }

    public virtual void PoolStart()
    {
      //  QuestGUI.Instance.OnSelectedQuestChanged += QuestChanged;
        QuestManager.Instance.OnTrackedQuestChanged += QuestChanged;
    }

    public virtual void Recycle()
    {
        //   QuestGUI.Instance.OnSelectedQuestChanged -= QuestChanged;
        QuestManager.Instance.OnTrackedQuestChanged -= QuestChanged;
    }

    protected virtual void LateUpdate() { }

    private void QuestChanged(Quest quest)
    {
        // If the quest is changed return this to the pool.
        DeRegisterQuestTaskOverlay();
    }

    public void DeRegisterQuestTaskOverlay()
    {
        QuestTasksPool.Instance.PoolObject(this);
    }


}
