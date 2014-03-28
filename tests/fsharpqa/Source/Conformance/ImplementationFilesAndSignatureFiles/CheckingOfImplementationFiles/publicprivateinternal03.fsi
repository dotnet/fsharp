// #Regression #Conformance #SignatureFiles 
#light

// Marked internal in FSI but marked private in FS 
//<Expects id="FS0034" status="error">Module 'PublicPrivateInternal' contains</Expects>





// Marked public in FSI but marked private in FS
//<Expects id="FS0034" status="error">Module 'PublicPrivateInternal' contains</Expects>





// Marked public in FSI but marked internal in FS
//<Expects id="FS0034" status="error">Module 'PublicPrivateInternal' contains</Expects>






// Verify error if the declared accessibility of the implementation differs with the FSI file
// { public, internal, private } x { public, internal, private }

namespace PublicPrivateInternal

[<Class>]
type FsiSaysArePublic =
    member public A : unit -> unit
    member public B : unit -> unit
    member public C : unit -> unit

[<Class>]
type FsiSaysAreInternal =
    member internal A : unit -> unit
    member internal B : unit -> unit
    member internal C : unit -> unit

[<Class>]
type FsiSaysArePrivate =
    member private A : unit -> unit
    member private B : unit -> unit
    member private C : unit -> unit
