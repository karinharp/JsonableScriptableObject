using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace am
{

[CustomEditor(typeof(SampleJSO))]
public class SampleJSOInspector : JsonableScriptableObjectInspector
{
    protected override void ImportFromJson(){
	var jso  = target as SampleJSO;
	JsonUtility.FromJsonOverwrite(GetJsonFile(), jso);
    }
    protected override void ExportToJson(){
	var jso  = target as SampleJSO;
	PutJsonFile(JsonUtility.ToJson(jso, false));
    }

    // 独自にインスペクターを組むときは、こんな感じでOverrideする。
    /* 
    protected override void DrawInspector(){
	var jso = target as SampleJSO;
	DrawSimpleIntField(jso,  "id",   ref jso.objId);
	DrawSimpleTextField(jso, "name", ref jso.objName);
    }
    */    
}
}    
