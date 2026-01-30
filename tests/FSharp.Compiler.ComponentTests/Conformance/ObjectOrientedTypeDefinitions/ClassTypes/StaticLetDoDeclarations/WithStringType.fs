// Regression test for DevDiv:139182
// Title: [fsbugs] Value types don't work properly when used with "static let"
// Static let consuming - string type
type T''() =
  static let str = "Test"
  static member Prop4 = str.ToLower()        // ok

exit <| if T''.Prop4 = "test" then 0 else 1