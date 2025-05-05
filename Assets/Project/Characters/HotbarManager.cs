using UnityEngine;

public class HotbarManager : MonoBehaviour
{
    [Header("Hotbar")]
    public int hotbarSize = 5;
    public ItemData[] heldItems;         // Track item data, not just prefabs
    public Transform handSlot;           // Where equipped item is shown
    public int currentSlot = 0;

    private GameObject currentItemInstance;

    void Start()
    {
        heldItems = new ItemData[hotbarSize];
        EquipSlot(currentSlot);
    }

    void Update()
    {
        HandleScrollInput();
        HandleNumberKeys();
    }

    public void HandleScrollInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            currentSlot = (currentSlot + 1) % hotbarSize;
            EquipSlot(currentSlot);
        }
        else if (scroll < 0f)
        {
            currentSlot = (currentSlot - 1 + hotbarSize) % hotbarSize;
            EquipSlot(currentSlot);
        }
    }

    void HandleNumberKeys()
    {
        for (int i = 0; i < hotbarSize; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                currentSlot = i;
                EquipSlot(currentSlot);
            }
        }
    }

    void EquipSlot(int index)
    {
        if (currentItemInstance != null)
            Destroy(currentItemInstance);

        ItemData data = heldItems[index];
        if (data != null && data.heldPrefab != null)
        {
            currentItemInstance = Instantiate(data.heldPrefab, handSlot);
            currentItemInstance.transform.localPosition = Vector3.zero;
            currentItemInstance.transform.localRotation = Quaternion.identity;
        }
    }

    public void EquipItem(ItemData data, int slot)
    {
        if (slot < 0 || slot >= hotbarSize || data == null) return;

        heldItems[slot] = data;

        if (slot == currentSlot)
        {
            EquipSlot(slot);
        }
    }

    public bool EquipItemToFirstEmptySlot(ItemData data)
    {
        for (int i = 0; i < hotbarSize; i++)
        {
            if (heldItems[i] == null)
            {
                EquipItem(data, i);
                return true;
            }
        }
        return false; // No empty slot
    }

    public void DropCurrentItem()
    {
        ItemData data = heldItems[currentSlot];
        if (data == null || data.pickupPrefab == null) return;

        Instantiate(data.pickupPrefab, transform.position + transform.forward * 1.5f, Quaternion.identity);
        heldItems[currentSlot] = null;
        EquipSlot(currentSlot);
    }

    public ItemData GetItemDataAtSlot(int index)
    {
        if (index >= 0 && index < hotbarSize)
            return heldItems[index];
        return null;
    }

    public int GetCurrentSlotIndex()
    {
        return currentSlot;
    }
}
