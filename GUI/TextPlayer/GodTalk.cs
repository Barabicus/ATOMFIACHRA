using UnityEngine;
using System.Collections;

public class GodTalk : TextPlayer
{

    public static GodTalk Instance { get; set; }

    protected void Awake()
    {
        Instance = this;
        Initialise();
        PoolStart();
    }

}
