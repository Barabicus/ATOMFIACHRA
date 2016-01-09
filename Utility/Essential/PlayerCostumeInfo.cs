using System.Collections.Generic;
using UnityEngine;

public class PlayerCostumeInfo : ScriptableObject
{
    [SerializeField]
    private PlayerCostume[] _costumes;

    public PlayerCostume[] Costumes
    {
        get { return _costumes; }
    }

    public Dictionary<string, PlayerCostume> CostumeDictionary
    {
        get
        {
            var costumeDictionary = new Dictionary<string, PlayerCostume>();
            foreach (PlayerCostume costume in Costumes)
            {
                if (!costumeDictionary.ContainsKey(costume.CostumeID))
                    costumeDictionary.Add(costume.CostumeID, costume);
                else
                    Debug.LogWarning("Costume already exists: " + costume.CostumeID);
            }
            return costumeDictionary;
        }
    } 

}
