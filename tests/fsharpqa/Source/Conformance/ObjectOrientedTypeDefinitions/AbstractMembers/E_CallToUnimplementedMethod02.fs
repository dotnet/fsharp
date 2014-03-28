// #Regression #Conformance #ObjectOrientedTypes #MethodsAndProperties #MemberDefinitions 
// Regression test for FSHARP1.0:5815
// It's illegal to call unimplemented base methods
//<Expects status="error" span="(9,30-9,39)" id="FS1201">Cannot call an abstract base member: 'M'$</Expects>

type C() = 
    inherit B()
    override x.M(a:int) = base.M(a)
    override x.M(a:string) = base.M(a)

