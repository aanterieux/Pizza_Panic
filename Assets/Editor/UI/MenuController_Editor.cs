using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MenuController))]
public class MenuController_Editor : Editor
{
    private MenuController m_target = null;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        m_target = (MenuController)(target);

        if (!m_target)
        {
            return;
        }

        GUILayoutOption width = GUILayout.Width(100f);

        GUILayout.BeginHorizontal(width);
            if (GUILayout.Button("Select next"))
            {
                m_target.SelectNextElement();
            }

            if (GUILayout.Button("Select previous"))
            {
                m_target.SelectPreviousElement();
            }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(width);
            if (GUILayout.Button("Interact with current"))
            {
                m_target.InteractWithSelectedElement();
            }

            if (GUILayout.Button("Deselect current"))
            {
                m_target.DeselectCurrentElement();
            }
        GUILayout.EndHorizontal();
    }
}
