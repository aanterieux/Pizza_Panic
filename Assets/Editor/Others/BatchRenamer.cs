using UnityEngine;
using UnityEditor;

namespace Antony
{
    public class BatchRenamer : EditorWindow
    {
        private string newName = "";
        private string partToRemove = "";
        private string partToReplace = "";
        private string replacement = "";
        private string partToAdd = "";
        private string addLocation = "";
        private bool addToStart = false;

        [MenuItem("Tools/Batch rename")]
        public static void ShowWindow()
        {
            BatchRenamer window = GetWindow<BatchRenamer>("Batch Renamer");
            Vector2 minSize = window.minSize;
            minSize.y = 350f;
            window.minSize = minSize;
        }

        private void OnGUI()
        {
            // Rename section //
            newName = EditorGUILayout.TextField("New name :", newName);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Rename"))
            {
                RenameSelectedObjects();
            }

            // ---- //
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            // ---- //

            // Remove section //
            partToRemove = EditorGUILayout.TextField("Part to remove :", partToRemove);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Remove from name"))
            {
                RemovePartFromName();
            }

            // ---- //
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            // ---- //

            // Replace section //
            partToReplace = EditorGUILayout.TextField("Part to replace :", partToReplace);
            replacement = EditorGUILayout.TextField("Replacement :", replacement);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Replace in name"))
            {
                RenamePart();
            }

            // ---- //
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            // ---- //

            // Adding part //
            EditorGUILayout.LabelField("Leave empty to append");
            addLocation = EditorGUILayout.TextField("Where to add :", addLocation);
            partToAdd = EditorGUILayout.TextField("What to add :", partToAdd);
            addToStart = EditorGUILayout.Toggle("Add to start instead", addToStart);

            if (GUILayout.Button("Add to name"))
            {
                AddPart(addToStart);
            }
        }

        private void RenameSelectedObjects()
        {
            string nameCopy = newName;

            // Rename objects in Assets
            if (Selection.activeObject != null && AssetDatabase.Contains(Selection.activeObject))
            {
                string path = "";
                for (int i = 0; i < Selection.objects.Length; ++i)
                {
                    if (i > 0)
                    {
                        newName = nameCopy + $" ({i})";
                    }

                    path = AssetDatabase.GetAssetPath(Selection.objects[i]);
                    AssetDatabase.RenameAsset(path, newName);
                }
                AssetDatabase.SaveAssets();
            }
            // Rename objects in Scene
            else
            {
                GameObject obj = null;

                for (int i = 0; i < Selection.gameObjects.Length; ++i)
                {
                    obj = Selection.gameObjects[i];

                    if (i > 0)
                    {
                        newName = nameCopy + $" ({i})";
                    }

                    Undo.RecordObject(obj, "Batch Rename");
                    obj.name = newName;
                    EditorUtility.SetDirty(obj);
                }
            }
        }

        private void RemovePartFromName()
        {
            // Remove part from name of objects in Assets
            if (Selection.activeObject != null && AssetDatabase.Contains(Selection.activeObject))
            {
                string path = "";
                string originalName = "";
                string truncatedName = "";
                foreach (Object obj in Selection.objects)
                {
                    originalName = obj.name;

                    if (originalName.Contains(partToRemove))
                    {
                        truncatedName = originalName.Replace(partToRemove, "");

                        path = AssetDatabase.GetAssetPath(obj);
                        AssetDatabase.RenameAsset(path, truncatedName);
                    }
                }
                AssetDatabase.SaveAssets();
            }
            // Remove part from name of objects in Scene
            else
            {
                foreach (GameObject obj in Selection.gameObjects)
                {
                    string originalName = obj.name;

                    if (originalName.Contains(partToRemove))
                    {
                        string truncatedName = originalName.Replace(partToRemove, "");

                        Undo.RecordObject(obj, "Remove from name");
                        obj.name = truncatedName;
                        EditorUtility.SetDirty(obj);
                    }
                }
            }
        }

        private void RenamePart()
        {
            // Remove part from name of objects in Assets
            if (Selection.activeObject != null && AssetDatabase.Contains(Selection.activeObject))
            {
                string path = "";
                string originalName = "";
                string updatedName = "";
                foreach (Object obj in Selection.objects)
                {
                    originalName = obj.name;

                    if (originalName.Contains(partToReplace))
                    {
                        updatedName = originalName.Replace(partToReplace, replacement);

                        path = AssetDatabase.GetAssetPath(obj);
                        AssetDatabase.RenameAsset(path, updatedName);
                    }
                }
                AssetDatabase.SaveAssets();
            }
            // Remove part from name of objects in Scene
            else
            {
                foreach (GameObject obj in Selection.gameObjects)
                {
                    string originalName = obj.name;

                    if (originalName.Contains(partToReplace))
                    {
                        string updatedName = originalName.Replace(partToReplace, replacement);

                        Undo.RecordObject(obj, "Replace part of name");
                        obj.name = updatedName;
                        EditorUtility.SetDirty(obj);
                    }
                }
            }
        }

        private void AddPart(in bool _addToStart)
        {
            // Add part in name of objects in Assets
            if (Selection.activeObject != null && AssetDatabase.Contains(Selection.activeObject))
            {
                string path = "";
                string originalName = "";
                string updatedName = "";
                foreach (Object obj in Selection.objects)
                {
                    originalName = obj.name;

                    if (addLocation.Length == 0)
                    {
                        updatedName = originalName.Insert(
                            (_addToStart) ? 0 : originalName.Length,
                            partToAdd
                        );
                    }
                    else if (originalName.Contains(addLocation))
                    {
                        int count = originalName.IndexOf(addLocation[0]);

                        updatedName = originalName.Insert(
                            (_addToStart)
                                ? count
                                : count + addLocation.Length,
                            partToAdd
                        );
                    }

                    path = AssetDatabase.GetAssetPath(obj);
                    AssetDatabase.RenameAsset(path, updatedName);
                }
                AssetDatabase.SaveAssets();
            }
            // Add part in name of objects in Scene
            else
            {
                foreach (GameObject obj in Selection.gameObjects)
                {
                    string originalName = obj.name;
                    string updatedName = "";

                    if (addLocation.Length == 0)
                    {
                        updatedName = originalName.Insert(
                            (_addToStart) ? 0 : originalName.Length,
                            partToAdd
                        );
                    }
                    else if (originalName.Contains(addLocation))
                    {
                        int count = originalName.IndexOf(addLocation[0]);

                        updatedName = originalName.Insert(
                            (_addToStart)
                                ? count
                                : count + addLocation.Length,
                            partToAdd
                        );
                    }

                    Undo.RecordObject(obj, "Add part to name");
                    obj.name = updatedName;
                    EditorUtility.SetDirty(obj);
                }
            }
        }
    }
}