using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Hotbar/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public GameObject pickupPrefab;  // For dropping in world
    public GameObject heldPrefab;    // For showing in hand
    public Sprite icon;
}
