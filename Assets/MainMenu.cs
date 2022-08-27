// Script coded by Ben Baeyens - https://www.benbaeyens.com/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    #region variables

    public AudioClip buttonClickSound;

    public string website = "https://benbaeyens.com/";
    public string github = "https://github.com/benbaeyens/";
    public string linkedin = "https://www.linkedin.com/in/ben-baeyens-159287175/";


    #endregion


    #region methods

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        AudioManager.Instance.Play(buttonClickSound);
    }


    public void LinkToSite()
    {
        Application.OpenURL(website);
        AudioManager.Instance.Play(buttonClickSound);

    }

    public void LinkToGithub()
    {
        AudioManager.Instance.Play(buttonClickSound);
        Application.OpenURL(github);
    }

    public void LinkToLinkedin()
    {
        AudioManager.Instance.Play(buttonClickSound);
        Application.OpenURL(linkedin);
    }

    #endregion
}
