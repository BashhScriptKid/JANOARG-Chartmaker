using System;
using System.Collections.Generic;
using System.Linq;
using JANOARG.Chartmaker.UI.Themeable;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace JANOARG.Chartmaker.Editor
{
    public class VersionBumper : EditorWindow
    {
        [MenuItem("JANOARG/Version Bumper", priority = 200)]
        public static void ShowWindow() 
        {
            VersionBumper window = GetWindow<VersionBumper>();
            window.minSize = new Vector2(200, 200);
            window.titleContent = new GUIContent(window.name = "Version Bumper");
            window.Show();
        }

        public void BumpVersion(int index)
        {
            List<string> versionParts = new(PlayerSettings.bundleVersion.Split("."));
            while (versionParts.Count < index + 1) {
                versionParts.Add("0");
            }
            while (versionParts.Count > Mathf.Max(2, index + 1)) {
                versionParts.RemoveAt(versionParts.Count - 1);
            }
            versionParts[index] = (int.Parse(versionParts[index]) + 1).ToString();
            PlayerSettings.bundleVersion = string.Join('.', versionParts);
        }

        public void OnGUI()
        {
            GUILayout.Label("Current version: ");
            GUILayout.Label(PlayerSettings.bundleVersion, EditorStyles.boldLabel);

            EditorGUILayout.Space();
            GUILayout.Label("Bump: ");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Major", EditorStyles.miniButtonLeft)) BumpVersion(0);
            if (GUILayout.Button("Minor", EditorStyles.miniButtonMid)) BumpVersion(1);
            if (GUILayout.Button("Patch", EditorStyles.miniButtonRight)) BumpVersion(2);
            GUILayout.EndHorizontal();
        }
    }
}