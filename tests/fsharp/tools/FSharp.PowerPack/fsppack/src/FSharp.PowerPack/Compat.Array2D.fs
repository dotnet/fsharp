#nowarn "9"
#nowarn "51" // The address-of operator may result in non-verifiable code. Its use is restricted to passing byrefs to functions that require them
   
namespace Microsoft.FSharp.Compatibility

[<RequireQualifiedAccess>]
module Array2D = 
    
    open System.Runtime.InteropServices
    
    let inline pinObjUnscoped (obj: obj) =  
        GCHandle.Alloc(obj,GCHandleType.Pinned) 

    let inline pinObj (obj: obj) f = 
        let gch = pinObjUnscoped obj 
        try f gch
        finally
            gch.Free()

    [<NoDynamicInvocation>]
    let inline pin (arr: 'T [,]) (f : nativeptr<'T> -> 'U) = 
        pinObj (box arr) (fun _ -> f (&&arr.[0,0]))
    
    [<NoDynamicInvocation>]
    let inline pinUnscoped (arr: 'T [,]) : nativeptr<'T> * _ = 
        let gch = pinObjUnscoped (box arr) 
        &&arr.[0,0], gch

    [<NoDynamicInvocation>]
    let inline pin_unscoped arr = pinUnscoped arr
