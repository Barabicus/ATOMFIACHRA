using UnityEngine;
using System.Collections;

public abstract class TimedUpdateableEffect : SpellEffect
{
    public bool singleShot = false;
    [Tooltip("If this is true the timer assoicated with this effect will instantly be set to tick when the spell starts. If not the spell will have to wait for the updateDelay frequency in order to tick.")]
    public bool ticksOnStart = false;
    public float updateDelay = 2f;
    private Timer timer;
    private bool r_hasFired;

    protected override void OnSpellStart()
    {
        base.OnSpellStart();
        r_hasFired = false;
        timer = new Timer(updateDelay);
        enabled = true;
        if (ticksOnStart)
        {
            timer.ForceTickTime();
        }
    }

    protected override void Update()
    {
        if (OnlyUpdateOnSpellEnabled)
            return;

        if (!r_hasFired && timer.CanTickAndReset())
        {
            if (singleShot)
            {
                UpdateSpell();
                //enabled = false;
                r_hasFired = true;
            }
            else
            {
                UpdateSpell();
            }
        }

        CurrentLivingTime += Time.deltaTime;
    }

}
