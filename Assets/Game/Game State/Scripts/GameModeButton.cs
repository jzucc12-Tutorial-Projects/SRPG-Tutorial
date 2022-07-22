using System;
using UnityEngine;
using UnityEngine.UI;

public class GameModeButton : MonoBehaviour
{
    [SerializeField] private GameMode myMode = GameMode.team2AI;
    [SerializeField] private Button button = null;
    private static Action<GameMode> SiblingClick = null;


    private void Awake()
    {
        SetActive(!GameGlobals.IsThisMode(myMode));
    }

    private void OnEnable()
    {
        SiblingClick += SetActive;
        button.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        SiblingClick -= SetActive;
        button.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        GameGlobals.SetMode(myMode);
        SiblingClick?.Invoke(myMode);
    }

    private void SetActive(GameMode newMode)
    {
        SetActive(!GameGlobals.IsThisMode(myMode));
    }

    private void SetActive(bool active)
    {
        button.interactable = active;
    }
}
