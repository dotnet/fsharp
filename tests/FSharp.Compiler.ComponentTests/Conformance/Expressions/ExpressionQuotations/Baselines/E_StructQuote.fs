// #Conformance #Quotations #Regression 
// Regression for bug 6420
//<Expects status="error" id="FS1220" span="(11,38)">ReflectedDefinitionAttribute may not be applied to an instance member on a struct type, because the instance member takes an implicit 'this' byref parameter</Expects>
//<Expects status="error" id="FS1220" span="(12,38)">ReflectedDefinitionAttribute may not be applied to an instance member on a struct type, because the instance member takes an implicit 'this' byref parameter</Expects>
//<Expects status="error" id="FS1220" span="(13,38)">ReflectedDefinitionAttribute may not be applied to an instance member on a struct type, because the instance member takes an implicit 'this' byref parameter</Expects>
[<Struct>]
type MyStruct =
    val private _i : int
    val private _s : single
    [<ReflectedDefinition>] new (i, s) = {_i = i; _s = s}
    [<ReflectedDefinition>] member t.I = t._i
    [<ReflectedDefinition>] member t.S = t._s
    [<ReflectedDefinition>] member t.Sum () = single t._i + t._s
exit 1