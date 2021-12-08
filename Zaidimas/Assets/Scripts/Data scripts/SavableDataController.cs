using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SavableSet
{
    public DateTime lastSave;
    public SavableObjectData[] setObjects;
}

[Serializable]
public class SavableObjectData
{
    public string name;
    public Transform transform;
    public string colorName;
}

public class SavableDataController : MonoBehaviour
{
    [Header("Set objects")]
    public SavableSet savableSet;

    [Header("Variables")]
    public string saveFilePath;

    [Header("Controllers")]
    [SerializeField]
    private CustomizationController _customizationController;
    [SerializeField]
    private UIController _uiController;

    //--------------------------

    public void FillSavableSet()
    {
        // put objects from _customizationController to savableSet
    }

    public void SaveSetData()
    {
        Debug.Log(Application.persistentDataPath);

        //if (SaveFileExists())
        //{
        //    File.Delete(Application.persistentDataPath + saveFilePath);
        //}

        //FillSavableSet();

        //string jsonString = JsonUtility.ToJson(savableSet, true);
        //StreamWriter writer = new StreamWriter(Application.persistentDataPath + saveFilePath);
        //writer.Write(jsonString);
        //writer.Close();

    }

    public void LoadSetData()
    {
        if (SaveFileExists())
        {
            ReadSavableDataFile();
        }
        else
        {
            Debug.LogWarningFormat(" SavableDataController | Data file _saveFileName was not found");
        }
    }

    private void ReadSavableDataFile()
    {
        if (SaveFileExists())
        {
            //savableSet = JsonUtility.FromJson<SavableSet>(Application.persistentDataPath + saveFilePath);
        }
        else
        {
            Debug.LogWarningFormat(" SavableDataController | Data file _saveFileName was not found");
        }
    }

    public bool SaveFileExists()
    {
        if (File.Exists(Application.persistentDataPath + saveFilePath))
            return true;
        else
            return false;
    }
}