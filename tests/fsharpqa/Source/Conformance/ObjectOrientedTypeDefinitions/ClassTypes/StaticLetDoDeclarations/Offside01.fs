// #Regression #Conformance #ObjectOrientedTypes #Classes #LetBindings 
// Offside rule for static let
// We should start counting from the 'static' not from 'let'
// Regression test for FSHARP1.0:2042
//<Expects status="success"></Expects>
#light
type T() =
  static let foo baz =
    1

exit 0
