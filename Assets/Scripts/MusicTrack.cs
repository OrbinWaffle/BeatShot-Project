
//A data object that simply holds relevant information about a music track

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class MusicTrack : ScriptableObject
{
    
    [Tooltip("The actual audio file.")]
    [SerializeField] public AudioClip song;
    [Tooltip("The BPM of the song.")]
    [SerializeField] public float BPM = 120;
    [Tooltip("Shifts when the first beat is registered.")]
    [SerializeField] public float offset = 0;
    #if UNITY_EDITOR
        [MenuItem("Assets/Create/Music Track")]
        public static void CreateBlobTile()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Music Track", "New Music Track", "asset", "Save Music Track", "Assets");
            if (path == "")
                return;
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<MusicTrack>(), path);
        }
    #endif
}
