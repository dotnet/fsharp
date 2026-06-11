namespace FSharp.Compiler.Service.Tests.HotReload

open Xunit

open FSharp.Compiler.ClosureNameAllocator
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTreeDiff

/// Unit tests for the Phase C3 occurrence-keyed closure name allocator: matched
/// occurrences must reuse baseline closure class names verbatim, unmatched (new)
/// occurrences must get generation-suffixed fresh names, and removed occurrences
/// must leave their baseline names unused forever.
module ClosureNameAllocatorTests =

    let private intTy = RuntimeTypeIdentity.NamedType("System.Int32", [])
    let private stringTy = RuntimeTypeIdentity.NamedType("System.String", [])

    let private memberSymbol: SymbolId =
        { Path = [ "Test" ]
          LogicalName = "f"
          Stamp = 1L
          Kind = SymbolKind.Value
          MemberKind = None
          IsSynthesized = false
          CompiledName = Some "f"
          TotalArgCount = Some 1
          GenericArity = Some 0
          ParameterTypeIdentities = None
          ReturnTypeIdentity = None }

    /// Builds an occurrence whose structural identity is determined by its parent
    /// chain and captures (the alignment never keys on the occurrence's own ordinal,
    /// matching C1). ParentChain is nearest-enclosing-first, as extracted by C1.
    let private mkOccurrence ordinal parentChain (captures: (string * RuntimeTypeIdentity) list) bodyHash : LambdaOccurrence =
        { Id =
            { MemberSymbol = memberSymbol
              Ordinal = ordinal
              ParentChain = parentChain }
          CurriedArity = 1
          ParameterTypes = [ [ intTy ] ]
          Captures =
            captures
            |> List.map (fun (name, ty) -> { LogicalName = name; Type = ty })
          ReturnTypeIdentity = intTy
          BodyHash = bodyHash
          Range = Range.range0 }

    let private assignmentNames (allocation: MemberClosureNameAllocation) =
        allocation.Assignments |> List.map (fun (_, assignment) -> assignment)

    [<Fact>]
    let ``generation-suffixed name format is baseName at hotreload hash g underscore o`` () =
        Assert.Equal("f@hotreload#g2_o3", formatGenerationSuffixedClosureName "f" 2 3)
        Assert.Equal("makeAdder@hotreload#g10_o0", formatGenerationSuffixedClosureName "makeAdder" 10 0)

    [<Fact>]
    let ``occurrence ordinal chain is root-first ending with own ordinal`` () =
        // ParentChain is nearest-enclosing-first: an occurrence with ordinal 2 nested
        // inside occurrence 1, itself nested inside occurrence 0, carries [1; 0].
        let inner = mkOccurrence 2 [ 1; 0 ] [] 0
        Assert.Equal<int list>([ 0; 1; 2 ], occurrenceOrdinalChain inner)

    [<Fact>]
    let ``matched occurrences reuse baseline names verbatim for pure body edits`` () =
        let baseline =
            [ mkOccurrence 0 [] [ "x", intTy ] 100
              mkOccurrence 1 [] [ "y", intTy ] 200 ]

        let baselineNames = Map.ofList [ [ 0 ], "f@hotreload"; [ 1 ], "f@hotreload-1" ]

        // Same lambda set, both bodies edited.
        let fresh =
            [ mkOccurrence 0 [] [ "x", intTy ] 101
              mkOccurrence 1 [] [ "y", intTy ] 201 ]

        let allocation = allocateMemberClosureNames baseline baselineNames fresh "f" 1

        Assert.Equal<ClosureNameAssignment list>(
            [ ClosureNameAssignment.Reused "f@hotreload"
              ClosureNameAssignment.Reused "f@hotreload-1" ],
            assignmentNames allocation
        )

        // The chain-forward table is unchanged: same occurrences, same names.
        Assert.Equal<Map<int list, string>>(baselineNames, allocation.RefreshedNamesByOccurrenceChain)

    [<Fact>]
    let ``added occurrence gets generation-suffixed name while survivors keep theirs`` () =
        let baseline =
            [ mkOccurrence 0 [] [ "x", intTy ] 100
              mkOccurrence 1 [] [ "y", intTy ] 200 ]

        let baselineNames = Map.ofList [ [ 0 ], "f@hotreload"; [ 1 ], "f@hotreload-1" ]

        // A new lambda (capturing z) is inserted between the two surviving ones, so the
        // second survivor's ordinal shifts from 1 to 2 — the alignment must still match
        // it (occurrence identity is structural, not ordinal-positional).
        let fresh =
            [ mkOccurrence 0 [] [ "x", intTy ] 100
              mkOccurrence 1 [] [ "z", stringTy ] 999
              mkOccurrence 2 [] [ "y", intTy ] 200 ]

        let allocation = allocateMemberClosureNames baseline baselineNames fresh "f" 2

        Assert.Equal<ClosureNameAssignment list>(
            [ ClosureNameAssignment.Reused "f@hotreload"
              ClosureNameAssignment.Fresh "f@hotreload#g2_o1"
              ClosureNameAssignment.Reused "f@hotreload-1" ],
            assignmentNames allocation
        )

        // The refreshed table re-keys the shifted survivor under its NEW chain.
        Assert.Equal<Map<int list, string>>(
            Map.ofList
                [ [ 0 ], "f@hotreload"
                  [ 1 ], "f@hotreload#g2_o1"
                  [ 2 ], "f@hotreload-1" ],
            allocation.RefreshedNamesByOccurrenceChain
        )

    [<Fact>]
    let ``removed occurrence leaves its baseline name unused and never chained`` () =
        let baseline =
            [ mkOccurrence 0 [] [ "x", intTy ] 100
              mkOccurrence 1 [] [ "y", intTy ] 200 ]

        let baselineNames = Map.ofList [ [ 0 ], "f@hotreload"; [ 1 ], "f@hotreload-1" ]

        // The first lambda is deleted; the survivor shifts to ordinal 0.
        let fresh = [ mkOccurrence 0 [] [ "y", intTy ] 200 ]

        let allocation = allocateMemberClosureNames baseline baselineNames fresh "f" 2

        Assert.Equal<ClosureNameAssignment list>(
            [ ClosureNameAssignment.Reused "f@hotreload-1" ],
            assignmentNames allocation
        )

        // The removed occurrence's name must not survive into the chained table.
        let chainedNames =
            allocation.RefreshedNamesByOccurrenceChain |> Map.toList |> List.map snd

        Assert.Equal<Map<int list, string>>(
            Map.ofList [ [ 0 ], "f@hotreload-1" ],
            allocation.RefreshedNamesByOccurrenceChain
        )

        Assert.DoesNotContain("f@hotreload", chainedNames)

    [<Fact>]
    let ``nested occurrences are keyed by their full ordinal chain`` () =
        let baseline =
            [ mkOccurrence 0 [] [ "x", intTy ] 100
              mkOccurrence 1 [ 0 ] [ "y", intTy ] 200 ]

        let baselineNames = Map.ofList [ [ 0 ], "f@hotreload"; [ 0; 1 ], "f@hotreload-1" ]

        let fresh =
            [ mkOccurrence 0 [] [ "x", intTy ] 100
              mkOccurrence 1 [ 0 ] [ "y", intTy ] 201 ]

        let allocation = allocateMemberClosureNames baseline baselineNames fresh "f" 1

        Assert.Equal<ClosureNameAssignment list>(
            [ ClosureNameAssignment.Reused "f@hotreload"
              ClosureNameAssignment.Reused "f@hotreload-1" ],
            assignmentNames allocation
        )

    [<Fact>]
    let ``capture-incompatible matched occurrence allocates a fresh name`` () =
        // Same shape, but the captured value changed type: pass 2 pairs them and the
        // pair is capture-incompatible, so per Roslyn semantics the baseline closure is
        // not reusable — the allocator must synthesize a fresh identity.
        let baseline = [ mkOccurrence 0 [] [ "x", intTy ] 100 ]
        let baselineNames = Map.ofList [ [ 0 ], "f@hotreload" ]
        let fresh = [ mkOccurrence 0 [] [ "x", stringTy ] 100 ]

        let allocation = allocateMemberClosureNames baseline baselineNames fresh "f" 3

        Assert.Equal<ClosureNameAssignment list>(
            [ ClosureNameAssignment.Fresh "f@hotreload#g3_o0" ],
            assignmentNames allocation
        )

        Assert.DoesNotContain(
            "f@hotreload",
            allocation.RefreshedNamesByOccurrenceChain |> Map.toList |> List.map snd
        )

    [<Fact>]
    let ``matched occurrence without a baseline name allocates fresh (fail closed)`` () =
        // The occurrence aligns, but the previous generation recorded no name for it
        // (unmappable method, dropped chain entry). A name is never guessed.
        let baseline = [ mkOccurrence 0 [] [ "x", intTy ] 100 ]
        let fresh = [ mkOccurrence 0 [] [ "x", intTy ] 100 ]

        let allocation = allocateMemberClosureNames baseline Map.empty fresh "f" 2

        Assert.Equal<ClosureNameAssignment list>(
            [ ClosureNameAssignment.Fresh "f@hotreload#g2_o0" ],
            assignmentNames allocation
        )

    [<Fact>]
    let ``allocation is stable across three simulated generations`` () =
        // Generation 1 (baseline): two lambdas with baseline-replay names.
        let gen1Occurrences =
            [ mkOccurrence 0 [] [ "x", intTy ] 100
              mkOccurrence 1 [] [ "y", intTy ] 200 ]

        let gen1Names = Map.ofList [ [ 0 ], "f@hotreload"; [ 1 ], "f@hotreload-1" ]

        // Generation 2: a new lambda (capturing z) is appended.
        let gen2Occurrences =
            [ mkOccurrence 0 [] [ "x", intTy ] 100
              mkOccurrence 1 [] [ "y", intTy ] 200
              mkOccurrence 2 [] [ "z", stringTy ] 300 ]

        let gen2Allocation = allocateMemberClosureNames gen1Occurrences gen1Names gen2Occurrences "f" 2

        Assert.Equal<ClosureNameAssignment list>(
            [ ClosureNameAssignment.Reused "f@hotreload"
              ClosureNameAssignment.Reused "f@hotreload-1"
              ClosureNameAssignment.Fresh "f@hotreload#g2_o2" ],
            assignmentNames gen2Allocation
        )

        // Generation 3: pure body edits — every name, including the generation-2
        // fresh one, must be reused verbatim from the chained table.
        let gen3Occurrences =
            [ mkOccurrence 0 [] [ "x", intTy ] 101
              mkOccurrence 1 [] [ "y", intTy ] 201
              mkOccurrence 2 [] [ "z", stringTy ] 301 ]

        let gen3Allocation =
            allocateMemberClosureNames
                gen2Occurrences
                gen2Allocation.RefreshedNamesByOccurrenceChain
                gen3Occurrences
                "f"
                3

        Assert.Equal<ClosureNameAssignment list>(
            [ ClosureNameAssignment.Reused "f@hotreload"
              ClosureNameAssignment.Reused "f@hotreload-1"
              ClosureNameAssignment.Reused "f@hotreload#g2_o2" ],
            assignmentNames gen3Allocation
        )

        // Generation 4: another addition — survivors (baseline-named AND g2-named)
        // keep their names; only the new occurrence gets a g4-suffixed name.
        let gen4Occurrences =
            [ mkOccurrence 0 [] [ "x", intTy ] 101
              mkOccurrence 1 [] [ "w", stringTy ] 400
              mkOccurrence 2 [] [ "y", intTy ] 201
              mkOccurrence 3 [] [ "z", stringTy ] 301 ]

        let gen4Allocation =
            allocateMemberClosureNames
                gen3Occurrences
                gen3Allocation.RefreshedNamesByOccurrenceChain
                gen4Occurrences
                "f"
                4

        Assert.Equal<ClosureNameAssignment list>(
            [ ClosureNameAssignment.Reused "f@hotreload"
              ClosureNameAssignment.Fresh "f@hotreload#g4_o1"
              ClosureNameAssignment.Reused "f@hotreload-1"
              ClosureNameAssignment.Reused "f@hotreload#g2_o2" ],
            assignmentNames gen4Allocation
        )
