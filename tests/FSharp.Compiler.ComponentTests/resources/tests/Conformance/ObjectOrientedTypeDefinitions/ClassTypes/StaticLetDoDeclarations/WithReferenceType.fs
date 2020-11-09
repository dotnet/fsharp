// Regression test for DevDiv:139182
// Title: [fsbugs] Value types don't work properly when used with "static let"
// Static let consuming - ref type
[<Class>]
type C() = member x.N() = 1

let n (c:C) = c.N()
 
type T'() =
  static let c = C()
  let c2 = C()
  static member Prop1 = c.N()                // ok
  static member Prop2 = n c                  // ok
  static member Prop3 = let c' = c in c'.N() // ok
  member x.Prop5 = c2.N()                    // ok

exit <| if T'.Prop1 = 1 && T'.Prop2 = 1 && T'.Prop3 = 1 && (new T'()).Prop5 = 1 then 0 else 1
