// #Regression #Misc 
// Code snippet reported to crash the compiler (FSHARP1.0:5087)
//<Expects span="(16,33)" status="error" id="FS0043">Method or object constructor 'op_Addition' not found</Expects>
module TestModule
type Vector2D = {mutable x:float;mutable  y:float} with
    member this.Length = sqrt (this.x*this.x + this.y*this.y)
    member this.Length2
        with get () =
            sqrt (this.x*this.x + this.y*this.y)
        and set len =
            let s = len / this.Length
            this.x <- s * this.x
            this.y <- s * this.y
    static member ( + ) a b =
        {x = a.x + b.x; y= a.y + b.y}
let vecLength = {x=3.; y = 4.}  + {x = 4. ; y = 7.}

