using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text movesText;
    [SerializeField] private TMP_Text bestScoreText; //best score display based on level
    [SerializeField] private GameObject gameOverPanel;

    // Called from UI button
    public void OnRestartButtonClicked()
    {
        gameOverPanel?.SetActive(false);
        GameManager.Instance?.StartNewGame(GameManager.Instance.rows, GameManager.Instance.cols);
    }

    // Called from UI button
    public void OnMenuButtonClicked()
    {
        SceneManager.LoadScene("MenuScene");
    }
    public void UpdateGameOverPanel(int finalScore, int movesMade)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + finalScore;
        if (movesText != null)
            movesText.text = "Moves: " + movesMade;
        // Update best score based on level
        int selectedLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        string bestScoreKey = "BestScore_Level_" + selectedLevel;
        int bestScore = PlayerPrefs.GetInt(bestScoreKey, 0);
        if (finalScore > bestScore)
        {
            bestScore = finalScore;
            PlayerPrefs.SetInt(bestScoreKey, bestScore);
        }
        if (bestScoreText != null)
            bestScoreText.text = "Best Score: " + bestScore;
    }
}
