// #Regression #Conformance #TypeConstraints 
// Same as Regressions01, but we keep the incorrect 'base'
// FSB 1748, Internal Error: when calling a base member
// Regression test for internal compiler error when calling a class's base member.
//<Expects id="FS0564" span="(30,33-30,40)" status="error">'inherit' declarations cannot have 'as' bindings\. To access members of the base class when overriding a method, the syntax 'base\.SomeMember' may be used; 'base' is a keyword\. Remove this 'as' binding\.$</Expects>
//<Expects id="FS0564" span="(38,39-38,46)" status="error">'inherit' declarations cannot have 'as' bindings\. To access members of the base class when overriding a method, the syntax 'base\.SomeMember' may be used; 'base' is a keyword\. Remove this 'as' binding\.$</Expects>
//<Expects id="FS0564" span="(47,40-47,47)" status="error">'inherit' declarations cannot have 'as' bindings\. To access members of the base class when overriding a method, the syntax 'base\.SomeMember' may be used; 'base' is a keyword\. Remove this 'as' binding\.$</Expects>
//<Expects id="FS0564" span="(53,46-53,53)" status="error">'inherit' declarations cannot have 'as' bindings\. To access members of the base class when overriding a method, the syntax 'base\.SomeMember' may be used; 'base' is a keyword\. Remove this 'as' binding\.$</Expects>

type Exp<'c when 'c :> Exp<'c>> = abstract Print : unit -> unit

type PrintLit<'c when 'c :> Exp<'c>>(value) =
    member x.Value = value
    member x.BasePrint() = printf "out %d" x.Value
    interface Exp<'c> with
        member x.Print() = x.BasePrint()

type PrintAdd<'c when 'c :> Exp<'c>>(left:'c, right:'c) =
    member x.Left = left
    member x.Right = right
    member x.BasePrint() = x.Left.Print(); printf "+"; x.Right.Print()
    interface Exp<'c> with
        member x.Print() = x.BasePrint()

type EvalExp<'c when 'c :> EvalExp<'c>> =
    inherit Exp<'c>
    abstract Eval : unit -> int

type EvalLit<'c when 'c :> EvalExp<'c>>(value:int) =
    inherit PrintLit<'c>(value) as base
    member x.BaseEval() = x.Value
    interface EvalExp<'c> with
        //the base is not strictly necessary here, but used for clarity
        member x.Print() = base.BasePrint()
        member x.Eval() = x.BaseEval()

type EvalAdd<'c when 'c :> EvalExp<'c>>(left:'c, right:'c) =
    inherit PrintAdd<'c>(left, right) as base
    member x.BaseEval() = x.Left.Eval() + x.Right.Eval()
    interface EvalExp<'c> with
        member x.Print() = base.BasePrint()
        member x.Eval() = x.BaseEval()

type EvalExpFix = inherit EvalExp<EvalExpFix>

type EvalLitFix(value) =
    inherit EvalLit<EvalExpFix>(value) as base
    interface EvalExpFix with
        member x.Print() = base.BasePrint()
        member x.Eval() = base.BaseEval()

type EvalAddFix(left:EvalExpFix, right:EvalExpFix) =
    inherit EvalAdd<EvalExpFix>(left, right) as base
    interface EvalExpFix with
        member x.Print() = base.BasePrint()
        member x.Eval() = base.BaseEval()

let e1 = new EvalLitFix(2)
let e2 = new EvalLitFix(3)
let e3 = new EvalAddFix(e1, e2) :> EvalExpFix

let results = sprintf "%A %A %A" e1 e2 e3

exit 0
