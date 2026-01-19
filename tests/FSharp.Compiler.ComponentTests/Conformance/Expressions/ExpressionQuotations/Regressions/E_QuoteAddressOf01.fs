// #Regression #Conformance #Quotations 
// Quotes do not allow addressof of a mutable
// <Expects id="FS0462" status="error" span="(7,16-7,17)">Quotations cannot contain this kind of type</Expects>
// <Expects id="FS0462" status="error" span="(16,17-16,18)">Quotations cannot contain this kind of type</Expects>

let x = <@ let mutable x = 1
           let y = &x
           y
        @>

[<Struct>]
type S(z : int) =
    [<DefaultValue>] val mutable x : int
      
let x2 = <@ let mutable x = S(0)
            let y = &x
            y @>
            
exit 0
