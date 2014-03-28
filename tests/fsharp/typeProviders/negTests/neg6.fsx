#r "helloWorldProvider.dll"

open FSharp.HelloWorld
open System.Collections.Generic

// Should not be able to inherit from an erased provided type
type T() =
  inherit HelloWorldType()

type T2() =
  inherit HelloWorldSubType()

type T3() =
  inherit HelloWorldSubException()


