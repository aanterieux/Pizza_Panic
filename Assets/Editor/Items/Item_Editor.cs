using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Item), true)]
public class Item_Editor : Editor
{
    private Item m_target = null;

    public override void OnInspectorGUI()
    {
        m_target = (Item)(target);

        if (m_target)
        {
            if (GUILayout.Button("Update initial transform"))
            {
                m_target.UpdateInitialTransform(true);
            }

            GUILayout.Space(10f);
        }

        base.OnInspectorGUI();
    }
}
