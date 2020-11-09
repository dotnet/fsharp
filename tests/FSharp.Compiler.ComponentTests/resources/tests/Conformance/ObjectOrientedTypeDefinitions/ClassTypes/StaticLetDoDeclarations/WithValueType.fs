// Regression test for DevDiv:139182
// Title: [fsbugs] Value types don't work properly when used with "static let"
// Static let consuming - value type
[<Struct>]
type S = member x.M() = 1

let m (s:S) = s.M()
 
type T() =
  static let s = S()
  let s2 = S()
  static member Prop1 = s.M()                // ok
  static member Prop2 = m s                  // ok
  static member Prop3 = let s' = s in s'.M() // ok
  member x.Prop5 = s2.M()                    // ok

exit <| if T.Prop1 = 1 && T.Prop2 = 1 && T.Prop3 = 1 && (new T()).Prop5 = 1 then 0 else 1
