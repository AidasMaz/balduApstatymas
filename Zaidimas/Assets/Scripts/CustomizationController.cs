using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using System;

public class CustomizationController : MonoBehaviour
{
    [Header("Customizable object")]
    public GameObject customizableObject;

    [Header("All prefabs")]
    [SerializeField]
    private List<GameObject> _allObjects;

    [Header("Set objects")]
    public List<GameObject> setObjects;

    [Header("General variables")]
    private bool _showMovingObject;
    private bool _selectingObject;
    [Space]
    public GameObject room;
    [Space]
    public string selectableTag = "Selectable";
    public string groundTag = "Ground";
    [Space]
    [SerializeField]
    private Material _objectMaterial;

    [Header("Controller objects")]
    [SerializeField]
    private UIController _uiController;
    [SerializeField]
    private ObjectDataController _objectDataController;

    //-------------------------------------

    #region Start Update... functions
    private void Start()
    {
        _showMovingObject = false;
        _selectingObject = false;

        LoadList();
    }

    private void LoadList()
    {
        _allObjects = new List<GameObject>();

        foreach (GameObject obj in Resources.LoadAll("CataloguePrefabs/New", typeof(GameObject)))
        {
            _allObjects.Add(obj);
        }
    }

    private void Update()
    {
        if (_showMovingObject && customizableObject != null)
        {
            SetMovingObjectCoordinates();

            // Possible upgrade: If colliding - red material and not alowing to place

            if (Input.GetMouseButtonDown(0))
            {
                PlaceObject();

                _uiController.DeselectLastSelectedCatalogueButton();
            }
        }

        if (_selectingObject && Input.GetMouseButtonDown(0))
        {
            SelectObjectIfRaycastHit();
        }
    }
    #endregion

    //-------------------------------------

    #region Raycast functions
    private void SetMovingObjectCoordinates()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction, 13.0F);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.CompareTag(groundTag))
            {
                customizableObject.transform.position = new Vector3(hits[i].point.x, 0, hits[i].point.z);
                break;
            }
        }
    }

    private void SelectObjectIfRaycastHit()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.CompareTag(selectableTag))
            {
                customizableObject = hit.transform.parent.gameObject;
                ObjectData objData = Array.Find(_objectDataController.allObjects.dataObjects, o => o.objectName == customizableObject.transform.GetChild(0).name.TrimEnd('_'));
                Debug.Log(objData.objectName);
                _selectingObject = false;
                _uiController.OpenCustomizationMenu(objData);
            }
        }
    }
    #endregion

    //-------------------------------------

    #region Simple object modifications
    public void RotateObjectLeft()
    {
        if (customizableObject != null)
        {
            customizableObject.transform.Rotate(Vector3.up, 45);
        }
        else
        {
            Debug.LogWarningFormat(" CustomizationController | No object to rotate left");
        }
    }
    public void RotateObjectRight()
    {
        if (customizableObject != null)
        {
            customizableObject.transform.Rotate(Vector3.up, -45);
        }
        else
        {
            Debug.LogWarningFormat(" CustomizationController | No object to rotate right");
        }
    }

    public void DeleteObject()
    {
        if (customizableObject != null)
        {
            setObjects.Remove(customizableObject);
            Destroy(customizableObject);
            customizableObject = null;
        }
        else
        {
            Debug.LogWarningFormat(" CustomizationController | No object to delete");
        }
    }

    public void SetObjectColor(Color32 color)
    {
        if (customizableObject != null)
        {
            customizableObject.GetComponentInChildren<MeshRenderer>().material.color = color;
        }
        else
        {
            Debug.LogWarningFormat(" CustomizationController | No object to set material color");
        }
    }
    #endregion

    //--------------------------------------------

    #region Object Moving and placing
    public void StartLookingForObject()
    {
        _selectingObject = true;
    }
    public void StopLookingForObject()
    {
        _selectingObject = false;
        customizableObject = null;
    }

    public void CreateTemporaryObject(string objectName, Color32 color)
    {
        GameObject prefab = _allObjects.Find(o => o.name == objectName);
        customizableObject = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity, room.transform); 

        // Possible upgrade: add temporary transparent material while object is not placed

        customizableObject.GetComponentInChildren<MeshRenderer>().material = _objectMaterial;
        customizableObject.GetComponentInChildren<MeshRenderer>().material.color = color;
        _showMovingObject = true;
    }
    public void DeleteTemporaryObject()
    {
        if (_showMovingObject && customizableObject != null)
        {
            Destroy(customizableObject);
            customizableObject = null;
        }
    }

    public void StartMoving()
    {
        _showMovingObject = true;
    }

    public void PlaceObject()
    {
        _showMovingObject = false;
        setObjects.Add(customizableObject);
        _uiController.SetSaveLoadButtonInteractability();
    }

    public void DeselectObject()
    {
        customizableObject = null;
    }
    #endregion
}
