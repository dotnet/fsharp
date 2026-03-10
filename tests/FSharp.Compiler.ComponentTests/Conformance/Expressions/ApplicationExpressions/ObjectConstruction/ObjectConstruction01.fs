// #Conformance #ApplicationExpressions #ObjectConstructors 
// Sanity check Object Construction via constructors
//<Expects status="success"></Expects>

type Foo =
    val m_value : int
    member this.Value = this.m_value 
    public new()    = { m_value = -1 }
    public new(x)   = { m_value = x }
    public new(x:string,y:string) = { m_value = -2 }

if (new Foo()).Value       <> -1 then exit 1
if (new Foo(5)).Value      <>  5 then exit 1
if (new Foo("", "")).Value <> -2 then exit 1

exit 0
