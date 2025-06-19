using System;
using UnityEditor;
using UnityEngine;

public class CloseButton : MonoBehaviour
{
    public static void CloseApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
