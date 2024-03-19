using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private string mainMenuScene;

    public void Continue()
    {
        gameObject.SetActive(false);
        UISoundManager.Instance.PlayAudio();
    }

    public void MainMenu()
    {
        UISoundManager.Instance.PlayAudio();
        StartCoroutine(LoadMainMenu());
    }

    public void Quit(){
        UISoundManager.Instance.PlayAudio();
        StartCoroutine(QuitGame());
    }

    IEnumerator LoadMainMenu(){
        yield return new WaitForSeconds(.3f);
        SceneManager.LoadScene(mainMenuScene);
    }
    IEnumerator QuitGame(){
        yield return new WaitForSeconds(.3f);
        Application.Quit();
    }
}
