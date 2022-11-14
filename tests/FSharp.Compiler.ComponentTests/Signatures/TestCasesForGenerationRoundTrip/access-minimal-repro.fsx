module Core_access
    
[<Sealed>]
type MyClassPropertiyGetters =     
    member internal x.InstInternal = 12
    member private  x.InstPrivate  = 12
    member public   x.InstPublic   = 12
    member          x.InstDefault  = 12   
