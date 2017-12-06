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

[CustomEditor(typeof(JsonableScriptableObject))]
public class JsonableScriptableObjectInspector : Editor 
{
    protected JsonableScriptableObjectConfig m_config;
    protected string []                      m_pathInfoList;
    protected int                            m_pathInfoTarget;
    protected Stack<Action>                  m_callbackQueue;
    protected bool                           m_isTermPollCallbackQueue;

    protected virtual void OnEnable()
    {
	//var jso = target as JsonableScriptableObject;
	m_callbackQueue   = new Stack<Action>();
	m_isTermPollCallbackQueue = false;

	// Load JsonableScriptableObject Editor Settings
	{	    
	    var guids = UnityEditor.AssetDatabase.FindAssets("t:JsonableScriptableObjectConfig");
	    if (guids.Length == 0) {
		//throw new System.IO.FileNotFoundException ("JsonableScriptableObjectConfig does not found");
		var dir = "Assets/AmPlugins/Settings/";
		if(!Directory.Exists(dir)){ Directory.CreateDirectory(dir); }
		var asset = CreateInstance<JsonableScriptableObjectConfig>();
		AssetDatabase.CreateAsset
		    (asset, "Assets/AmPlugins/Settings/JsonableScriptableObjectConfig.asset");
		AssetDatabase.Refresh();
		guids = UnityEditor.AssetDatabase.FindAssets("t:JsonableScriptableObjectConfig");
	    }
	    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
	    m_config = AssetDatabase.LoadAssetAtPath<JsonableScriptableObjectConfig>(path);
	}

	// Cache JsonableScriptableObject pathInfoList
	// @todo フォルダの命名規則に合わせて、デフォルトのTargetをサーチしてセットする
	{
	    m_pathInfoList   = m_config.selectableList;
	    m_pathInfoTarget = 0;	    
	}
    }

    protected virtual void DrawCustomInspector(){
	DrawDefaultInspector(); 
    }
    
    public override void OnInspectorGUI()
    {
	var jso = target as JsonableScriptableObject;
	DrawConvertMenu(jso);
	DrawCustomInspector();
    }

    void PollCallbackQueue(){
	while(m_callbackQueue.Count > 0){ (m_callbackQueue.Pop())(); }
	if(! m_isTermPollCallbackQueue){ EditorApplication.delayCall += PollCallbackQueue; }
    }

    /*=================================================================================================*/

    protected string GetJsonFile(){
	string json = "";
	var jso  = target as JsonableScriptableObject;
	var path = m_config.pathInfoList[m_pathInfoTarget].workspacePath + "/" + jso.name + ".json";
	if(! File.Exists(path)){ Debug.Log(path + " Not Found !"); }
	else {	    
	    using(var sr = new StreamReader(path, Encoding.GetEncoding("utf-8"))){ json = sr.ReadToEnd(); }
	}
	return json;
    }
    protected void PutJsonFile(string json){
	var jso  = target as JsonableScriptableObject;
	var path = m_config.pathInfoList[m_pathInfoTarget].workspacePath + "/" + jso.name + ".json";
	using(var sw = new StreamWriter(path, false, Encoding.GetEncoding("utf-8"))){ sw.Write(json); }
    }
    protected void CopyFromS3(Action fCompleteCallback){
	var jso  = target as JsonableScriptableObject;
	var info = m_config.pathInfoList[m_pathInfoTarget];
	var dst  = info.workspacePath + "/" + jso.name + ".json";
	var src  = "s3://" + info.s3Bucket + info.s3Folder + "/" + jso.name + ".json";
	S3Command.Put(m_config.s3CliSetting, src, dst, fCompleteCallback);
    }
    protected void CopyToS3(Action fCompleteCallback){
	var jso  = target as JsonableScriptableObject;
	var info = m_config.pathInfoList[m_pathInfoTarget];
	var src  = info.workspacePath + "/" + jso.name + ".json";
	var dst  = "s3://" + info.s3Bucket + info.s3Folder + "/" + jso.name + ".json";
	S3Command.Put(m_config.s3CliSetting, src, dst, fCompleteCallback);
    }
    protected virtual void ImportFromJson(){}    
    protected virtual void ExportToJson(){}

