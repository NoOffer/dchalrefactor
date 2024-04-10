using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ContinueGame() {
        if (PlayerPrefs.HasKey("CurrentLevel")) {
            SceneManager.LoadScene(PlayerPrefs.GetString("CurrentLevel"));
        } else {
            PlayerPrefs.SetString("CurrentLevel", "Test Combat Room");
            SceneManager.LoadScene("Test Combat Room");
        }
    }

    public void NewGame() {
        SceneManager.LoadScene("Test Combat Room");
    }

    public void Options() {
        SceneManager.LoadScene("Options");
    }

    public void Credits() {
        SceneManager.LoadScene("Credits");
    }

    public void Menu() {
        SceneManager.LoadScene("Main Menu");
    }

    public void Controls() {
        SceneManager.LoadScene("Controls");
    }

    public void Difficulty() {
        SceneManager.LoadScene("Difficulty");
    }

    public void Quit() {
        Application.Quit();
    }
}