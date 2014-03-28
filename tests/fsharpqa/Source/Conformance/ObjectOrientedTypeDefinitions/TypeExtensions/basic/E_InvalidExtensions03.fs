// #Regression #Conformance #ObjectOrientedTypes #TypeExtensions 
#light

//<Expects id="FS0925" status="error">Invalid type extension</Expects>

type System.String with
    member this.TwiceLength = this.Length * 2
    
type System.String =
    member this.TwiceLength = this.Length * 2
    
exit 1
