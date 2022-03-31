// #Regression #Conformance #TypeInference #Recursion 
// Regression test for FSharp1.0:5423
// Title: code sample stack overflow from fsc.exe and brings down VS2008 intelisense

//<Expects status="error" id="FS0001" span="(20,48-20,49)">This expression was expected to have type</Expects>
//<Expects status="error" id="FS0001" span="(21,27-21,28)">The type 'myint<'d>' is not compatible with the type 'SfsIntTerm<'a>'</Expects>
//<Expects status="error" id="FS0001" span="(21,29-21,30)">The type 'myint<'d>' is not compatible with the type 'SfsIntTerm<'b>'</Expects>
//<Expects status="error" id="FS0193" span="(21,27-21,28)">Type constraint mismatch</Expects>

type myint<'a> = Myint of 'a

// Crashes type checker.
type SfsIntTerm<'a> () =
    static member ( - ) (l: SfsIntTerm<'a>, r: SfsIntTerm<'a>) = SfsModel.Difference ()
    static member ( - ) (l: myint<'a>       , r: SfsIntTerm<'a>) = SfsModel.CreateIntConstant l  - r
    static member ( * ) (l: myint<_>        , r: SfsIntTerm<_> ) =
        let term = SfsModel.CreateIntConstant l
        SfsModel.Product (term, r) 
    static member ( * ) (l: SfsIntTerm<'c>, r: myint<'d>) =
        let term =  SfsModel.CreateIntConstant l 
        SfsModel.Product (r,r) 

and SfsModel () =
    static member CreateIntConstant (x: myint<'a>) = Unchecked.defaultof<SfsIntTerm<'a>>
    static member Difference () = Unchecked.defaultof<SfsIntTerm<'a>>
    static member Product (l: SfsIntTerm<'a>,r: SfsIntTerm<'b>) : SfsIntTerm<'a> = failwith ""
