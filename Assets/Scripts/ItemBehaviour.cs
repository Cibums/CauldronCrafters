using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour
{
    [SerializeField] public Item item;

    private Vector3 offset;

    private void Start()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = item.graphics;
    }

    void OnMouseDown()
    {
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
    }

    void OnMouseDrag()
    {
        Vector2 curPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        transform.GetChild(0).position = curPosition;
    }

    private void OnMouseUp()
    {
        Debug.Log(item.GetActionsText());

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.CompareTag("Cauldron"))
            {
                MonsterController.instance.addedItems.Add(item);
                Destroy(gameObject);
                return;
            }
        }

        transform.GetChild(0).position = transform.position;
    }
}
