using UnityEngine;
using UnityEditor;
using System.IO;
using System.Net;
using System.Collections.Generic;

public class MetaJsonGenerator : EditorWindow
{
    private string masterSheetId = "";
    private string masterGid = "0";
    private string metaOutputPath = "Assets/_Proj/Scripts/Editor/Tools/table_meta.json";

    [MenuItem("Tools/Generate Meta JSON")]
    public static void ShowWindow() => GetWindow<MetaJsonGenerator>();

    void OnGUI()
    {
        GUILayout.Label("Meta JSON 자동 생성기", EditorStyles.boldLabel);

        masterSheetId = EditorGUILayout.TextField("Master Sheet ID", masterSheetId);
        masterGid = EditorGUILayout.TextField("Master GID", masterGid);
        metaOutputPath = EditorGUILayout.TextField("Output JSON Path", metaOutputPath);

        if (GUILayout.Button("Generate meta.json"))
        {
            GenerateMetaJson();
        }
    }

    private void GenerateMetaJson()
    {
        if (string.IsNullOrEmpty(masterSheetId))
        {
            Debug.LogError("Master Sheet ID를 입력하세요.");
            return;
        }

        string url =
            $"https://docs.google.com/spreadsheets/d/{masterSheetId}/export?format=csv&gid={masterGid}";

        string csv;
        using (WebClient wc = new WebClient())
            csv = wc.DownloadString(url);

        string[] lines = csv.Split('\n');

        var entries = new List<TableMetaEntry>();

        for (int i = 1; i < lines.Length; i++)
        {
            var cols = lines[i].Split(',');

            if (cols.Length < 3) continue;

            string name = cols[0].Trim();
            string type = cols[1].Trim();
            string gid = cols[2].Trim();

            if (string.IsNullOrEmpty(name)) continue;

            string exportUrl = gid;

            entries.Add(new TableMetaEntry
            {
                name = name,
                type = type,
                url = exportUrl
            });
        }

        var wrapper = new TableMetaList { entries = entries };

        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(metaOutputPath, json);

        AssetDatabase.Refresh();

        Debug.Log("meta.json 생성 완료: " + metaOutputPath);
    }
}