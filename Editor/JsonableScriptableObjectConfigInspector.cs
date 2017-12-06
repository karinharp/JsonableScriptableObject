using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace am
{

[CustomEditor(typeof(JsonableScriptableObjectConfig))]
public class JsonableScriptableObjectConfigInspector : Editor 
{
    
    protected virtual void DrawCustomInspector(){
	DrawDefaultInspector(); 
    }
    
    public override void OnInspectorGUI()
    {
	DrawConvertMenu();
	DrawCustomInspector();
    }

    protected string GetJsonFile(string path){
	string json = "";
	using(var sr = new StreamReader(path, Encoding.GetEncoding("utf-8"))){ json = sr.ReadToEnd(); }
	return json;
    }
    protected void PutJsonFile(string path, string json){
	using(var sw = new StreamWriter(path, false, Encoding.GetEncoding("utf-8"))){ sw.Write(json); }
    }
    
    protected virtual void ImportFromJson(string path){
	var config  = target as JsonableScriptableObjectConfig;
	JsonUtility.FromJsonOverwrite(GetJsonFile(path), config);
    }
    protected virtual void ExportToJson(string path){
	var config  = target as JsonableScriptableObjectConfig;
	PutJsonFile(path, JsonUtility.ToJson(config, false));
    }
    
    protected virtual void DrawConvertMenu(){

	EditorGUILayout.BeginHorizontal();
	{
	    if(GUILayout.Button("Import", EditorStyles.miniButton)){
		var path = EditorUtility.OpenFilePanel
		    ("Select SavePath", Application.dataPath, "json");
		if(path != ""){
		    ImportFromJson(path);
		    EditorUtility.SetDirty(target);
		    EditorUtility.DisplayDialog("JsonImport", "Complete", "OK");
		}
	    }
	    if(GUILayout.Button("Export", EditorStyles.miniButton)){
		var path = EditorUtility.SaveFilePanel
		    ("Select SavePath", Application.dataPath, "JsonableScriptableObjectConfig", "json");
		if(path != ""){
		    ExportToJson(path);
		    //EditorUtility.SetDirty(jso);
		    EditorUtility.DisplayDialog("JsonExport", "Complete", "OK");
		}
	    }
	}
	EditorGUILayout.EndHorizontal();
	GUILayout.Space(5);	
	GUILayout.Box(GUIContent.none, HrStyle.EditorLine, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
	GUILayout.Space(5);	
    }
    
}
}    

/*
 * Local variables:
 * compile-command: "make -C../ deploy"
 * End:
 */
