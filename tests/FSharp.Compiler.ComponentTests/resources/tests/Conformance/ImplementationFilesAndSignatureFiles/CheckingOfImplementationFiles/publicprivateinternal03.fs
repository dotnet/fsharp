// #Regression #Conformance #SignatureFiles 


// Verify error if the declared accessibility of the implementation differs with the FSI file
// { public, internal, private } x { public, internal, private }

// Marked public in FSI but marked internal in FS
//<Expects id="FS0034" status="error">Module 'PublicPrivateInternal' contains\smember FsiSaysArePublic.C : unit -> unit\sbut its signature specifies\s        member FsiSaysArePublic.C : unit -> unit\sThe accessibility specified in the signature is more than that specified in the implementation</Expects>

// Marked public in FSI but marked private in FS
//<Expects id="FS0034" status="error">Module 'PublicPrivateInternal' contains\smember FsiSaysArePublic.B : unit -> unit\sbut its signature specifies\s        member FsiSaysArePublic.B : unit -> unit\sThe accessibility specified in the signature is more than that specified in the implementation</Expects>

// Marked internal in FSI but marked private in FS 
//<Expects id="FS0034" status="error">Module 'PublicPrivateInternal' contains\smember FsiSaysAreInternal.C : unit -> unit\sbut its signature specifies\s      member FsiSaysAreInternal.C : unit -> unit\sThe accessibility specified in the signature is more than that specified in the implementation</Expects>

namespace PublicPrivateInternal

type FsiSaysArePublic() =
    member public   this.A () = ()
    member internal this.B () = ()
    member private  this.C () = ()

type FsiSaysAreInternal =
    member public   this.A () = ()
    member internal this.B () = ()
    member private  this.C () = ()


type FsiSaysArePrivate =
    member public   this.A () = ()
    member internal this.B () = ()
    member private  this.C () = ()
