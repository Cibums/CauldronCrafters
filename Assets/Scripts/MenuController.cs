using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public Transform[] doNotDestroyOnLoad;

    void Start()
    {
        foreach (Transform t in doNotDestroyOnLoad)
        {
            DontDestroyOnLoad(t.gameObject);
        }

        SceneManager.LoadScene("SampleScene");
    }
}
