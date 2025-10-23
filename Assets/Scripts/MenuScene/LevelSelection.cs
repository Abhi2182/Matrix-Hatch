using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour
{
    [SerializeField]private Toggle[] levelToggles; // Assign 7 toggles in inspector

    public void OnPlayButtonClick()
    {
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
            Debug.LogWarning("No level selected!");
            return;
        }

        // Save selected level to PlayerPrefs
        PlayerPrefs.SetInt("SelectedLevel", selectedLevel);
        PlayerPrefs.Save();

        SceneManager.LoadScene("GamePlayScene");
    }
}
