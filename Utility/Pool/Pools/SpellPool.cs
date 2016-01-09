using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellPool : IDListPool<Spell>
{
    protected override Dictionary<string, Spell> ConstructedPrefabs
    {
        get { //return Resources.LoadAll<SpellListInfo>("Utility")[0].SpellDictionary;
            return SpellList.Instance.SpellDictionary;
        }
    }
}
