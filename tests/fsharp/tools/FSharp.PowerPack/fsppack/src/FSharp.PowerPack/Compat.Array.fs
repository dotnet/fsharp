#nowarn "9"
#nowarn "51" // The address-of operator may result in non-verifiable code. Its use is restricted to passing byrefs to functions that require them
   
namespace Microsoft.FSharp.Compatibility

module Array = 

    open System.Runtime.InteropServices
    
    let invalidArg arg msg = raise (new System.ArgumentException((msg:string),(arg:string)))        
    
    let nonempty (arr: _[]) = (arr.Length <> 0)
    
    let inline pinObjUnscoped (obj: obj) =  GCHandle.Alloc(obj,GCHandleType.Pinned) 

    let inline pinObj (obj: obj) f = 
        let gch = pinObjUnscoped obj 
        try f gch
        finally
            gch.Free()
    
    [<NoDynamicInvocation>]
    let inline pin (arr: 'T []) (f : nativeptr<'T> -> 'U) = 
        pinObj (box arr) (fun _ -> f (&&arr.[0]))
    
    [<NoDynamicInvocation>]
    let inline pinUnscoped (arr: 'T []) : nativeptr<'T> * _ = 
        let gch = pinObjUnscoped (box arr) 
        &&arr.[0], gch

    [<NoDynamicInvocation>]
    let inline pin_unscoped arr= pinUnscoped arr 
      
    let inline contains x (arr: 'T []) =
        let mutable found = false
        let mutable i = 0
        let eq = LanguagePrimitives.FastGenericEqualityComparer

        while not found && i < arr.Length do
            if eq.Equals(x,arr.[i]) then
                found <- true
            else
                i <- i + 1
        found

    let inline mem x arr = contains x arr
        
    let scanSubRight f (arr : _[]) start fin initState = 
        let mutable state = initState 
        let res = Array.create (2+fin-start) initState 
        for i = fin downto start do
          state <- f arr.[i] state;
          res.[i - start] <- state
        done;
        res

    let scanSubLeft f  initState (arr : _[]) start fin = 
        let mutable state = initState 
        let res = Array.create (2+fin-start) initState 
        for i = start to fin do
          state <- f state arr.[i];
          res.[i - start+1] <- state
        done;
        res

    let scanReduce f (arr : _[]) = 
        let arrn = arr.Length
        if arrn = 0 then invalidArg "arr" "the input array is empty"
        else scanSubLeft f arr.[0] arr 1 (arrn - 1)

    let scanReduceBack f (arr : _[])  = 
        let arrn = arr.Length
        if arrn = 0 then invalidArg "arr" "the input array is empty"
        else scanSubRight f arr 0 (arrn - 2) arr.[arrn - 1]

    let createJaggedMatrix (n:int) (m:int) (x:'T) = 
        let arr = (Array.zeroCreate n : 'T [][]) 
        for i = 0 to n - 1 do 
            let inner = (Array.zeroCreate m : 'T []) 
            for j = 0 to m - 1 do 
                inner.[j] <- x
            arr.[i] <- inner
        arr

    let create_matrix n m x = createJaggedMatrix n m x

    let isEmpty array = Array.isEmpty array

    let zero_create n = Array.zeroCreate n 

    let fold_left f z array = Array.fold f z array

    let fold_right f array z = Array.foldBack f array z

    let for_all f array = Array.forall f array


    let split array = Array.unzip array

    let combine array1 array2 = Array.zip array1 array2

    let make (n:int) (x:'T) = Array.create n x

    let to_list array = Array.toList array

    let of_list list  = Array.ofList list

