using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class SpellUtilityTools
{

    public static IEnumerable<SpellCategoryContainer> GetSpellCategoryList()
    {
        var typesWithMyAttribute =
            from a in AppDomain.CurrentDomain.GetAssemblies()
            from t in a.GetTypes()
            let attributes = t.GetCustomAttributes(typeof (SpellCategoryAttribute), true)
            where attributes != null && attributes.Length > 0
            select new SpellCategoryContainer(t, attributes.Cast<SpellCategoryAttribute>().FirstOrDefault());
        return typesWithMyAttribute;
    }

}

public struct SpellCategoryContainer
{
    private Type _type;
    private SpellCategoryAttribute _attribute;

    public Type Type
    {
        get { return _type; }
        set { _type = value; }
    }

    public SpellCategoryAttribute Attribute
    {
        get { return _attribute; }
        set { _attribute = value; }
    }

    public SpellCategoryContainer(Type type, SpellCategoryAttribute attribute)
    {
        _type = type;
        _attribute = attribute;
    }


}