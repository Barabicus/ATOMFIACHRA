using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class StandardComponentInspector : Editor
{

    private MonoBehaviour _objectTarget;

    protected MonoBehaviour ObjectTarget { get { return _objectTarget; } }

    protected virtual void OnEnable()
    {
        _objectTarget = target as MonoBehaviour;
    }

    #region Creation & Get

    /// <summary>
    /// Unity's get child components is not working. Substitute with this
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private List<T> GetChildComponents<T>()
    {
        List<T> comps = new List<T>();
        foreach (Transform t in ObjectTarget.transform)
        {
            T comp = t.GetComponent<T>();
            if (comp != null)
                comps.Add(comp);
        }
        return comps;
    }

    /// <summary>
    /// Unity's get child component is not working. Substitute with this
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private T GetChildComponent<T>()
    {
        foreach (Transform t in ObjectTarget.transform)
        {
            T comp = t.GetComponent<T>();
            if (comp != null)
                return comp;
        }
        return default(T);
    }

    /// <summary>
    /// Draw an option to add more than one component. Will add the component and specify how many components of that time are added
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="title"></param>
    protected void DrawAddComponent<T>(string title) where T : Component
    {
        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField(title + " (" + GetChildComponents<T>().Count + ")", EditorStyles.boldLabel);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Create Effect"))
        {
            GameObject go = new GameObject(title + " (" + GetChildComponents<T>().Count + ")");
            Undo.RegisterCreatedObjectUndo(go, "Created " + title);
            go.layer = LayerMask.NameToLayer("Spell");
            go.AddComponent<T>();
            go.transform.parent = ObjectTarget.transform;
            go.transform.localPosition = Vector3.zero;
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draw an option to add more than one component. Will add the component and specify how many components of that time are added
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="title"></param>
    protected void DrawAddComponent(string title, string createName, Type type)
    {
        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField(title + " (" + ObjectTarget.gameObject.GetComponentsInChildren(type, true).Length + ")", EditorStyles.boldLabel);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Create " + createName))
        {
            GameObject go = new GameObject(title + " (" + ObjectTarget.gameObject.GetComponentsInChildren(type, true).Length + ")");
            Undo.RegisterCreatedObjectUndo(go, "Created " + title);
            go.layer = LayerMask.NameToLayer("Spell");
            go.AddComponent(type);
            go.transform.parent = _objectTarget.transform;
            go.transform.localPosition = Vector3.zero;
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draw an option to add a single component of a specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="associatedObject"></param>
    /// <param name="title"></param>
    protected void DrawHasComponent<T>(string title) where T : Component
    {
        EditorGUILayout.BeginHorizontal("box");
        // Rect rect = EditorGUILayout.GetControlRect();
        Color c;

        var associatedObject = GetChildComponent<T>();

        if (associatedObject == null)
            c = Color.white;
        else
            c = new Color(0, 0.93f, 0);
        GUI.color = c;
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        GUI.color = Color.white;

        if (associatedObject == null)
        {
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Add Motor"))
            {
                GameObject go = new GameObject(title);
                Undo.RegisterCreatedObjectUndo(go, "Created " + title);
                go.layer = LayerMask.NameToLayer("Spell");
                go.AddComponent<T>();
                go.transform.parent = _objectTarget.transform;
                go.transform.localPosition = Vector3.zero;
            }
            GUI.backgroundColor = Color.white;
        }
        EditorGUILayout.EndHorizontal();
    }
    /// <summary>
    /// Draw an option to add a single component of a specified type. Once added it will no longer allow the user
    /// to create any subsquent objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="associatedObject"></param>
    /// <param name="title"></param>
    protected void DrawHasComponent(string title, Type type)
    {
        EditorGUILayout.BeginHorizontal("box");
        Color c;

        var associatedObject = ObjectTarget.gameObject.GetComponentsInChildren(type, true);
        if (associatedObject.Length == 0)
            c = Color.white;
        else
            c = new Color(0, 0.93f, 0);
        GUI.color = c;
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        GUI.color = Color.white;

        if (associatedObject.Length == 0)
        {
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Add Component"))
            {
                GameObject go = new GameObject(title);
                Undo.RegisterCreatedObjectUndo(go, "Created " + title);
                go.layer = LayerMask.NameToLayer("Spell");
                go.AddComponent(type);
                go.transform.parent = _objectTarget.transform;
                go.transform.localPosition = Vector3.zero;
            }
            GUI.backgroundColor = Color.white;
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draw an option to add a component with a specific call back for when it is added. This allows a component
    /// to have a custom drawing property if it is added. For example calling this when there is no component will
    /// prompt the user with a button to add one. Once it is added the draw action call back will control drawing.
    /// This is useful if you have a unique component that you wish to be able to control the draw actions for.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="associatedObject"></param>
    /// <param name="drawAction"></param>
    /// <param name="title"></param>
    protected void DrawAddType<T>(T associatedObject, Action<T> drawAction, string title) where T : UnityEngine.Component
    {
        if (associatedObject != null)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            drawAction(associatedObject);
            EditorUtility.SetDirty(associatedObject);
            EditorGUILayout.EndVertical();
        }
        else
            if (GUILayout.Button(new GUIContent("Add " + title)))
            {
                GameObject go = new GameObject(title);
                go.layer = LayerMask.NameToLayer("Spell");
                go.AddComponent<T>();
                go.transform.parent = _objectTarget.transform;
                go.transform.localPosition = Vector3.zero;
            }
    }

    protected void DrawAddPrefab(string title, GameObject prefab, Action<GameObject> clickCallBack)
    {
        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Create Object"))
        {
            GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            Undo.RegisterCreatedObjectUndo(go, "Created " + title);
            if (clickCallBack != null)
            {
                clickCallBack(go);
            }
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
    }

    #endregion

}
