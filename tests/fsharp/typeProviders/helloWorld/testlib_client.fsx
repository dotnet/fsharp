#r "provider.dll"
#r "test_lib.dll"

// To compile this function we must reference test_lib.dll. In the test (see build.bat) we reference
// the _old_ test_lib.dll, which was compiled when referencing a type with one optional static parameter.
// The provider is then recompiled to change the type to have two optional static parameters
// (analogous to adding a static parameter to a type provider). This
// provider must function correctly in the sense that it is able to resolve 
// old metadata references that don't give a value to the new optional static parameter.
// This is managed by the TP implementation in the F# compiler which fills in the static parameter
// based on its optional value.
let functionThatUsesTypeInformationFromTestLibWhichInvolvesTypesWhichUndergoBinaryCompatibleChange () = 
    Test.Int32.testBinaryCompatFunction 

#if ADD_AN_OPTIONAL_STATIC_PARAMETER
// after we added an optional static parameter, sanity check we can use it
let c = FSharp.HelloWorld.HelloWorldTypeWithStaticOptionalInt32Parameter<Count=1,ExtraParameter=2>.NestedType.StaticProperty1
#endif

