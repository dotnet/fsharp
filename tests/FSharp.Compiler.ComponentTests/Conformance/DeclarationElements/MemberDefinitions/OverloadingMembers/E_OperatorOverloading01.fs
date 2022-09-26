// #Regression #Conformance #DeclarationElements #MemberDefinitions #Overloading 
// Regression test for FSHARP1.0:4964
// Compiler should not ICE on inappropriate operator overloading
//<Expects status="error" span="(21,13-21,14)" id="FS0043">Method or object constructor 'op_Addition' not found</Expects>
type MyPoint =
    struct
        val mutable private m_X : float
        val mutable private m_Y : float
        
        new (x, y) = { m_X = x; m_Y = y }
        
        member this.X with get () = this.m_X and set x = this.m_X <- x
        member this.Y with get () = this.m_Y and set y = this.m_Y <- y
        member this.Len = sqrt ( this.X * this.X + this.Y * this.Y )
        
        // (* Correct *) static member (+) (p1 : MyPoint, p2 : MyPoint) = MyPoint(p1.X + p2.X, p1.Y + p2.Y)
        (* Incorrect *) static member (+) (p1 : MyPoint) (p2 : MyPoint) = MyPoint(p1.X + p2.X, p1.Y + p2.Y)
    end

let p1 = MyPoint(1.0, 1.0)
let p2 = p1 + p1        //  error on +
