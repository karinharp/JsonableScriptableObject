using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace am
{

[Serializable, CreateAssetMenu( fileName = "ApiGatewayJSO", menuName = "am/ApiGatewayInfo", order = 1500 )]
public class ApiGatewayJSO : JsonableScriptableObject
{
    
    [Serializable]
    public class ApiGatewayInfo {
	public string label;
	public string key;
	public string url;
	public string accessKey;	
    };

    // apiListを定義して吐き出したJsonをRuntimeで受け取れるEndpointを設定する
    // Runtimeでは、初期化時にそれを取得して、ランタイム用にインスタンス化したこのクラスにImportする。
    [SerializeField]
    public ApiGatewayInfo infoApi; 
    
    [SerializeField]
    public List<ApiGatewayInfo> apiList;

    public ApiGatewayInfo this[string key]{
	get { return apiList.FirstOrDefault(api => api.key == key); }
    }

    // テスト用
    public void Dump(){
	foreach(var api in apiList){
	    Debug.Log(api.label + ":" + api.key + ":" + api.url + ":" + api.accessKey);
	}
    }
    
}
}

/*
 * Local variables:
 * compile-command: "make -C./ deploy"
 * End:
 */