    protected virtual void DrawConvertMenu(JsonableScriptableObject jso){

	if(m_config.pathInfoList.Count == 0){
	    EditorGUILayout.BeginHorizontal();
	    {	    
		var style = new GUIStyle();
		style.alignment   = TextAnchor.MiddleLeft;
		style.fixedHeight = 32;
		style.richText    = true;

		GUILayout.Label(SystemIconManager.instance.GetIconTexture(SystemIcon.IconType.Warn),
				GUILayout.Width(32));
		GUILayout.Label("<color=#FF0000>Please Add Deploy Setting !</color>", style);
	    }
	    EditorGUILayout.EndHorizontal();
	    
	    GUILayout.Space(5);	
	    GUILayout.Box(GUIContent.none, HrStyle.EditorLine, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
	    GUILayout.Space(5);	    
	    return;
	}
	
	m_pathInfoTarget = EditorGUILayout.Popup("Target", m_pathInfoTarget, m_pathInfoList);

	var info = m_config.pathInfoList[m_pathInfoTarget];
	DrawSimpleLabelField("JsonDirPath : ", info.workspacePath);
	DrawSimpleLabelField("S3 Path : ",     "s3://" + info.s3Bucket + info.s3Folder);
	DrawSimpleLabelField("JsonName : ",    jso.name + ".json");
	EditorGUILayout.BeginHorizontal();
	{
	    if(GUILayout.Button("Import", EditorStyles.miniButton)){
		ImportFromJson();
		EditorUtility.SetDirty(jso);
		EditorUtility.DisplayDialog("JsonImport", "Complete", "OK");	
	    }
	    if(GUILayout.Button("Export", EditorStyles.miniButton)){
		ExportToJson();
		//EditorUtility.SetDirty(jso);
		EditorUtility.DisplayDialog("JsonExport", "Complete", "OK");			
	    }
	    if(GUILayout.Button("S3 Load", EditorStyles.miniButton)){
		CopyFromS3(() => 
			{ 
			    m_callbackQueue.Push(() => { 
				    ImportFromJson();
				    EditorUtility.SetDirty(jso);
				    EditorUtility.DisplayDialog("S3 Load", "Complete", "OK"); 
				    m_isTermPollCallbackQueue = true;
				}); 
			});
		m_isTermPollCallbackQueue = false;
		EditorApplication.delayCall += PollCallbackQueue;
	    }
	    if(GUILayout.Button("S3 Save", EditorStyles.miniButton)){
		ExportToJson(); // 一応
		CopyToS3(() => 
			{ 
			    m_callbackQueue.Push(() => { 
				    EditorUtility.DisplayDialog("S3 Save", "Complete", "OK"); 
				    m_isTermPollCallbackQueue = true;
				}); 
			});
		m_isTermPollCallbackQueue = false;
		EditorApplication.delayCall += PollCallbackQueue;
	    }
	}
	EditorGUILayout.EndHorizontal();
	GUILayout.Space(5);	
	GUILayout.Box(GUIContent.none, HrStyle.EditorLine, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
	GUILayout.Space(5);	
    }

    protected void DrawSimpleLabelField(string label, string value = "",
					float defaultLabelWidth = 80f)
    {
	EditorGUILayout.BeginHorizontal();
	{
	    EditorGUILayout.LabelField(label, GUILayout.Width(defaultLabelWidth));
	    if(value != ""){
		EditorGUILayout.LabelField(value);
	    }
	}
	EditorGUILayout.EndHorizontal();	
    }
    
    protected void DrawSimpleTextField(JsonableScriptableObject jso, string label, ref string value,
				       float defaultLabelWidth = 80f)
    {
	EditorGUILayout.BeginHorizontal();
	{
	    EditorGUILayout.LabelField(label, GUILayout.Width(defaultLabelWidth));
	    var input = EditorGUILayout.TextField(value);
	    if(input != value){
		Undo.RegisterCompleteObjectUndo(jso, label + " Change");
		value = input;
		EditorUtility.SetDirty(jso);
	    }
	}
	EditorGUILayout.EndHorizontal();	
    }
    
    protected void DrawSimpleIntField(JsonableScriptableObject jso, string label, ref int value,
				       float defaultLabelWidth = 80f)
    {
	EditorGUILayout.BeginHorizontal();
	{
	    EditorGUILayout.LabelField(label, GUILayout.Width(defaultLabelWidth));
	    var input = EditorGUILayout.IntField(value);
	    if(input != value){
		Undo.RegisterCompleteObjectUndo(jso, label + " Change");
		value = input;
		EditorUtility.SetDirty(jso);
	    }
	}
	EditorGUILayout.EndHorizontal();	
    }
    
}
}    

/*
 * Local variables:
 * compile-command: "make -C../ deploy"
 * End:
 */
