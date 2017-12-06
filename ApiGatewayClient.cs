using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace am
{

public class ApiGatewayClient : MonoBehaviour
{
    [Serializable]
    public class ApiGatewayInfo {
	public string label;
	public string key;
	public string url;
	public string accessKey;	
    };

    [Serializable]
    public class CompleteEvent : UnityEvent {}

    [Serializable]
    public class ErrorEvent : UnityEvent<int> {}

    [Serializable]
    public class ProgressEvent : UnityEvent<float> {}
    
    [SerializeField]
    ApiGatewayInfo m_apiGatewayInfo;

    [SerializeField]
    ApiGatewayListener m_listener;
    public ApiGatewayListener listener { get; set; }
    
    public string label
    {
	get { return m_apiGatewayInfo.label; }
	set { m_apiGatewayInfo.label = value; }
    }
    public string key
    {
	get { return m_apiGatewayInfo.key; }
	set { m_apiGatewayInfo.key = value; }
    }
    public string url
    {
	get { return m_apiGatewayInfo.url; }
	set { m_apiGatewayInfo.url = value; }
    }
    public string accessKey
    {
	get { return m_apiGatewayInfo.accessKey; }
	set { m_apiGatewayInfo.accessKey = value; }
    }

    [SerializeField]
    int m_apiGatewayClientBufSize = 1024*32; // 32 KB
    public int apiGatewayClientBufSize { set { m_apiGatewayClientBufSize = value; }}

    [SerializeField]
    int m_apiGatewayClientTimeout = 60; // 60 sec
    public int apiGatewayClientTimeout { set { m_apiGatewayClientTimeout = value; }}

    [SerializeField,HideInInspector]
    CompleteEvent m_fCompleteDispatcher;

    [SerializeField,HideInInspector]
    ErrorEvent m_fErrorDispatcher;

    [SerializeField,HideInInspector]
    ProgressEvent m_fProgressDispatcher;
    
    byte [] m_buf;

    /*
     * 簡易実装。再利用を考えてないので、GCがマッハでやばい。
     * そのうち真面目なByteArray実装に置き換える。
     */
    public class FileDownloadHandler : DownloadHandlerScript
    {
	ProgressEvent     m_fProgressDispatcher;
	List<byte>        m_byteArray;
	//int             m_offset = 0;
	//int             m_length = 0;
	    
	public string downloadData { get { return Encoding.UTF8.GetString(m_byteArray.ToArray()); }}

	public FileDownloadHandler(byte[] buffer, ProgressEvent fProgressDispatcher) : base(buffer){
	    m_byteArray         = new List<byte>();
	    m_fProgressDispatcher = fProgressDispatcher;
	}
	
	protected override bool ReceiveData(byte[] data, int dataLength){
	    //Debug.Log("ReceiveData : " + dataLength + " bytes");
	    //m_offset += dataLength;
	    m_byteArray.AddRange(data.Take(dataLength).ToArray());
	    if(m_fProgressDispatcher != null){ m_fProgressDispatcher.Invoke(GetProgress()); }
	    return true;
	}

	/*
	protected override float GetProgress(){
	    if(m_length == 0){ return 0.0f; }
	    return (float)m_offset / m_length;
	}
	protected override void CompleteContent(){
	    Debug.Log("Download Complete.");
	    Debug.Log(downloadData);
	}
	protected override void ReceiveContentLength(int contentLength){
	    //Debug.Log("ReceiveContentLength : " + contentLength + " bytes");
	    m_length = contentLength;
	}	
	*/	
    }
    
    void Awake() {
	m_buf = new byte[m_apiGatewayClientBufSize];
	if(m_listener != null){
	    m_fCompleteDispatcher.AddListener(m_listener.CompleteHandler);
	    m_fErrorDispatcher.AddListener(m_listener.ErrorHandler);
	    m_fProgressDispatcher.AddListener(m_listener.ProgressHandler);
	}
    }

    public void Get(){ StartCoroutine(SendGetRequest()); }
    public void Post(string queryJson){ StartCoroutine(SendPostRequest(queryJson)); }

    IEnumerator SendGetRequest()
    {
	var request         = new UnityWebRequest(m_apiGatewayInfo.url, UnityWebRequest.kHttpVerbGET);
	var downloadHandler = new FileDownloadHandler(m_buf, m_fProgressDispatcher);

	request.timeout         = m_apiGatewayClientTimeout;
	request.downloadHandler = downloadHandler;
	request.SetRequestHeader("x-api-key", m_apiGatewayInfo.accessKey);
	
	yield return request.SendWebRequest();

	if(request.isNetworkError){
	    Debug.Log("Network Error: " + request.error);
	    m_fErrorDispatcher.Invoke(0); 
	}
	else if(request.isHttpError){
	    Debug.Log("Http Error: " + request.responseCode);
	    m_fErrorDispatcher.Invoke((int)request.responseCode); 
	}
	else {
	    Debug.Log(downloadHandler.downloadData);
	    m_fCompleteDispatcher.Invoke(); 
	}
    }
    
    IEnumerator SendPostRequest(string queryJson)
    {
	var request         = new UnityWebRequest(m_apiGatewayInfo.url, UnityWebRequest.kHttpVerbPOST);
	var downloadHandler = new FileDownloadHandler(m_buf, m_fProgressDispatcher);
	var uploadHandler   = new UploadHandlerRaw(Encoding.UTF8.GetBytes(queryJson));
	
	request.uploadHandler   = uploadHandler;
	request.downloadHandler = downloadHandler;
	request.SetRequestHeader("x-api-key",    m_apiGatewayInfo.accessKey);
	request.SetRequestHeader("Content-Type", "application/json");

	yield return request.SendWebRequest();

	if(request.isNetworkError){
	    Debug.Log("Network Error: " + request.error);
	    m_fErrorDispatcher.Invoke(0); 	    
	}
	else if(request.isHttpError){
	    Debug.Log("Http Error: " + request.responseCode);
	    m_fErrorDispatcher.Invoke((int)request.responseCode); 
	}
	else {
	    Debug.Log(downloadHandler.downloadData);
	    m_fCompleteDispatcher.Invoke(); 
	}	
    }
    
}
}

/*
 * Local variables:
 * compile-command: "make -C./ deploy"
 * End:
 */
