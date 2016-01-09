using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

public class GeneralObjectHelperWindow : EditorWindow
{

    [MenuItem("Window/Utility/General Object Utility")]
    static void Init()
    {
        GeneralObjectHelperWindow window = (GeneralObjectHelperWindow)EditorWindow.GetWindow(typeof(GeneralObjectHelperWindow));
        window.Show();
    }

    private GameObject target;
    private HelperWindowMethod _method;
    private string _addComponentType;
    private GameObject[] _missingFilters;

    public enum HelperWindowMethod
    {
        AddComponent,
        Custom,
        EnableAllRenderers,
        FindMissingFilters
    }

    void OnGUI()
    {
        _method = (HelperWindowMethod)EditorGUILayout.EnumPopup("Helper Method", _method);

        target = EditorGUILayout.ObjectField(target, typeof(GameObject), true) as GameObject;

        switch (_method)
        {
            case HelperWindowMethod.AddComponent:
                _addComponentType = EditorGUILayout.TextField("Component Type", _addComponentType);
                if(GUILayout.Button("Add Components"))
                {
                    AddComponentMethod(_addComponentType);
                }
                break;
            case HelperWindowMethod.Custom:
                if(GUILayout.Button("Do Work"))
                {
                    Custom();
                }
                break;
            case HelperWindowMethod.EnableAllRenderers:
                if(GUILayout.Button("Enable All Renderers"))
                {
                    EnableAllRenderers();
                }
                break;
            case HelperWindowMethod.FindMissingFilters:
                if(GUILayout.Button("Find Missing Filters"))
                {
                    FindMissingFilters();
                }
                if(_missingFilters != null)
                {
                    foreach(var f in _missingFilters)
                    {
                        EditorGUILayout.ObjectField(f, typeof(GameObject));
                    }
                }
                break;
        }
    }

    private void AddComponentMethod(string component)
    {
        Type type = Type.GetType(component);
        if(type == null)
        {
            Debug.Log("Invalid Type: " + component);
            return;
        }
        GameObject go = target.gameObject;
        var objs = go.GetComponentsInChildren<GameObject>(true);
        foreach (var o in objs)
        {
            o.AddComponent(type);
        }
        Debug.Log("Components added on " + objs.Length + " object(s)");
    }

    private void Custom()
    {
        var objs = target.GetComponentsInChildren<Transform>(true);
        foreach(var o in objs)
        {
            o.gameObject.AddComponent<StandardVoidFX>();
        }
        Debug.Log("Completed");
    }

    private void EnableAllRenderers()
    {
        foreach(var g in GameObject.FindObjectsOfType<MeshRenderer>())
        {
            g.enabled = true;
        }
    }

    private void FindMissingFilters()
    {
        var filters = new List<GameObject>();
        foreach(var f in GameObject.FindObjectsOfType<MeshFilter>())
        {
            if(f.sharedMesh == null)
            {
                filters.Add(f.gameObject);
            }
        }
        _missingFilters = filters.ToArray();
    }
}
