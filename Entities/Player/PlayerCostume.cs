using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class PlayerCostume : MonoBehaviour
{
    [SerializeField]
    private string _costumeID;

    [SerializeField]
    private string _costumeName;
    [SerializeField]
    private Spell _costumeBaseSpell;

    [SerializeField]
    private Spell[] _defaultSpellList;

    public string CostumeID { get { return _costumeID; } }
    public string CostumeName { get { return _costumeName; } }
    public Spell[] DefaultSpellList { get { return _defaultSpellList;} }
    public Spell CostumeBaseSpell { get { return _costumeBaseSpell; } }
}

