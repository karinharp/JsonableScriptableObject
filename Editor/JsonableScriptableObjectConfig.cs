using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace am
{
public class JsonableScriptableObjectConfig : ScriptableObject {

    [Serializable]
    public class PathInfo {
	public string label;
	public string workspacePath;
	public string s3Bucket;
	public string s3Folder;
    }

    public S3Command.CliSetting s3CliSetting;
    public List<PathInfo> pathInfoList;

    public string [] selectableList {
	get { return pathInfoList.Select(info => info.label).ToArray<string>(); }
    }

}
}

/*
 * Local variables:
 * compile-command: make -C../ deploy
 * End:
 */
