using UnityEngine;
using BehaviorDesigner.Runtime;

[System.Serializable]
public class SharedEntity : SharedVariable
{
	public Entity Value { get { return mValue; } set { mValue = value; } }
	[SerializeField]
    private Entity mValue;

	public override object GetValue() { return mValue; }
    public override void SetValue(object value) { mValue = (Entity)value; }

	public override string ToString() { return mValue == null ? "null" : mValue.ToString(); }
    public static implicit operator SharedEntity(Entity value) { var sharedVariable = new SharedEntity(); sharedVariable.SetValue(value); return sharedVariable; }
}