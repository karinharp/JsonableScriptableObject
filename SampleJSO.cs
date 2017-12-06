using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace am
{
    
[Serializable, CreateAssetMenu( fileName = "SampleJSO", menuName = "am/SampleJSO", order = 1500 )]
public class SampleJSO : JsonableScriptableObject
{

    public int    castId;
    public string castName;

    [Range(0,200)]
    public int hp;
    [Range(0,300)]
    public float speed;
    [Range(1,100)]
    public int ssPower; 
    [Range(1,200)]
    public int dsPower; 

}
}

/*
 * Local variables:
 * compile-command: "make -C./ deploy"
 * End:
 */
