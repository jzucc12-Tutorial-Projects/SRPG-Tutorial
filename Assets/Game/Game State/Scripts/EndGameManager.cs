using System.Collections;
using JZ.AUDIO;
using UnityEngine;

public class EndGameManager : MonoBehaviour
{
    #region //Variables
    [SerializeField] private bool playerIsSoldier = true;
    [SerializeField] private GameObject team1Container = null;
    [SerializeField] private GameObject team2Container = null;
    [SerializeField] private float waitTimer = 1f;
    [SerializeField] private SoundPlayer sfxPlayer = null;
    [SerializeField] private SoundPlayer bgMusic = null;
    #endregion
    

    #region //Monobehaviour
    private void Awake()
    {
        team1Container.SetActive(false);
        team2Container.SetActive(false);
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
        bgMusic.StopAll();
        StartCoroutine(GameOverWait(!(playersWon ^ playerIsSoldier)));
    }

    private IEnumerator GameOverWait(bool team1Won)
    {
        yield return new WaitForSecondsRealtime(waitTimer);
        if(GameGlobals.TwoPlayer() || !GameGlobals.IsAI(team1Won))
            sfxPlayer.Play("victory");
        else
            sfxPlayer.Play("defeat");
        team1Container.SetActive(team1Won);
        team2Container.SetActive(!team1Won);
        Time.timeScale = 0;
    }
    #endregion
}
