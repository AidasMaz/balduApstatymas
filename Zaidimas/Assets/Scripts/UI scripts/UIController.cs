using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Main Menu objects")]
    public GameObject mainMenu;
    [Space]
    private Vector3 _originalMainMenuPosition;
    [Space]
    public GameObject customizationButton;
    public GameObject catalogueButton;
    public GameObject saveButton;
    public GameObject loadButton;

    [Header("Hidden menu")]
    public GameObject hiddenMenu;
    [Space]
    private Vector3 _originalhiddenMenuPosition;

    [Header("Catalogue menu objects")]
    public GameObject catalogueMenu;
    [Space]
    private Vector3 _originalCatalogueMenuPosition;
    [Space]
    private bool _isCatalogueMenuActivated;
    [Space]
    private Button _lastSelectedCatalogueButton;
    [Space]
    public GameObject scrollableCatalogueObject;
    public GameObject catalogueButtonPrefab;

    [Header("Customization menu objects")]
    public GameObject customizationMenu;
    [Space]
    private Vector3 _originalCustomizationMenuPosition;
    [Space]
    [SerializeField]
    private Button[] _customizationColorButtons;
    [Space]
    [SerializeField]
    private Text _customizationNameText;
    [SerializeField]
    private Text _customizationPriceText;
    [SerializeField]
    private Text _customizationColorText;
    [Space]
    private bool _isCustomizationMenuActivated;
    private bool _isCustomizationNotificationActive;

    [Header("Notification object")]
    public GameObject notification;
    [Space]
    private Vector3 _originalNotificationPosition;

    [Header("Quit pop up")]
    public GameObject quitPopUp;
    [Space]
    public Text lastSaveText;

    [Header("General variables")]
    [Range(0, 1)]
    [SerializeField]
    private float _movementUISpeed;
    [Range(0, 1)]
    [SerializeField]
    private float _movementNotificationSpeed;
    [Space]
    private bool _isUIVisible;
    [Space]
    public Color buttonHighLightColor;
    public Color buttonNormalColor;
    [Space]
    public List<ObjectData> catalogueObjects;

    [Header("Controllers")]
    [SerializeField]
    private ObjectDataController _objectDataController;
    [SerializeField]
    private SavableDataController _savableDataController;
    [SerializeField]
    private CustomizationController _customizationController;

    //---------------------------------

    #region Start, Update... functions
    void Start()
    {
        _isUIVisible = true;

        _isCatalogueMenuActivated = false;
        _isCustomizationMenuActivated = false;
        _isCustomizationNotificationActive = false;

        SetCatalogueButtonData();
        SetSaveLoadButtonInteractability();
    }

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (quitPopUp.activeInHierarchy)
            {
                CloseQuitPopUp();
            }

            if (_lastSelectedCatalogueButton != null)
            {
                DeselectLastSelectedCatalogueButton();
                _customizationController.DeleteTemporaryObject();
            }

            if (_isCustomizationNotificationActive)
            {
                HideCustomizationNotification();
                EnableCustomizationButton();
                _customizationController.StopLookingForObject();
            }
        }
    }
    #endregion

    //----------------------------------

    #region Setting UI with data
    private void SetCatalogueButtonData()
    {
        ObjectDataCategory objects = _objectDataController.GetCategory();

        GameObject button;
        Sprite sprite;
        foreach (var obj in objects.dataObjects)
        {
            button = Instantiate(catalogueButtonPrefab, scrollableCatalogueObject.transform);
            sprite = Resources.Load<Sprite>("CatalogueObjectSprites/New/" + obj.objectName);
            button.GetComponentsInChildren<Image>()[1].sprite = sprite;

            button.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                SelectCatalogueButton();
                Color32 color = new Color32((byte)obj.colors[0].rgbValues[0], (byte)obj.colors[0].rgbValues[1], (byte)obj.colors[0].rgbValues[2], 255);
                _customizationController.CreateTemporaryObject(obj.objectName, color);
            });
        }
    }

    private void SetCustomizationMenuData(ObjectData obj)
    {
        _customizationNameText.text = "Name: " + obj.objectName;
        _customizationPriceText.text = "Price: " + obj.price;
        _customizationColorText.text = "Color: " + obj.colors[0].colorName;

        for (int i = 0; i < 3; i++)
        {
            if (i < obj.colors.Length)
            {
                var color = new Color32((byte)obj.colors[i].rgbValues[0], (byte)obj.colors[i].rgbValues[1], (byte)obj.colors[i].rgbValues[2], 255);
                _customizationColorButtons[i].GetComponent<Image>().color = color;
                int index = i;
                _customizationColorButtons[i].GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    _customizationController.SetObjectColor(color);
                    SetColorText(obj.colors[index].colorName);
                });
                _customizationColorButtons[i].gameObject.SetActive(true);
            }
            else
            {
                _customizationColorButtons[i].gameObject.SetActive(false);
            }
        }
    }
    private void SetColorText(string text)
    {
        _customizationColorText.text = "Color: " + text;
    }
    #endregion

    //----------------------------------

    #region Menu window open/close
    public void OpenCatalogueMenu()
    {
        if (_isCustomizationMenuActivated)
        {
            CloseCustomizationMenu();
        }
        if (_isCustomizationNotificationActive)
        {
            HideCustomizationNotification();
        }

        _isCatalogueMenuActivated = true;
        DisableCatalogueButton();
        ShowCatalogueMenu();
    }
    public void CloseCatalogueMenu()
    {
        _isCatalogueMenuActivated = false;
        EnableCatalogueButton();
        HideCatalogueMenu();
        DeselectLastSelectedCatalogueButton();
    }

    public void OpenCustomizationFirstStage()
    {
        if (_isCatalogueMenuActivated)
        {
            CloseCatalogueMenu();
        }

        DisableCustomizationButton();
        ShowCustomizationNotification();
    }

    public void OpenCustomizationMenu(ObjectData objectData)
    {
        _isCustomizationMenuActivated = true;
        SetCustomizationMenuData(objectData);
        HideCustomizationNotification();
        ShowCustomizationMenu();
    }
    public void CloseCustomizationMenu()
    {
        _isCustomizationMenuActivated = false;
        EnableCustomizationButton();
        _customizationController.DeselectObject();
        HideCustomizationMenu();
    }
    #endregion

    #region Disable/Enable menu buttons
    private void DisableCustomizationButton()
    {
        customizationButton.GetComponent<Button>().interactable = false;
        customizationButton.GetComponent<Image>().color = buttonHighLightColor;
    }
    private void EnableCustomizationButton()
    {
        customizationButton.GetComponent<Button>().interactable = true;
        customizationButton.GetComponent<Image>().color = buttonNormalColor;
    }

    private void DisableCatalogueButton()
    {
        catalogueButton.GetComponent<Button>().interactable = false;
        catalogueButton.GetComponent<Image>().color = buttonHighLightColor;
    }
    private void EnableCatalogueButton()
    {
        catalogueButton.GetComponent<Button>().interactable = true;
        catalogueButton.GetComponent<Image>().color = buttonNormalColor;
    }

    public void SetSaveLoadButtonInteractability()
    {
        if (_savableDataController.SaveFileExists())
        {
            loadButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            loadButton.GetComponent<Button>().interactable = false;
        }

        if (_customizationController.setObjects.Count > 0)
        {
            saveButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            saveButton.GetComponent<Button>().interactable = false;
        }
    }
    #endregion

    //----------------------------------

    #region Active UI show/hide
    public void HideAllActiveUI()
    {
        _isUIVisible = false;

        HideMainMenu();
        ShowHiddenMenu();

        if (_isCatalogueMenuActivated)
        {
            HideCatalogueMenu();
        }

        if (_isCustomizationMenuActivated)
        {
            HideCustomizationMenu();
        }

        if (_isCustomizationNotificationActive)
        {
            HideCustomizationNotification();
        }
    }
    public void ShowAllActiveUI()
    {
        _isUIVisible = true;

        ShowMainMenu();
        HideHiddenMenu();

        if (_isCatalogueMenuActivated)
        {
            ShowCatalogueMenu();
        }

        if (_isCustomizationMenuActivated)
        {
            ShowCustomizationMenu();
        }

        if (_isCustomizationNotificationActive)
        {
            ShowCustomizationNotification();
        }
    }
    #endregion

    #region Show/Hide window tweens
    private void ShowMainMenu()
    {
        _originalMainMenuPosition = mainMenu.gameObject.GetComponent<RectTransform>().position;
        float pos = mainMenu.gameObject.GetComponent<RectTransform>().rect.width / 2;
        LeanTween.move(mainMenu.gameObject, new Vector3(pos, _originalMainMenuPosition.y, _originalMainMenuPosition.z), _movementUISpeed);
    }
    private void HideMainMenu()
    {
        _originalMainMenuPosition = mainMenu.gameObject.GetComponent<RectTransform>().position;
        float pos = mainMenu.gameObject.GetComponent<RectTransform>().rect.width / 2;
        LeanTween.move(mainMenu.gameObject, new Vector3(-pos, _originalMainMenuPosition.y, _originalMainMenuPosition.z), _movementUISpeed);
    }

    private void ShowHiddenMenu()
    {
        _originalhiddenMenuPosition = hiddenMenu.gameObject.GetComponent<RectTransform>().position;
        float pos = hiddenMenu.gameObject.GetComponent<RectTransform>().rect.width / 2;
        LeanTween.move(hiddenMenu.gameObject, new Vector3(pos, _originalhiddenMenuPosition.y, _originalhiddenMenuPosition.z), _movementUISpeed);
    }
    private void HideHiddenMenu()
    {
        _originalhiddenMenuPosition = hiddenMenu.gameObject.GetComponent<RectTransform>().position;
        float pos = hiddenMenu.gameObject.GetComponent<RectTransform>().rect.width / 2;
        LeanTween.move(hiddenMenu.gameObject, new Vector3(-pos, _originalhiddenMenuPosition.y, _originalhiddenMenuPosition.z), _movementUISpeed);
    }

    private void ShowCatalogueMenu()
    {
        _originalCatalogueMenuPosition = catalogueMenu.gameObject.GetComponent<RectTransform>().position;
        float pos = catalogueMenu.gameObject.GetComponent<RectTransform>().rect.width / 2;
        LeanTween.move(catalogueMenu.gameObject, new Vector3(pos, _originalCatalogueMenuPosition.y, _originalCatalogueMenuPosition.z), _movementUISpeed);
    }
    private void HideCatalogueMenu()
    {
        _originalCatalogueMenuPosition = catalogueMenu.gameObject.GetComponent<RectTransform>().position;
        float pos = catalogueMenu.gameObject.GetComponent<RectTransform>().rect.width / 2;
        LeanTween.move(catalogueMenu.gameObject, new Vector3(-pos, _originalCatalogueMenuPosition.y, _originalCatalogueMenuPosition.z), _movementUISpeed);
    }

    private void ShowCustomizationMenu()
    {
        _originalCustomizationMenuPosition = customizationMenu.gameObject.GetComponent<RectTransform>().position;
        float pos = customizationMenu.gameObject.GetComponent<RectTransform>().rect.width / 2;
        LeanTween.move(customizationMenu.gameObject, new Vector3(pos, _originalCustomizationMenuPosition.y, _originalCustomizationMenuPosition.z), _movementUISpeed);

    }
    private void HideCustomizationMenu()
    {
        _originalCustomizationMenuPosition = customizationMenu.gameObject.GetComponent<RectTransform>().position;
        float pos = customizationMenu.gameObject.GetComponent<RectTransform>().rect.width / 2;
        LeanTween.move(customizationMenu.gameObject, new Vector3(-pos, _originalCustomizationMenuPosition.y, _originalCustomizationMenuPosition.z), _movementUISpeed);
    }
    #endregion

    // -----------------------------

    public void SelectCatalogueButton()
    {
        DeselectLastSelectedCatalogueButton();

        _lastSelectedCatalogueButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        _lastSelectedCatalogueButton.image.color = buttonHighLightColor;
    }
    public void DeselectLastSelectedCatalogueButton()
    {
        if (_lastSelectedCatalogueButton != null)
        {
            _lastSelectedCatalogueButton.image.color = buttonNormalColor;
            _lastSelectedCatalogueButton = null;
        }
    }

    // -----------------------------

    #region Notification logic
    public void AnimateSavingNotification()
    {
        notification.GetComponentInChildren<Text>().text = "Set saved";

        StartCoroutine(MoveNotification());
    }
    public void AnimateLoadingNotification()
    {
        notification.GetComponentInChildren<Text>().text = "Set loaded";

        StartCoroutine(MoveNotification());
    }

    public void ShowCustomizationNotification()
    {
        notification.GetComponentInChildren<Text>().text = "Select object";
        _isCustomizationNotificationActive = true;
        ShowNotification();
    }
    public void HideCustomizationNotification()
    {
        _isCustomizationNotificationActive = false;
        HideNotification();
    }

    private void ShowNotification()
    {
        _originalNotificationPosition = notification.gameObject.GetComponent<RectTransform>().position;
        float pos = notification.gameObject.GetComponent<RectTransform>().rect.height / 2;
        LeanTween.move(notification.gameObject, new Vector3(_originalNotificationPosition.x, pos, _originalNotificationPosition.z), _movementNotificationSpeed);
    }
    public void HideNotification()
    {
        _originalNotificationPosition = notification.gameObject.GetComponent<RectTransform>().position;
        float pos = notification.gameObject.GetComponent<RectTransform>().rect.height / 2;
        LeanTween.move(notification.gameObject, new Vector3(_originalNotificationPosition.x, -pos, _originalNotificationPosition.z), _movementNotificationSpeed);
    }

    private IEnumerator MoveNotification()
    {
        // Set start position
        //_originalNotificationPosition = notification.gameObject.GetComponent<RectTransform>().position;
        //float pos = notification.gameObject.GetComponent<RectTransform>().rect.height / 2;

        // Show notification
        //LeanTween.move(notification.gameObject, new Vector3(_originalNotificationPosition.x, pos, _originalNotificationPosition.z), _movementNotificationSpeed);
        //yield return new WaitForSeconds(_movementNotificationSpeed + 2.5f);

        // Hide notification
        //LeanTween.move(notification.gameObject, new Vector3(_originalNotificationPosition.x, -pos, _originalNotificationPosition.z), _movementNotificationSpeed);
        ShowNotification();
        yield return new WaitForSeconds(_movementNotificationSpeed + 2.5f);
        HideNotification();
    }

    #endregion

    // -----------------------------

    public void OpenQuitPopUp()
    {
        if (_savableDataController.SaveFileExists())
        {
            //lastSaveText.text = string.Format("Last saved at {0}.", File.GetCreationTimeUtc(_savableDataController._saveFile.));
        }
        else
        {
            lastSaveText.text = "No saves found.";
        }

        quitPopUp.SetActive(true);
    }
    public void CloseQuitPopUp()
    {
        quitPopUp.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
