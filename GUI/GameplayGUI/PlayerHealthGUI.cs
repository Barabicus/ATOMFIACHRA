using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealthGUI : MonoBehaviour
{
    [SerializeField]
    private Image _fillingHealthImage;
    [SerializeField]
    private Text _healthText;

    private Entity _player;

    private void Start()
    {
        _player = GameMainReferences.Instance.PlayerController.Entity;
    }

    private void Update()
    {
        _healthText.text = Mathf.Ceil(_player.CurrentHp).ToString();
        _fillingHealthImage.fillAmount = MathUtility.GetPercent(_player.CurrentHp, _player.StatHandler.MaxHp);
    }


}
