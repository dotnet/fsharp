// #Regression #Diagnostics 
// Regression test for FSHARP1.0:3229
//<Expects id="FS1125" span="(7,18-7,21)" status="warning">The instantiation of the generic type 'T' is missing and can't be inferred from the arguments or return type of this member\. Consider providing a type instantiation when accessing this type, e\.g\. 'T<_>'</Expects>

type T<'a>() = 
    static member P = sizeof<'a>
    member x.M = T.P              // warning: should have used T<'a>.P or T<_>.P, most likely!
                                  // Instead, the compiler thinks we meant T<obj>.P

let v = (new T<int64>()).M

if v <> sizeof<obj> then 
    exit 1

exit 0

