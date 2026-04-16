using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainButtonsPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private string heroSelectSceneName = "HeroSelect";

    private void Start()
    {
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void StartRun()
    {
        SceneManager.LoadScene(heroSelectSceneName);
    }

    public void ContinueGame()
    {
        Debug.Log("Continue not implemented yet.");
    }

    public void OpenSettings()
    {
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}