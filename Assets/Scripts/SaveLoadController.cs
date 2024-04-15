using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadController : MonoBehaviour
{
    public static GameSave gameSave = new GameSave();
    public static bool hasLoadedFromFile = false;

    public static SaveLoadController instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        LoadGame();
    }

    public static void SaveGame()
    {
        if (gameSave != null)
        {
            string jsonData = JsonUtility.ToJson(gameSave);
            PlayerPrefs.SetString("GameSaveData", jsonData);
            PlayerPrefs.Save();  // Don't forget to save PlayerPrefs changes.
            Debug.Log("Game saved!");
        }
    }

    public static void LoadGame()
    {
        string jsonData = PlayerPrefs.GetString("GameSaveData", "{}");
        gameSave = JsonUtility.FromJson<GameSave>(jsonData);
        if (gameSave != null)
        {
            Debug.Log("Game loaded!");
            hasLoadedFromFile = true;

            if (SceneManager.GetActiveScene().name == "")
            {
                MenuController.instance.buttonText.SetText("Continue");
            }
        }
        else
        {
            Debug.Log("No save data found!");
        }
            
    }
}

[Serializable]
public class GameSave
{

}
