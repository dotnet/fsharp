// #Regression #Conformance #Quotations 
// Regression of FSB 5027, ICE when putting ReflectedDefinition on an extension member

type System.String with
    member this.Foo = 1
    
    [<ReflectedDefinition>]
    member this.Bar = 2

// Previously we would crash during compilation, if we get this far great.

exit 0
