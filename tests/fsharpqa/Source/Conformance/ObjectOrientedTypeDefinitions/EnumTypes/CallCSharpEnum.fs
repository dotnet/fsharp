// #Conformance #ObjectOrientedTypes #Enums #ReqNOMT 
#light
// Consume a C# Enum from F#
namespace NF
    module M = 
      let e = NC.SimpleEnum.A
      (int e + int NC.SimpleEnum.B + int NC.SimpleEnum.C) |> exit

