using UnityEngine;

public class ItemInteraction : MonoBehaviour
{
    public float interactRange = 3f;
    public LayerMask pickupMask;
    public Camera playerCamera;
    public HotbarManager hotbar;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickup();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            hotbar.DropCurrentItem();
        }
    }

    void TryPickup()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, pickupMask))
        {
            ItemPickup pickup = hit.collider.GetComponent<ItemPickup>();
            if (pickup != null && pickup.itemData != null)
            {
                bool success = hotbar.EquipItemToFirstEmptySlot(pickup.itemData);
                if (success)
                {
                    Destroy(pickup.gameObject); // Remove world object
                }
                else
                {
                    Debug.Log("No space in hotbar!");
                }
            }
        }
    }
}
