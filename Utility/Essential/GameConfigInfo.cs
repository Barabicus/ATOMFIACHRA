using UnityEngine;
using System.Collections;

public class GameConfigInfo : ScriptableObject
{
    public EntitySpeechBubble EntitySpeechBubblePrefab;
    public EntityInfoGUI EntityHealthBarPrefab;
    public UGUIEntityMinimapIcon EntityMinimapIcon;
    public EntityCastingBar EntityCastingBarPrefab;
    public SpellColorDetails FireColorDetails;
    public SpellColorDetails WaterColorDetails;
    public SpellColorDetails AirColorDetails;
    public SpellColorDetails EarthColorDetails;
    public SpellColorDetails DeathColorDetails;
    public SpellColorDetails ArcaneColorDetails;

}

public struct SpellColorDetails
{
    private Color _spellColor;
    private Color _outlineColor;

    public Color SpellColor
    {
        get
        {
            return _spellColor;
        }

        set
        {
            _spellColor = value;
        }
    }

    public Color OutlineColor
    {
        get
        {
            return _outlineColor;
        }

        set
        {
            _outlineColor = value;
        }
    }
}