using UnityEngine;
using UnityEngine.UI;

public class HotbarUI : MonoBehaviour
{
    public Image[] slotImages;     // Background images for slots
    public Image[] itemIcons;      // Icon images inside each slot
    public Color selectedColor = Color.yellow;
    public Color defaultColor = Color.white;

    public HotbarManager hotbarManager;

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            // Highlight selected slot
            slotImages[i].color = (i == hotbarManager.GetCurrentSlotIndex()) ? selectedColor : defaultColor;

            // Show item icon if present
            ItemData data = hotbarManager.GetItemDataAtSlot(i);
            if (data != null && data.icon != null)
            {
                itemIcons[i].sprite = data.icon;
                itemIcons[i].enabled = true;
            }
            else
            {
                itemIcons[i].enabled = false;
            }
        }
    }
}
