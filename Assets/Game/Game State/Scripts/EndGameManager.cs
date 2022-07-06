using System.Collections;
using UnityEngine;

public class EndGameManager : MonoBehaviour
{
    #region //Variables
    [SerializeField] private bool playerIsSoldier = true;
    [SerializeField] private GameObject soldierWinContainer = null;
    [SerializeField] private GameObject wizardWinContainer = null;
    [SerializeField] private float waitTimer = 1f;
    #endregion
    

    #region //Monobehaviour
    private void Awake()
    {
        soldierWinContainer.SetActive(false);
        wizardWinContainer.SetActive(false);
    }

    private void OnEnable()
    {
        UnitManager.GameOverSided += GameOver;
    }

    private void OnDisable()
    {
        UnitManager.GameOverSided += GameOver;
    }
    #endregion

    #region //End game
    private void GameOver(bool playersWon)
    {
        StartCoroutine(GameOverWait(!(playersWon ^ playerIsSoldier)));
    }

    private IEnumerator GameOverWait(bool soldiersWon)
    {
        yield return new WaitForSecondsRealtime(waitTimer);
        soldierWinContainer.SetActive(soldiersWon);
        wizardWinContainer.SetActive(!soldiersWon);
        Time.timeScale = 0;
    }
    #endregion
}
