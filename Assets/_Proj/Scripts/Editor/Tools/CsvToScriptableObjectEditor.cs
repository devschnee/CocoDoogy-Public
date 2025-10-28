using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CsvToScriptableObjectEditor : EditorWindow
{
    string csvPath = "";
    DecoDatabase targetDb;
    Vector2 scroll;


    [MenuItem("Window/Data Tools/CSV -> ScriptableObject")]
    public static void ShowWindow()
    {
        GetWindow<CsvToScriptableObjectEditor>("CSV -> SO");
    }


    void OnGUI()
    {
        GUILayout.Label("CSV -> ScriptableObject Importer", EditorStyles.boldLabel);
        EditorGUILayout.Space();


        EditorGUILayout.LabelField("1) CSV 파일 (UTF-8) :");
        EditorGUILayout.BeginHorizontal();
        csvPath = EditorGUILayout.TextField(csvPath);
        if (GUILayout.Button("Browse", GUILayout.Width(80)))
        {
            string p = EditorUtility.OpenFilePanel("Select CSV file", Application.dataPath, "csv");
            if (!string.IsNullOrEmpty(p)) csvPath = p;
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.Space();
        EditorGUILayout.LabelField("2) Target ScriptableObject (DecoDatabase) :");
        targetDb = (DecoDatabase)EditorGUILayout.ObjectField(targetDb, typeof(DecoDatabase), false);
        EditorGUILayout.Space();


        if (GUILayout.Button("Create New DecoDatabase Asset"))
        {
            string p = EditorUtility.SaveFilePanelInProject("Save Deco Database", "DecoDatabase", "asset", "Create DecoDatabase asset");
            if (!string.IsNullOrEmpty(p))
            {
                DecoDatabase db = ScriptableObject.CreateInstance<DecoDatabase>();
                AssetDatabase.CreateAsset(db, p);
                AssetDatabase.SaveAssets();
                targetDb = db;
            }
        }
        EditorGUILayout.Space();
        scroll = EditorGUILayout.BeginScrollView(scroll);
        if (GUILayout.Button("Import CSV"))
        {
            if (string.IsNullOrEmpty(csvPath) || targetDb == null)
            {
                EditorUtility.DisplayDialog("Error", "CSV path or target DB is not set.", "OK");
            }
            else
            {
                ImportCsvToDecoDatabase(csvPath, targetDb);
            }
        }
        EditorGUILayout.EndScrollView();
    }


    void ImportCsvToDecoDatabase(string path, DecoDatabase db)
    {
        string text = File.ReadAllText(path);
        // 기본적으로 CRLF와 LF를 모두 처리
        string[] lines = text.Replace("\r\n", "\n").Split('\n');
        if (lines.Length < 2)
        {
            EditorUtility.DisplayDialog("Error", "CSV has no data.", "OK");
            return;
        }


        string headerLine = lines[0];
        string[] headers = headerLine.Split(',');


        var newList = new List<DecoData>();
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] cols = ParseCsvLine(lines[i]);
            var d = new DecoData();
            for (int c = 0; c < cols.Length; c++)
            {
                string h = headers[c].Trim();
                string v = cols[c].Trim();
                // 컬럼별 파싱
                try
                {
                    switch (h)
                    {
                        case "id": d.id = int.Parse(v); break;
                        case "name": d.name = v; break;
                        case "prefabPath": d.prefabPath = v; break;
                        case "iconPath": d.iconPath = v; break;
                        case "category": d.category = v; break;
                        case "tag": d.tag = v; break;
                        case "acquire": d.acquire = v; break;
                        case "stack": d.stack = int.Parse(v); break;
                        case "description": d.description = v; break;
                        default: break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to parse cell ({h}) value '{v}' on line {i + 1}: {ex.Message}");
                }
            }
            newList.Add(d);
        }// 덮어쓰기
        db.decos = newList;
        db.ResolveAssets();
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Import Complete", $"Imported {newList.Count} rows.", "OK");
    }


    // 간단한 CSV 파서(필드에 쉼표가 포함되면 "로 감싼 경우만 처리)
    string[] ParseCsvLine(string line)
    {
        var cells = new List<string>();
        bool inQuotes = false;
        var cur = new System.Text.StringBuilder();
        for (int i = 0; i < line.Length; i++)
        {
            char ch = line[i];
            if (ch == '"')
            {
                inQuotes = !inQuotes;
                continue;
            }
            if (ch == ',' && !inQuotes)
            {
                cells.Add(cur.ToString());
                cur.Clear();
            }
            else
            {
                cur.Append(ch);
            }
        }
        cells.Add(cur.ToString());
        return cells.ToArray();
    }
}
