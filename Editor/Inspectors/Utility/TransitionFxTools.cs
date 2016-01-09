using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class TransitionFxTools {

    public static IEnumerable<TransitionFxCategoryContainer> GetCategoryList()
    {
        var typesWithMyAttribute =
            from a in AppDomain.CurrentDomain.GetAssemblies()
            from t in a.GetTypes()
            let attributes = t.GetCustomAttributes(typeof(TransitionFxCategoryAttribute), true)
            where attributes != null && attributes.Length > 0
            //  select new { Type = t, Attribute = Attribute.Cast<SpellCategoryAttribute>() };
            select new TransitionFxCategoryContainer(t, attributes.Cast<TransitionFxCategoryAttribute>().FirstOrDefault());
        return typesWithMyAttribute;
    }

}

public struct TransitionFxCategoryContainer
{
    private Type _type;
    private TransitionFxCategoryAttribute _attribute;

    public Type Type
    {
        get { return _type; }
        set { _type = value; }
    }

    public TransitionFxCategoryAttribute Attribute
    {
        get { return _attribute; }
        set { _attribute = value; }
    }

    public TransitionFxCategoryContainer(Type type, TransitionFxCategoryAttribute attribute)
    {
        _type = type;
        _attribute = attribute;
    }
}