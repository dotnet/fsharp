namespace FSharp.Test

open Xunit

[<CollectionDefinition(nameof DoNotRunInParallel, DisableParallelization = true)>]
type DoNotRunInParallel = class end