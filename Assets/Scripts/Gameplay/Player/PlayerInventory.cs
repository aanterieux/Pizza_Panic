using UnityEngine;

public class PlayerInventory : PlayerComponent
{
    [SerializeField] private Item m_currentItem = null;

    private Item m_currentItemCpy = null;

    public Item m_CurrentItem
    {
        get => m_currentItem;
    }

    private void OnValidate()
    {
        if (m_currentItemCpy != m_currentItem)
        {
            SetCurrentItem(m_currentItem);

            if (m_currentItem)
            {
                m_currentItem.OnPickup(m_CamTransform_);
            }

            m_currentItemCpy = m_currentItem;
        }
    }


    public void SetCurrentItem(Item _item)
    {
        if (m_currentItem == _item)
        {
            return;
        }

        if (m_currentItem)
        {
            m_currentItem.OnRelease();
        }

        m_currentItem = _item;
    }
    public void ClearCurrentItem()
    {
        m_currentItem = null;
    }
}
