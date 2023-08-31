namespace FSharp.Compiler.ComponentTests.Checking

open System
open Xunit
open FSharp.Compiler.PostTypeCheckSemanticChecks.Limit

module PostInferenceChecksTests =

    [<Literal>]
    let internal NUM_TESTS = 10_000

    let genNat (rng : Random) : int =
        let i = rng.Next ()
        if i = Int32.MinValue then
            0
        else
            abs i

    let internal possibleLimitFlags =
        Enum.GetValues typeof<LimitFlags>
        |> Seq.cast<LimitFlags>
        |> Array.ofSeq

    let internal genLimitFlags (rng : Random) : LimitFlags =
        Seq.init (genNat rng % 10 + 1) (fun _ -> possibleLimitFlags[genNat rng % possibleLimitFlags.Length])
        |> Seq.reduce (|||)

    let internal generateLimit (rng : Random) : Limit =
        { scope = genNat rng
          flags = genLimitFlags rng }

    let createSeed () =
        DateTime.UtcNow.Ticks % int64 Int32.MaxValue
        |> int

    [<Fact>]
    let ``NoLimit is almost a zero for CombineTwoLimits`` () =
        let seed = createSeed ()
        let rng = Random seed

        for count in 0..NUM_TESTS - 1 do
            try
                let original = generateLimit rng
                let after = CombineTwoLimits original NoLimit
                Assert.StrictEqual (after.flags, original.flags)
                if original.scope <> after.scope then
                    // See docs on the `scope` field for the conditions under which
                    // these special values are used.
                    Assert.InRange (after.scope, 0, 1)
            with
            | exc ->
                let exc = AggregateException ($"Failed with seed %i{seed}, count %i{count}", exc)
                raise exc

    [<Fact>]
    let ``CombineTwoLimits is commutative`` () =
        let seed = createSeed ()
        let rng = Random seed

        for count in 0..NUM_TESTS - 1 do
            try
                let x = generateLimit rng
                let y = generateLimit rng
                Assert.StrictEqual (CombineTwoLimits x y, CombineTwoLimits y x)
            with
            | exc ->
                let exc = AggregateException ($"Failed with seed %i{seed}, count %i{count}", exc)
                raise exc

    [<Fact>]
    let ``CombineTwoLimits is associative`` () =
        let seed = createSeed ()
        let rng = Random seed

        for count in 0..NUM_TESTS - 1 do
            try
                let x = generateLimit rng
                let y = generateLimit rng
                let z = generateLimit rng

                let lhs = CombineTwoLimits (CombineTwoLimits x y) z
                let rhs = CombineTwoLimits x (CombineTwoLimits y z)
                Assert.StrictEqual (lhs, rhs)
            with
            | exc ->
                let exc = AggregateException ($"Failed with seed %i{seed}, %i{count}", exc)
                raise exc
