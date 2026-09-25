using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerInventory))]
public class PlayerInventory_Editor : Editor
{
    private PlayerInventory m_target = null;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        m_target = (PlayerInventory)(target);

        if (!m_target)
        {
            return;
        }

        GUILayoutOption width = GUILayout.Width(100f);

        GUILayout.BeginHorizontal(width);
            if (GUILayout.Button("Pickup Gun"))
            {
                m_target.SetCurrentItem(GetRandomItem<Gun>());
            }

            if (GUILayout.Button("Pickup Holdable"))
            {
                m_target.SetCurrentItem(GetRandomItem<Holdable>());
            }

            if (GUILayout.Button("Pickup Consumable"))
            {
                m_target.SetCurrentItem(GetRandomItem<Consumable>());
            }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(width);
            if (GUILayout.Button("Pickup random"))
            {
                m_target.SetCurrentItem(GetRandomItem<Item>());
            }

            if (GUILayout.Button("Release item"))
            {
                m_target.SetCurrentItem(null);
            }
        GUILayout.EndHorizontal();
    }


    private Item GetRandomItem<T>() where T : Item
    {
        Item[] items = FindObjectsByType<Item>();
        T[] matchingItems = items.OfType<T>().ToArray();
        int itemNb = matchingItems.Length;

        if (itemNb == 0)
        {
            return null;
        }

        return matchingItems[Random.Range(0, itemNb)];
    }
}
