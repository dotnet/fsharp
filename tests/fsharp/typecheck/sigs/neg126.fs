module Neg126

// Variation on test case mentioned in https://github.com/dotnet/fsharp/pull/6805#issuecomment-580368303
//
// Here we are attempting to select a witness based on input type.
//
// However, only one witness is present.
//
// Due to the problem described in https://github.com/dotnet/fsharp/pull/6805#issuecomment-580396911, 
// "generic inline code we apply weak resolution to constraints that could otherwise be generalised",
// this generates a warning because overload resolution is invoked and the input type of "foo" becomes "sbyte" 
//
// The inferred type should ideally be
//    foo: ^a -> ^b
// but is actually 
//    foo: sbyte -> byte
//
// That is, the code is not generic at all, because the F# compiler thinks that it commit to the one and only witness.
//
// For pre-FS0143 this test exists to pin down that we get a warning produced saying ^a has been instantiated to "sbyte"
// For post-FS0143 this test exists to check that the code now compiles
module Negative_SelectOverloadedWitnessBasedOnInputTypeOneWitness = 
    type witnesses = 
      static member inline foo_witness (x : sbyte) : byte = byte x

    // Note, this doesn't try to use the output to select
    let inline call_foo_witness< ^witnesses, ^input, ^output when (^witnesses or ^input) : (static member foo_witness : ^input -> ^output)> (x : ^input) =
      ((^witnesses or ^input) : (static member foo_witness : ^input -> ^output) x)

    let inline foo (num: ^a) = call_foo_witness<witnesses, _, _> num
    let v1 = foo 0y

