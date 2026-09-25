using UnityEngine;

public class PlayerInventory : PlayerComponent
{
    [SerializeField] private Item m_currentItem = null;

    private Item m_previousItem = null;

    public Item CurrentItem
    {
        get => m_currentItem;
    }

    private void OnValidate()
    {
        if (m_previousItem != m_currentItem)
        {
            SetCurrentItem(m_currentItem);

            if (m_currentItem)
            {
                m_currentItem.OnPickup(CamTransform_);
            }

            if (m_previousItem)
            {
                m_previousItem.OnRelease();
            }

            m_previousItem = m_currentItem;
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

        if (m_currentItem)
        {
            m_currentItem.OnPickup(CamTransform_);
        }
    }
    public void ClearCurrentItem()
    {
        m_currentItem = null;
    }
}
