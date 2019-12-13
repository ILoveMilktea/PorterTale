using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 이제 여기서 Tool 부분 만들어야 함.
/// </summary>
public class ExcelWindow : EditorWindow
{
    string fileName = "";

    [MenuItem("Tool/ExcelReader")]
    static public void CreateWindow()
    {
        EditorWindow window = (ExcelWindow)EditorWindow.GetWindow(typeof(ExcelWindow));
    }

    private void OnGUI()
    {

        if (GUILayout.Button("Set Tables", GUILayout.Height(30)))
        {
            ExcelReader.SetTables();
            Repaint();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        fileName = GUILayout.TextField(fileName, GUILayout.Height(30));

        if (GUILayout.Button("Set Table in Merge Mode", GUILayout.Height(30)))
        {
            ExcelReader_MergeMode.SetTables(fileName);
            Repaint();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
        //GUILayout.BeginArea(new Rect(10, 10, 600, 400));
        //{
            
        //}
        //GUILayout.EndArea();

    }

}