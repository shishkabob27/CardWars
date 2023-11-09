using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;


public class CWAudioPrefabHandler : MonoBehaviour
{
    [SerializeField] private string _path;
    [SerializeField] private bool Execute;
    [SerializeField] private List<GameObject> _objects;



    private void OnValidate()
    {
        if (Execute)
        {
            Execute = false;
            GetChildren();
            ConvertPrefabs();
        }
    }

    private void GetChildren()
    {
        _objects = new List<GameObject>();
        foreach (Transform o in transform)
        {
            _objects.Add(o.gameObject);
        }
    }

    private void ConvertPrefabs()
    {
        foreach (var o in _objects)
        {
            var audio = o.GetComponent<AudioSource>();
            if (audio == null)
            {
                EditorApplication.delayCall += () =>
                {
                    DestroyImmediate(o.gameObject);
                };
                continue;
            }

            var newAudioScript = o.AddComponent<CWPlayAudioScript>();
            newAudioScript.clips = new AudioClip[] { audio.clip };
            newAudioScript.playOnStartIndex = audio.clip == null ? -1 : 0;

            EditorApplication.delayCall += () =>
            {
                DestroyImmediate(audio);
/*
                TO DO: Find a way to overwrite Prefabs without changing fileID
                
                Literally can't override prefabs in code because Unity 2017 doesn't have a dedicated "Save" API.
                PrefabUtility.CreatePrefab changes the fileID and breaks the object reference in scenes, 
                SaveAsPrefabAsset is not available in Unity 2017

                EditorApplication.delayCall += () =>
                {
                    OverwritePrefab(o);
                };
*/
            };
        }
    }

    private void OverwritePrefab(GameObject obj)
    {
        //PrefabUtility.SaveAsPrefabAsset(obj, PrefabPath(obj.name));
    }

    private string PrefabPath(string objName)
    {
        return _path + objName + ".prefab";
    }
}
#endif