// #NoMT #CodeGen #Interop 
#light

// Verify anonymous classes generated for object expressions
// are private.

let VerifyTypeIsPrivate (ty : System.Type) =
    if ty.IsVisible <> false || ty.IsPublic <> false then
        exit 1
    ()


let oe1 = 
    { 
        new System.Object() with
            override this.ToString() = "oe1"
    }
    
VerifyTypeIsPrivate (oe1.GetType())
    
let oe2 = 
    {
        new System.IDisposable with
            member this.Dispose() = ()
    }
    
VerifyTypeIsPrivate (oe2.GetType())

exit 0
