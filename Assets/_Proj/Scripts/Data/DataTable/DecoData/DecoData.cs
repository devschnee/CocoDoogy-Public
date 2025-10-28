using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[Serializable]
public class DecoData
{
    public int id;
    public string name;
    public string prefabPath;
    public string iconPath;
    public string category;
    public string tag;
    public string acquire;
    public int stack;
    public string description;

    [NonSerialized] public GameObject prefab;
    [NonSerialized] public Sprite icon;
}