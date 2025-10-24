using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour
{
    [SerializeField] private Toggle[] levelToggles;


    private void Awake()
    {
        // Initialize toggles based on saved selection
        SetSavedToggle();
    }

    private void SetSavedToggle()
    {
        int savedLevel = PlayerPrefs.GetInt("SelectedLevel", 1);

        if (savedLevel < 1 || savedLevel > levelToggles.Length) savedLevel = 1;

        levelToggles[savedLevel - 1].isOn = true;
    }

    public void OnLevelToggleChanged(Toggle toggle)
    {
        if (!toggle.isOn) return; // only handle when toggle is turned on

        int selectedLevel = -1;
        for (int i = 0; i < levelToggles.Length; i++)
        {
            if (levelToggles[i].isOn)
            {
                selectedLevel = i + 1;
                break;
            }
        }

        if (selectedLevel == -1)
        {
            Debug.Log("No level selected!");
            return;
        }

        // Save selected level to PlayerPrefs
        PlayerPrefs.SetInt("SelectedLevel", selectedLevel);
        PlayerPrefs.Save();

    }

    // Called from UI button
    public void OnPlayButtonClick()
    {
        SceneManager.LoadScene("GamePlayScene");
    }
}
