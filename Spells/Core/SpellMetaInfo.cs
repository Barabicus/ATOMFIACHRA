using UnityEngine;
using System.Collections;
using System;

[DisallowMultipleComponent]
public class SpellMetaInfo : MonoBehaviour
{
    [SerializeField]
    private SpellFilters[] _filters;

    private SpellMetaInfoConstructed _metaInfoCache;

    public SpellMetaInfoConstructed MetaInfoConstructed
    {
        get
        {
            if(_metaInfoCache != null)
            {
                return _metaInfoCache;
            }
            // Construct meta info
            var info = new SpellMetaInfoConstructed();
            foreach (SpellFilters filter in _filters)
            {
                info.SetFilter(filter, true);
            }
            _metaInfoCache = info;
            return info;
        }
    }
}

public class SpellMetaInfoConstructed
{
    private SpellFilters _filters;

    public SpellFilters Filters { get { return _filters; } }

    public SpellMetaInfoConstructed()
    {
        _filters = 0;
    }

    public void SetFilter(SpellFilters filter, bool value)
    {
        if (value)
        {
            _filters = _filters.Add<SpellFilters>(filter);
        }
        else
        {
            _filters = _filters.Remove<SpellFilters>(filter);
        }
    }

    public void SetFilter(string filter, bool value)
    {
        SpellFilters f = (SpellFilters)Enum.Parse(typeof(SpellFilters), filter);
        SetFilter(f, value);
    }

    public bool SpellApplicable(SpellMetaInfo spell)
    {
        return (spell.MetaInfoConstructed.Filters & Filters) > 0;
    }
}

[System.Flags]
public enum SpellFilters
{
    Fire = 1 << 2,
    Water = 1 << 3,
    Air = 1 << 4,
    Earth = 1 << 5,
    Death = 1 << 6,
    Arcane = 1 << 7
}
