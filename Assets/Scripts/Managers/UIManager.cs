using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Elements")]
    public TMP_Text scoreText;
    public TMP_Text movesText;

    [Header("Game Over Panel Reference")]
    public GameOverPanel gameOverPanel;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        UpdateScore(0);
        UpdateMoves(0);

        if (gameOverPanel != null) gameOverPanel.gameObject.SetActive(false);
    }

    // called by GameManager whenever score changes
    public void UpdateScore(int value)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + value;
    }

    // called by GameManager whenever moves change
    public void UpdateMoves(int value)
    {
        if (movesText != null)
            movesText.text = "Moves: " + value;
    }

    // show game over popup
    public void ShowWinPanel(int finalScore)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.gameObject.SetActive(true);
            gameOverPanel.UpdateGameOverPanel(finalScore, GameManager.Instance.movesMade);
        }
    }

    // Called from UI button
    public void OnBackButtonCLick()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
