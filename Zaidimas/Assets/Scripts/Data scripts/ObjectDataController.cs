using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class ObjectDataCategory
{
    public string categoryName;
    public ObjectData[] dataObjects;

    public void CheckDataObjectForMistakes()
    {
        if (categoryName == null)
        {
            Debug.LogWarning(string.Format(" ObjectDataCategory | Category has no name"));
        }

        CheckObjectDataArrayForMistakes();
    }

    private void CheckObjectDataArrayForMistakes()
    {
        for (int i = 0; i < dataObjects.Length; i++)
        {
            if (dataObjects[i].objectName == null)
            {
                Debug.LogWarningFormat(" ObjectDataCategory | In category {1} object {0} has no name", i + 1, categoryName);
            }
            if (dataObjects[i].price == 0)
            {
                Debug.LogWarningFormat(" ObjectDataCategory | In category {1} object {0} has no price", i + 1, categoryName);
            }

            CheckColorDataArrayForMistakes(i);
        }
    }

    private void CheckColorDataArrayForMistakes(int objNumber)
    {
        var obj = dataObjects[objNumber];

        if (obj.colors.Length > 0)
        {
            for (int j = 0; j < obj.colors.Length; j++)
            {
                if (obj.colors[j].colorName == null)
                {
                    Debug.LogWarningFormat(" ObjectDataController | In category {1} object {0} color {2} has no name", objNumber + 1, categoryName, j + 1);
                }

                CheckColorForMistakes(objNumber, j);
            }
        }
    }

    private void CheckColorForMistakes(int objNumber, int colorNumber)
    {
        var color = dataObjects[objNumber].colors[colorNumber];

        if (color.rgbValues.Length != 3)
        {
            Debug.LogWarning(string.Format(" ObjectDataController | In category {1} object {0} color {2} has incorrect number of values", objNumber + 1, categoryName, colorNumber + 1));

            if (color.rgbValues.Length > 0)
            {
                if (color.rgbValues[0] < 0 || color.rgbValues[0] > 255)
                {
                    Debug.LogWarning(string.Format(" ObjectDataController | In category {1} object {0} color {2} first (red) value is incorrect", objNumber + 1, categoryName, colorNumber + 1));

                }
                if (color.rgbValues[1] < 0 || color.rgbValues[1] > 255)
                {
                    Debug.LogWarning(string.Format(" ObjectDataController | In category {1} object {0} color {2} second (green) value is incorrect", objNumber + 1, categoryName, colorNumber + 1));

                }
                if (color.rgbValues[2] < 0 || color.rgbValues[2] > 255)
                {
                    Debug.LogWarning(string.Format(" ObjectDataController | In category {1} object {0} color {2} third (blue) value is incorrect", objNumber + 1, categoryName, colorNumber + 1));

                }
            }
        }
    }
}

[Serializable]
public class ObjectData
{
    public string objectName;
    public float price;         // float - because no complex math operations will be used with it. Otherwise, here should be decimal
    public ColorData[] colors;
}

[Serializable]
public class ColorData
{
    public string colorName;
    public int[] rgbValues;
}

public class ObjectDataController : MonoBehaviour
{
    [Header("Object categories")]
    // It is intended to have more categories later on.
    [SerializeField]
    public ObjectDataCategory allObjects;

    [Header("Variables")]
    [SerializeField]
    private TextAsset _dataObjectFile;

    //-------------------------------

    public ObjectDataCategory GetCategory()
    {
        if (String.IsNullOrEmpty(allObjects.categoryName))
        {
            ReadDataObjectFile();
        }

        return allObjects;
    }

    private void ReadDataObjectFile()
    {
        if (_dataObjectFile != null)
        {
            allObjects = JsonUtility.FromJson<ObjectDataCategory>(_dataObjectFile.text);

            if (allObjects.dataObjects.Length == 0)
            {
                Debug.LogWarningFormat(" ObjectDataController | Data file {0} has no data objects", _dataObjectFile.name);
                return;
            }

            allObjects.CheckDataObjectForMistakes();
        }
        else
        {
            Debug.LogWarningFormat(" ObjectDataController | Data file _dataObjectFile was not found");
        }
    }
}
