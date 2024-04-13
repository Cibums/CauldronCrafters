using UnityEngine;
using UnityEngine.UI;

public class InformationWindow : MonoBehaviour
{
    private GameObject popup;
    private Canvas popupCanvas;
    public Vector3 offset = new Vector3(1, 1, 0);
    private Camera mainCamera;

    private void Awake()
    {
        popup = GameObject.FindGameObjectWithTag("InformationWindow");
        popupCanvas = GameObject.FindGameObjectWithTag("PopupCanvas").GetComponent<Canvas>();

        mainCamera = Camera.main;
    }

    private void Start()
    {
        if (popup != null)
            popup.SetActive(false);
    }

    private void UpdatePopupPosition()
    {
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(transform.position + offset);
        Vector2 canvasPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(popupCanvas.GetComponent<RectTransform>(), screenPosition, popupCanvas.worldCamera, out canvasPosition);
        popup.GetComponent<RectTransform>().anchoredPosition = canvasPosition;
    }

    private void UpdateText(string title, string content)
    {
        UserInterfaceController.instance.InformationWindowTitleText.SetText(title);
        UserInterfaceController.instance.InformationWindowDescriptionText.SetText(content);
    }

    void OnMouseEnter()
    {
        Debug.Log("Mouse entered on object: " + gameObject.name);
        popup.SetActive(true);
        UpdatePopupPosition();

        Item item = transform.gameObject.GetComponent<ItemBehaviour>().item;
        if (item != null)
        {
            UpdateText(item.itemName, item.GetActionsText());
        }
    }

    void OnMouseExit()
    {
        popup.SetActive(false);
    }
}