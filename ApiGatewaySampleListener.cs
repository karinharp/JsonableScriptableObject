using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Events;
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace am
{

public class ApiGatewaySampleListener: ApiGatewayListener
{

    [SerializeField]
    Text m_status;
    
    public override void CompleteHandler(){
	m_status.text = "通信完了";
	m_status.text = "通信完了";
    }

    public override void ErrorHandler(int status){
	Debug.Log(status.ToString());
	if(status == 0){
	    m_status.text = "通信エラー\n通信環境の良いところで再度お試しください。";
	}
	else {
	    m_status.text = "通信エラー\nサーバとの通信に失敗しました。\nエラーコード : " + status.ToString();	    
	}
    }

    public override void ProgressHandler(float progress){
	var label = Math.Round((Decimal)progress * 100, 2, MidpointRounding.AwayFromZero);
	m_status.text = "通信中\n" + label.ToString() + " %";	
    }
    
}
}

/*
 * Local variables:
 * compile-command: "make -C./ deploy"
 * End:
 */
