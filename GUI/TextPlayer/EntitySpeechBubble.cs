using UnityEngine;
using System.Collections;

public class EntitySpeechBubble : TextPlayer
{

    [SerializeField]
    private Transform _container;

    public Entity Entity { get; set; }

    private void LateUpdate()
    {
        if (Entity != null)
            _container.position = RectTransformUtility.WorldToScreenPoint(Camera.main, Entity.GUISpeechPoint.position);
        if (Entity.LivingState != EntityLivingState.Alive)
        {
            SpeechBubblePool.Instance.PoolObject(this);
        }
    }

    protected override void FinishedPlaying()
    {
        SpeechBubblePool.Instance.PoolObject(this);
    }
}
