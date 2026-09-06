using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private Item currentItem = null;

    private Item currentItemCpy = null;

    public Item CurrentItem
    {
        get => currentItem;
    }

    private void OnValidate()
    {
        if (currentItemCpy != currentItem)
        {
            currentItem.OnPickup();

            currentItemCpy = currentItem;
        }
    }

    public void SetCurrentItem(Item _item)
    {
        currentItem = _item;
    }
}
