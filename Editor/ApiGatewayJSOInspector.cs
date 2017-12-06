using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace am
{

[CustomEditor(typeof(ApiGatewayJSO))]
public class ApiGatewayJSOInspector : JsonableScriptableObjectInspector
{
    protected override void ImportFromJson(){
	var jso  = target as ApiGatewayJSO;
	JsonUtility.FromJsonOverwrite(GetJsonFile(), jso);
    }
    protected override void ExportToJson(){
	var jso  = target as ApiGatewayJSO;
	PutJsonFile(JsonUtility.ToJson(jso, false));
    }

    protected override void DrawCustomInspector(){
	var jso = target as ApiGatewayJSO;
	int idx = 0;
	int nr  = jso.apiList.Count;

	DrawSimpleLabelField("Info API");
	GUILayout.Space(3);	
	//DrawSimpleTextField(jso, "label",     ref jso.infoApi.label);
	//DrawSimpleTextField(jso, "key",       ref jso.infoApi.key);
	DrawSimpleTextField(jso, "url",       ref jso.infoApi.url);
	DrawSimpleTextField(jso, "accessKey", ref jso.infoApi.accessKey);

	GUILayout.Space(5);	
	GUILayout.Box(GUIContent.none, HrStyle.EditorLine, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
	GUILayout.Space(5);	
	
	DrawSimpleLabelField("API List");
	GUILayout.Space(3);	

	for(idx = 0; idx < nr; ++idx){
	    var api = jso.apiList[idx];
	    DrawSimpleTextField(jso, "label",     ref api.label);
	    DrawSimpleTextField(jso, "key",       ref api.key);
	    DrawSimpleTextField(jso, "url",       ref api.url);
	    DrawSimpleTextField(jso, "accessKey", ref api.accessKey);
	    EditorGUILayout.BeginHorizontal();	    
	    if(GUILayout.Button("Duplicate", EditorStyles.miniButton)){
		var clone = new ApiGatewayJSO.ApiGatewayInfo(){
		    label     = api.label + "(clone)",
		    key       = api.key,
		    url       = api.url,
		    accessKey = api.accessKey
		};
		jso.apiList.Add(clone);
		EditorUtility.SetDirty(target);
	    }
	    if(GUILayout.Button("Remove", EditorStyles.miniButton)){ 
		jso.apiList.RemoveAt(idx); 
		--idx;
		--nr;
		EditorUtility.SetDirty(target);
	    }
	    EditorGUILayout.EndHorizontal();	    
	    GUILayout.Space(8);
	}

	GUILayout.Space(5);	
	GUILayout.Box(GUIContent.none, HrStyle.EditorLine, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
	GUILayout.Space(5);	
	
	if(GUILayout.Button("Add", EditorStyles.miniButton)){
	    var api = new ApiGatewayJSO.ApiGatewayInfo(){ label = "newApi", key = "", url = "", accessKey = ""};
	    jso.apiList.Add(api);
	    EditorUtility.SetDirty(target);
	}
	GUILayout.Space(8);	
	
    }
}
}    

/*
 * Local variables:
 * compile-command: "make -C../ deploy"
 * End:
 */
