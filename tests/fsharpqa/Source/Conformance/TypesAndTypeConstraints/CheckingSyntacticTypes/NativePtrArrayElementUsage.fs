// #NativePtr #FSharpQA #Conformance #TypeConstraints  
// Dev11 bug 293120
open System.Runtime.InteropServices

module Array = 

   let inline pinObjUnscoped (obj: obj) =

         GCHandle.Alloc(obj,GCHandleType.Pinned) 

   let inline pinObj (obj: obj) f = 

      let gch = pinObjUnscoped obj 

      try f gch

      finally

              gch.Free()

   let array1 = [|1;2;3;4|]
   let foo = &&array1.[0]


   [<NoDynamicInvocation>]
   let inline pin (arr: 'T []) (f : nativeptr<'T> -> 'U) = 

          pinObj (box arr) (fun _ -> f (&&arr.[0]))
