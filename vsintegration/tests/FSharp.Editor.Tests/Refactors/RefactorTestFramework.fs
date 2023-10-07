module FSharp.Editor.Tests.Refactors.RefactorTestFramework


open System
open System.Collections.Immutable
open System.Collections.Generic
open System.Text.RegularExpressions

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks

open FSharp.Compiler.Diagnostics
open FSharp.Editor.Tests.Helpers
open Microsoft.CodeAnalysis.CodeRefactorings
open Microsoft.CodeAnalysis.CodeActions
open System.Collections.Generic
open System.Threading
open Microsoft.CodeAnalysis.Tags
open System.Reflection
open Microsoft.FSharp.Reflection

type DynamicHelper =  
  static member MkMethod<'t,'u> (mi:MethodInfo) o : 't -> 'u=
    let typ = typeof<'t>
    fun t -> 
      let args = 
        if (typ = typeof<unit>) then [||]
        else
          if not (FSharpType.IsTuple typ) then [| box t |]
          else
            FSharpValue.GetTupleFields t
      mi.Invoke(o, args) :?> 'u

let (?) (o:'a) s : 'b =
  let ty = o.GetType()
  let field = ty.GetField(s, BindingFlags.Instance ||| BindingFlags.Public ||| BindingFlags.NonPublic)
  if field <> null then field.GetValue(o) :?> 'b
  else
    let prop = ty.GetProperty(s, BindingFlags.Instance ||| BindingFlags.Public ||| BindingFlags.NonPublic)
    if prop <> null then prop.GetValue(o, null) :?> 'b
    else
      let meth = ty.GetMethod(s, BindingFlags.Instance ||| BindingFlags.Public ||| BindingFlags.NonPublic)
      let d,r = FSharpType.GetFunctionElements(typeof<'b>)
      typeof<DynamicHelper>.GetMethod("MkMethod").MakeGenericMethod([|d;r|]).Invoke(null, [| box meth; box o |]) :?> 'b
type TestCodeFix = { Message: string; FixedCode: string }

type Mode =
    | Auto
    | WithOption of CustomProjectOption: string
    | WithSignature of FsiCode: string
    | Manual of Squiggly: string * Diagnostic: string
    | WithSettings of CodeFixesOptions

let inline toOption o =
    match o with
    | ValueSome v -> Some v
    | _ -> None

let mockAction =
    Action<CodeActions.CodeAction, ImmutableArray<Diagnostic>>(fun _ _ -> ())

let parseDiagnostic diagnostic =
    let regex = Regex "([A-Z]+)(\d+)"
    let matchGroups = regex.Match(diagnostic).Groups
    let prefix = matchGroups[1].Value
    let number = int matchGroups[2].Value
    number, prefix

let getDocument code mode =
    match mode with
    | Auto -> RoslynTestHelpers.GetFsDocument code
    | WithOption option -> RoslynTestHelpers.GetFsDocument(code, option)
    | WithSignature fsiCode -> RoslynTestHelpers.GetFsiAndFsDocuments fsiCode code |> Seq.last
    | Manual _ -> RoslynTestHelpers.GetFsDocument code
    | WithSettings settings -> RoslynTestHelpers.GetFsDocument(code, customEditorOptions = settings)

let getRelevantDiagnostics (document: Document) =
    cancellableTask {
        let! _, checkFileResults = document.GetFSharpParseAndCheckResultsAsync "test"

        return checkFileResults.Diagnostics
    }
    |> CancellableTask.startWithoutCancellation
    |> fun task -> task.Result



let tryRefactor (code: string) (cursorPosition) (ct) (refactorProvider: 'T when 'T :> CodeRefactoringProvider ) =
    cancellableTask {
        let refactoringActions= new List<CodeAction>()

        let mutable solution = RoslynTestHelpers.CreateSolution(code)

        let document = RoslynTestHelpers.GetSingleDocument solution

        use mutable workspace = solution.Workspace

        let context = CodeRefactoringContext(document, TextSpan(cursorPosition,1), (fun a -> refactoringActions.Add a),ct)

        do! refactorProvider.ComputeRefactoringsAsync context
        for action in refactoringActions do
            let! operations = action.GetOperationsAsync ct
            for operation in operations do
                let codeChangeOperation = operation :?> ApplyChangesOperation
                codeChangeOperation.Apply(workspace,ct)
                solution <- codeChangeOperation.ChangedSolution 
                ()


        let! changedText = solution.GetDocument(document.Id).GetTextAsync(ct)
        return changedText
    }
    |> CancellableTask.startWithoutCancellation
