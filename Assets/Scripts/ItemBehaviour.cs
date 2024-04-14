using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour
{
    [SerializeField] public Item item;
    private Vector3 offset;

    private void Start()
    {
        UpdateGraphics();
    }

    public void UpdateGraphics()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = item.graphics;
    }

    void OnMouseDown()
    {
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        AudioController.instance.PlaySound(0, 0.5f); //click
    }

    void OnMouseDrag()
    {
        Vector2 curPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        transform.GetChild(0).position = curPosition;
    }

    private void OnMouseUp()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.CompareTag("Cauldron"))
            {
                SpriteRenderer cauldronSpriteRenderer = hit.collider.gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
                ParticleSystem cauldronParticleSystem = GameController.instance.cauldronParticleSystem;

                Color existingColor = cauldronSpriteRenderer.color;

                List<ColorAction> colorActions = item.actions.OfType<ColorAction>().ToList();

                if (colorActions.Count > 0)
                {
                    foreach (ColorAction colorAction in colorActions)
                    {
                        (Color colorToAdd, PaletteColor paletteColor) = MonsterColor.GetColor(colorAction.color);

                        //Mixing mixing
                        Color mixedColor = new Color(
                            (existingColor.r + colorToAdd.r) / 2,
                            (existingColor.g + colorToAdd.g) / 2,
                            (existingColor.b + colorToAdd.b) / 2,
                            1
                        );

                        if (cauldronParticleSystem != null)
                        {
                            var mainModule = cauldronParticleSystem.main;
                            mainModule.startColor = mixedColor;
                        }

                        cauldronSpriteRenderer.color = mixedColor;
                        existingColor = mixedColor;
                    }
                }

                AudioController.instance.PlaySound(6, 0.6f); //water
                MonsterController.instance.addedItems.Add(item);
                Destroy(gameObject);
                return;
            }
        }

        AudioController.instance.PlaySound(2); //on_floor
        transform.GetChild(0).position = transform.position;
    }
}
