Json <=> ScriptableObject 可能な ScriptableObject基本クラス + Editor拡張
========================================================================

![](https://user-images.githubusercontent.com/1039507/33340662-1238f028-d4c0-11e7-95ae-f080fb3e3455.png)

### How To Use

#### Step.0

依存先のPackageを先に入れてください。

- [EditorUtils.unitypackage](https://github.com/karinharp/EditorUtils/releases)

#### Step.1

JsonableScriptableObject を継承した ScriptableObject を適当に作って、Inspector表示させると、

/Assets/AmPlugins/Settins/JsonableScriptableObjectConfig.asset

が生成されるので、そこに、Import/Export先となるディレクトリを設定

#### Step.2

あとは、

- JsonableScriptableObject.cs
- Editor/JsonableScriptableObjectInspector.cs

を継承したものを書くだけ。

### Sample

#### JsonableScriptableObject.cs 継承例

```
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace am
{
    
[Serializable, CreateAssetMenu( fileName = "SampleJSO", menuName = "am/SampleJSO", order = 1500 )]
public class SampleJSO : JsonableScriptableObject
{

    public int    objId;
    public string objName;

}
}
```

#### Editor/JsonableScriptableObjectInspector.cs 継承例

```
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
```
