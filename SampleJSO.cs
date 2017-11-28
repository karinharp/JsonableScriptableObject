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
