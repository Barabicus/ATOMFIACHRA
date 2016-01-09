using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class QuestUtilityTools
{

    public static IEnumerable<QuestCategoryContainer> GetCategoryList()
    {
        var typesWithMyAttribute =
            from a in AppDomain.CurrentDomain.GetAssemblies()
            from t in a.GetTypes()
            let attributes = t.GetCustomAttributes(typeof(QuestCategoryAttribute), true)
            where attributes != null && attributes.Length > 0
            select new QuestCategoryContainer(t, attributes.Cast<QuestCategoryAttribute>().FirstOrDefault());
        return typesWithMyAttribute;
    }

}

public struct QuestCategoryContainer
{
    private Type _type;
    private QuestCategoryAttribute _attribute;

    public Type Type
    {
        get { return _type; }
        set { _type = value; }
    }

    public QuestCategoryAttribute Attribute
    {
        get { return _attribute; }
        set { _attribute = value; }
    }

    public QuestCategoryContainer(Type type, QuestCategoryAttribute attribute)
    {
        _type = type;
        _attribute = attribute;
    }
}
