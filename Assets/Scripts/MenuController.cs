using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public Transform[] doNotDestroyOnLoad;
    public TextMeshProUGUI buttonText;

    public static MenuController instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        foreach (Transform t in doNotDestroyOnLoad)
        {
            DontDestroyOnLoad(t.gameObject);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
