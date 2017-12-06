using UnityEngine;
using UnityEditor;
using System;
using System.Diagnostics;

namespace am
{
public class S3Command
{
    [Serializable]
    public class CliSetting {
	public string awsCommandPath;
    }

    public static void Put(CliSetting awsConf, string src, string dst, Action fCompleteCallback){
	var exProcess = new Process();
	exProcess.StartInfo.FileName = awsConf.awsCommandPath;
	exProcess.StartInfo.Arguments = " s3 cp " + src + " " + dst;
	UnityEngine.Debug.Log(exProcess.StartInfo.Arguments);
	exProcess.EnableRaisingEvents = true;
	exProcess.StartInfo.CreateNoWindow = true;
	exProcess.StartInfo.RedirectStandardOutput = true;	    
	exProcess.StartInfo.UseShellExecute = false;
	exProcess.Exited += (object sender, System.EventArgs e) => {  
	    //UnityEngine.Debug.Log("ProcessComplete");
	    //UnityEngine.Debug.Log(exProcess.StandardOutput.ReadToEnd());
	    exProcess.Dispose();
	    exProcess = null;	    
	    if(fCompleteCallback != null){ fCompleteCallback(); }
	};
	exProcess.Start();
    }

}
}

/*
 * Local variables:
 * compile-command: "make -C../ deploy"
 * End:
 */
