Json <=> ScriptableObject <=> S3 Object 可能な ScriptableObject基本クラス + Editor拡張
========================================================================

![](https://user-images.githubusercontent.com/1039507/33512221-d94f55a2-d76e-11e7-8461-3a0762c9790e.png)

### How To Use

#### Step.1

依存先のPackageを先に入れてください。

- [EditorUtils.unitypackage](https://github.com/karinharp/EditorUtils/releases)

#### Step.2

JsonableScriptableObject を継承した ScriptableObject を適当に作って、Inspector表示させると、

/Assets/AmPlugins/Settins/JsonableScriptableObjectConfig.asset

が生成されるので、そこに、Import/Export先となるディレクトリを設定。<br />
S3を使う場合はS3の設定とAWS CLIの設定も合わせて。


#### Step.3

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
