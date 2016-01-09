using UnityEngine;
using BehaviorDesigner.Runtime;


[System.Serializable]
public class SharedSpell : SharedVariable
{
	public Spell Value { get { return mValue; } set { mValue = value; } }
	[SerializeField]
    private Spell mValue;

	public override object GetValue() { return mValue; }
    public override void SetValue(object value) { mValue = (Spell)value; }

	public override string ToString() { return mValue == null ? "null" : mValue.ToString(); }
    public static implicit operator SharedSpell(Spell value) { var sharedVariable = new SharedSpell(); sharedVariable.SetValue(value); return sharedVariable; }
}