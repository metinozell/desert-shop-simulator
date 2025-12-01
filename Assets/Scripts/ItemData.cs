using UnityEngine;

public enum ItemCategory
{
    Ingredient,
    Equipment,
    Decoration
}

[CreateAssetMenu(fileName = "NewItemData", menuName = "Dessert Shop/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea]
    public string itemDescription;
    public float price;
    public Sprite image;
    public ItemCategory category;
    public bool isStackable = true;
}
