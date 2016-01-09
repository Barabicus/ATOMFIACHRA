using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerExperienceGUI : MonoBehaviour {

    [SerializeField]
    private Image _experienceImage;

    private PlayerCharacter _player;

    private void Start()
    {
        _player = GameMainReferences.Instance.PlayerCharacter;
        SetXP();
    }

    private void LateUpdate()
    {
        SetXP();
    }

    private void SetXP()
    {
        _experienceImage.fillAmount = _player.CurrentExperience / _player.NextLevelExperience;
    }

}
