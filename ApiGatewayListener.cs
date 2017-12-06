using UnityEngine;

namespace am
{

public class ApiGatewayListener: MonoBehaviour
{

    public virtual void CompleteHandler(){}

    public virtual void ErrorHandler(int status){}

    public virtual void ProgressHandler(float progress){}
    
}
}

/*
 * Local variables:
 * compile-command: "make -C./ deploy"
 * End:
 */
