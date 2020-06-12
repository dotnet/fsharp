namespace FSharp.Compiler.UnitTests
open FSharp.Reflection
open NUnit.Framework

[<TestFixture>]
module ILMemberAccessTests =
  open FSharp.Compiler.AbstractIL.IL 
  [<Test>]
  let ``ILMemberAccess exhaustively OK for comparison`` () =
    
    let allCases = 
      let allCasesInfos = FSharpType.GetUnionCases (typeof<ILMemberAccess>)
      allCasesInfos
      |> Array.map (fun caseInfo ->
        FSharpValue.MakeUnion(caseInfo, [||]) :?> ILMemberAccess
      )
      |> Set.ofArray

    let addItem, checkedCases =
      let mutable items = Set.empty
      (fun item -> items <- Set.add item items)
      , (fun () -> items)

    let expectedComparisons =
      let rec cummulativeSum n =
        match n with
        | 0 -> 0
        | 1 -> 1
        | _ -> n + cummulativeSum(n-1)
      cummulativeSum (allCases.Count - 1)

    let mutable comparisonsCount = 0
    let compareIsGreater a b =
      if a > b then
        addItem a
        addItem b
        comparisonsCount <- comparisonsCount + 1
      else
        failwithf "%A > %A didn't hold" a b

    compareIsGreater ILMemberAccess.Public ILMemberAccess.FamilyOrAssembly
    compareIsGreater ILMemberAccess.Public ILMemberAccess.Family
    compareIsGreater ILMemberAccess.Public ILMemberAccess.Assembly
    compareIsGreater ILMemberAccess.Public ILMemberAccess.FamilyAndAssembly
    compareIsGreater ILMemberAccess.Public ILMemberAccess.Private
    compareIsGreater ILMemberAccess.Public ILMemberAccess.CompilerControlled
    compareIsGreater ILMemberAccess.FamilyOrAssembly ILMemberAccess.Assembly
    compareIsGreater ILMemberAccess.FamilyOrAssembly ILMemberAccess.Family
    compareIsGreater ILMemberAccess.FamilyOrAssembly ILMemberAccess.FamilyAndAssembly
    compareIsGreater ILMemberAccess.FamilyOrAssembly ILMemberAccess.Private
    compareIsGreater ILMemberAccess.FamilyOrAssembly ILMemberAccess.CompilerControlled
    compareIsGreater ILMemberAccess.Assembly ILMemberAccess.Family
    compareIsGreater ILMemberAccess.Assembly ILMemberAccess.FamilyAndAssembly
    compareIsGreater ILMemberAccess.Assembly ILMemberAccess.Private
    compareIsGreater ILMemberAccess.Assembly ILMemberAccess.CompilerControlled
    compareIsGreater ILMemberAccess.Family ILMemberAccess.FamilyAndAssembly
    compareIsGreater ILMemberAccess.Family ILMemberAccess.Private
    compareIsGreater ILMemberAccess.Family ILMemberAccess.CompilerControlled
    compareIsGreater ILMemberAccess.FamilyAndAssembly ILMemberAccess.Private
    compareIsGreater ILMemberAccess.FamilyAndAssembly ILMemberAccess.CompilerControlled
    compareIsGreater ILMemberAccess.Private ILMemberAccess.CompilerControlled

    let checkedCases = checkedCases ()
    Assert.IsTrue((checkedCases = allCases), sprintf "all cases weren't checked: %A versus %A" checkedCases allCases)
    Assert.AreEqual(expectedComparisons, comparisonsCount)