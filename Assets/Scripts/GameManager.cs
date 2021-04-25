using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static bool PlayerIsAlive;
    public static bool GameIsPaused;

    public static bool _disablePowerUp_Spawning = false;

    [SerializeField] bool _isGameOver = false;
    [SerializeField] bool _gamePaused = false;
    [SerializeField] GameObject _pausePanel;

    void Awake()
    {
        instance = this;
        PlayerIsAlive = true;
        GameIsPaused = false;
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.I)) && _isGameOver)
        {
            SceneManager.LoadScene(1); // Reload current game scene "GalaxyShooterDemo"
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !_isGameOver)
        {
            //QuitGame();
            PauseGame();
        }

        if (Input.GetKeyDown(KeyCode.P) && !_isGameOver)
        {
            PauseGame();
        }

        /*
        if (_isAsteroidDestroyed && !_EnemyInvasion)
        {
            _EnemyInvasion = true;
            _spawnManager.Spawn();
        }
        */
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void PauseGame()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;

        if (!_gamePaused)
        {
            _gamePaused = true;
            _pausePanel.SetActive(true);
        }
        else
        {
            _gamePaused = false;
            _pausePanel.SetActive(false);
        }
    }

    public void GameOver()
    {
        _isGameOver = true;
    }
    public void OnPlayerDeath()
    {
        PlayerIsAlive = false;
        _isGameOver = true;
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Scene 0 = 'MainMenu'
    }
}
