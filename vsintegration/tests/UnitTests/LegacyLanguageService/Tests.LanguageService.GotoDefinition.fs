// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests.LanguageService.GotoDefinition

open System
open System.IO
open Xunit
open Salsa.Salsa
open Salsa.VsOpsUtils
open UnitTests.TestLib.Salsa
open UnitTests.TestLib.Utils
open System.Collections.Generic
open System.Text.RegularExpressions
open UnitTests.TestLib.LanguageService
open UnitTests.TestLib.ProjectSystem
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

type UsingMSBuild()  = 
    inherit LanguageServiceBaseTests()

    //GoToDefinitionSuccess Helper Function
    member private this.VerifyGoToDefnSuccessAtStartOfMarker(fileContents : string, marker : string,  definitionCode : string,?addtlRefAssy : string list) =
        let (sln, proj, file) = this.CreateSingleFileProject(fileContents, ?references = addtlRefAssy)

        MoveCursorToStartOfMarker (file, marker)
        let identifier = (GetIdentifierAtCursor file).Value |> fst //use marker to get the identifier
        let result = GotoDefinitionAtCursor file
        CheckGotoDefnResult
            (GotoDefnSuccess identifier definitionCode) 
            file
            result 
    
    member private this.VerifyGotoDefnSuccessForNonIdentifierAtStartOfMarker(fileContents : string, marker: string, pos : int * int, ?extraRefs) =
        let (_, _, file) = this.CreateSingleFileProject(fileContents, ?references = extraRefs)
        MoveCursorToStartOfMarker (file, marker)
        let result = GotoDefinitionAtCursor file
        Assert.True(result.Success, "result.Success")
        let actualPos = (result.Span.iStartLine, result.Span.iStartIndex)
        let line = GetLineNumber file (result.Span.iStartLine + 1)
        printfn "Actual line:%s, actual pos:%A" line actualPos
        Assert.Equal(pos, actualPos)
                    
    //GoToDefinitionFail Helper Function
    member private this.VerifyGoToDefnFailAtStartOfMarker(fileContents : string,  marker :string,?addtlRefAssy : string list) =
        
        this.VerifyGoToDefnFailAtStartOfMarker(
            fileContents = fileContents,
            marker = marker,
            f = (fun (file,result) -> CheckGotoDefnResult GotoDefnFailure file result),
            ?addtlRefAssy = addtlRefAssy
            )


    //GoToDefinitionFail Helper Function
    member private this.VerifyGoToDefnFailAtStartOfMarker(fileContents : string,  marker :string, f : OpenFile * GotoDefnResult -> unit, ?addtlRefAssy : string list) =
        let (sln, proj, file) = this.CreateSingleFileProject(fileContents, ?references = addtlRefAssy)

        MoveCursorToStartOfMarker (file, marker)
        let result = GotoDefinitionAtCursor file
        f (file, result)


    //GoToDefinition verify no Error dialog
    //The verification result should be:
    //  Fail at automation lab
    //  Succeed on dev machine with enlistment installed.
    member private this.VerifyGoToDefnNoErrorDialogAtStartOfMarker(fileContents : string,  marker :string, definitionCode : string, ?addtlRefAssy : string list) =
        let (sln, proj, file) = this.CreateSingleFileProject(fileContents, ?references = addtlRefAssy)

        MoveCursorToStartOfMarker (file, marker)
        let identifier = (GetIdentifierAtCursor file).Value |> fst //use marker to get the identifier
        let result = GotoDefinitionAtCursor file
        if not result.Success then
            CheckGotoDefnResult
                GotoDefnFailure 
                file
                result
        else
            CheckGotoDefnResult
                (GotoDefnSuccess identifier definitionCode) 
                file 
                result 
    


    

           


    /// run a GotoDefinition test where the expected result is a file that we
    /// have an `OpenFile` handle for (this won't work, e.g., if this file is a
    /// generated .fsi that (potentially) doesn't yet exist)

    /// exp  = (<identifier where cursor ought to be>, <whole line where cursor ought to be>, <name of file that's expected>) option
    /// file = <file to gotodefinition in>
    /// act  = <the result of the GotoDefinition call>
    member internal this.GotoDefinitionCheckResultAgainst (exp : (string * string * string) option)(file : OpenFile)(act : GotoDefnResult) : unit =
      match (exp, act.ToOption()) with
      | (Some (toFind, expLine, expFile), Some (span, actFile)) -> printfn "%s" "Result received, as expected; checking."
                                                                   Assert.Equal (expFile, actFile)
                                                                   printfn "%s" "Filename matches expected."
                                                                   MoveCursorTo(file, span.iStartLine + 1, span.iStartIndex + 1) // adjust & move to the identifier
                                                                   match GetIdentifierAtCursor file with // REVIEW: actually check that we're on the leftmost character of the identifier
                                                                   | None         -> Assert.Fail("No identifier at cursor!")
                                                                   | Some (id, _) -> Assert.Equal (toFind, id) // are we on the identifier we expect?
                                                                                     printfn "%s" "Identifier at cursor matches expected."
                                                                                     Assert.Equal (expLine.Trim (), (span.iStartLine |> (+) 1 |> GetLineNumber file).Trim ()) // ignore initial- / final-whitespace-introduced noise; adjust for difference in index numbers
                                                                                     printfn "%s" "Line at cursor matches expected."
      | (None,                            None)                 -> printfn "%s" "No result received, as expected." // sometimes we may expect GotoDefinition to fail, e.g., when the cursor isn't placed on a valid position (i.e., over an identifier, and, maybe, a constant if we decide to support that)
      | (Some _,                          None)                 -> Assert.Fail("No result received, but one was expected!") // distinguish this and the following case to give insight in case of failure
      | (None,                            Some _)               -> Assert.Fail("Result received, but none was expected!")

    /// this can be used when we don't have the expected file open; we still
    /// need its name

    /// exp = (<identifier where cursor ought to be>, <name of file that's expected>) option
    member internal this.GotoDefinitionCheckResultAgainstAnotherFile (proj : OpenProject)(exp : (string * string) option)(act : GotoDefnResult) : unit =
      match (exp, act.ToOption()) with
      | (Some (toFind, expFile), Some (span, actFile)) -> printfn "%s" "Result received, as expected; checking."
                                                          Assert.Equal (expFile, Path.GetFileName actFile)
                                                          printfn "%s" "Filename matches expected."
                                                          let file = OpenFile (proj, actFile)
                                                          let line = span.iStartLine |> ((+) 1) |> GetLineNumber file // need to adjust line number here
                                                          Assert.Equal (toFind, line.Substring (span.iStartIndex, toFind.Length))
                                                          printfn "%s" "Identifier at cursor matches expected."
      | (None,                   None)                 -> printfn "%s" "No result received, as expected." // sometimes we may expect GotoDefinition to fail, e.g., when the cursor isn't placed on a valid position (i.e., over an identifier, and, maybe, a constant if we decide to support that)
      | (Some _,                 None)                 -> Assert.Fail("No result received, but one was expected!") // distinguish this and the following case to give insight in case of failure
      | (None,                   Some _)               -> Assert.Fail("Result received, but none was expected!")

    /// exp = (<expected line>, <expected identifier>) option
    member this.GotoDefinitionTestWithSimpleFile (startLoc : string)(exp : (string * string) option) : unit =
        this.SolutionGotoDefinitionTestWithSimpleFile startLoc exp 





    member internal this.GotoDefinitionTestWithMarkup (lines : string list) =      
      let origins = Dictionary<string, int*int>()
      let targets = Dictionary<string, int*int>()
      let lines =
        [   let mutable lineNo = 0
            for l in lines do
                let builder = new System.Text.StringBuilder(l)
                let mutable cont = true
                while cont do
                    let s = builder.ToString()
                    let index = s.IndexOfAny([|'$';'#'|])
                    if index < 0 then
                        cont <- false
                    else
                        let c = s.[index]
                        let nextIndex = s.IndexOf(c, index+1) 
                        let marker = s.Substring(index+1, nextIndex - (index+1))
                        if c = '$' then 
                            origins.Add(marker, (lineNo+1,index+1)) // caret positions are 1-based, but...
                        else 
                            targets.Add(marker, (lineNo,index)) // ...spans are 0-based. Argh. Thank you, Salsa!
                        builder.Remove(index, nextIndex - index + 1) |> ignore
                yield builder.ToString()
                lineNo <- lineNo + 1
        ]

      let (_, _, file) = this.CreateSingleFileProject(lines)

      for KeyValue(marker,(line,col)) in origins do
          MoveCursorTo(file, line, col)
          let res = GotoDefinitionAtCursor file
          match res.ToOption() with
          |   None -> Assert.False(targets.ContainsKey(marker), sprintf "%s: definition not found " marker)
          |   Some (span,text) ->
                  match targets.TryGetValue(marker) with
                  |   false, _ ->  Assert.Fail(sprintf "%s: unexpected definition found" marker)
                  |   true, (line1, col1) -> 
                          Assert.True(span.iStartIndex = col1 && span.iStartLine = line1, 
                                sprintf "%s: wrong definition found expected %d %d but found %d %d %s" marker line1 col1 span.iStartLine span.iStartIndex text )

    
    /// exp = (<expected line>, <expected identifier>) option
    member internal this.SolutionGotoDefinitionTestWithSimpleFile (startLoc : string)(exp : (string * string) option) : unit =
      let lines = 
        [
          ""
          "let _ = 3"
          "let _ = \"hi\""
          "let _ = 2 + 3"
          "let _ = []"
          "let _ = ()"
          "let _ = null"
          "let _ ="
          "  let x = () (*loc-2*)"
          "  x (*loc-1*)"
          "let _ ="
          "  let x = () (*loc-5*)"
          "  let x = () (*loc-3*)"
          "  x (*loc-4*)"
          "let _ ="
          "  let x = () (*loc-7*)"
          "  let x ="
          "    x (*loc-6*)"
          "  ()"
          "let _ ="
          "  let     x = ()"
          "  let rec x = (*loc-9*)"
          "    fun y -> (*loc-10*)"
          "      x y (*loc-8*)"
          "  ()"
          "let _ ="
          "  let (+) x _ = x (*loc-12*)"
          "  2 + 3 (*loc-11*)"
          "type Zero = (*loc-13*)"
          "let foo (_ : Zero) : 'a = failwith \"hi\" (*loc-14*)"
          "type One = (*loc-16*)"
          "  One (*loc-15*)"
          "let f (x : One) = (*loc-17*)"
          "  One (*loc-18*)"
          "type Nat = (*loc-19*)"
          "  | Suc of Nat (*loc-20*)"
          "  | Zro (*loc-21*)"
          "let rec plus m n = (*loc-23*)"
          "  match m with (*loc-22*)"
          "  | Zro   -> (*loc-24*)"
          "      n"
          "  | Suc m -> (*loc-25*)"
          "      Suc (plus m n) (*loc-26*)"
          "type MyRec = (*loc-27*)"
          "  { myX : int (*loc-28*)"
          "    myY : int (*loc-29*)"
          "  }"
          "let rDefault ="
          "  { myX = 2 (*loc-30*)"
          "    myY = 3 (*loc-31*)"
          "  }"
          "let _ = { rDefault with myX = 7 } (*loc-32*)"
          "let _ ="
          "  let a = 2"
          "  let id (x : 'a) (*loc-33*)"
          "    : 'a = x (*loc-34*)"
          "  ()"
          "let _ ="
          "  let foo          = ()"
          "  let f (_ as foo) = (*loc-35*)"
          "    foo (*loc-36*)"
          "  ()"
          "let _ ="
          "  let foo          = ()"
          "  let f (x as foo) = foo"
          "  ()"
          "let _ ="
          "  fun x (*loc-37*)"
          "      x -> (*loc-38*)"
          "    x (*loc-39*)"
          "let _ ="
          "  let f = () (*loc-40*)"
          "  let f = (*loc-41*)"
          "    function f -> (*loc-42*)"
          "      f (*loc-43*)"
          "  ()"
          "let _ ="
          "  let f x ="
          "    match x with"
          "    | Suc x (*loc-44*)"
          "    | x (*loc-45*) -> "
          "        x"
          "  ()"
          "let _ ="
          "  let f x ="
          "    match x with"
          "    | Suc y & z -> (*loc-47*)"
          "        y (*loc-46*)"
          "  ()"
          "let _ ="
          "  let f xs ="
          "    match xs with"
          "    | x :: xs -> (*loc-49*)"
          "        x (*loc-48*)"
          "    | _       -> []"
          "  ()"
          "let _ ="
          "  let f x ="
          "    match x with"
          "    | (y : int, z) -> (*loc-51*)"
          "         y (*loc-50*)"
          "  ()"
          "let _ ="
          "  let f xs ="
          "    match xs with"
          "    | x :: xs (*loc-54*)"
          "      when xs <> [] -> (*loc-52*)"
          "        x :: xs (*loc-53*)"
          "  ()"
          "module Too = (*loc-55*)"
          "  let foo = 0 (*loc-56*)"
          "module Bar ="
          "  open Too (*loc-57*)"
          "let _ = Too.foo (*loc-58*)"
          "module Overlap ="
          "  type Parity = Even | Odd"
          "  let (|Even|Odd|) x = (*loc-59*)"
          "    if x % 0 = 0"
          "       then Even (*loc-60*)"
          "       else Odd"
          "  let foo (x : int) ="
          "    match x with"
          "    | Even -> 1 (*loc-61*)"
          "    | Odd  -> 0"
          "  let patval = (|Even|Odd|) (*loc-61b*)"
          "let _ ="
          "  op_Addition 2 2"
          "type Class () = (*loc-62*)"
          "  member c.Method () = () (*loc-63*)"
          "  static member Foo () = () (*loc-64*)"
          "let _ ="
          "  let c = Class () (*loc-65*)"
          "  c.Method () (*loc-66*)"
          "  Class.Foo () (*loc-67*)"
          "type Class' () ="
          "  member c.Method  () = c.Method () (*loc-68*)"
          "  member c.Method1 () = c.Method2 () (*loc-69*)"
          "  member c.Method2 () = c.Method1 () (*loc-70*)"
          "  member c.Method3  () ="
          "    let c = Class ()"
          "    c.Method () (*loc-71*)"
          "type Colors = Red   = 1"
          "            | White = 2"
          "            | Blue  = 3"
          "let _ = Colors.Red"
          "let _ ="
          "  let x = 2"
          "  \"x(*loc-72*)\""
          "let _ ="
          "  let x = 2"
          "  \"this is a string"
          "    x(*loc-73*)"
          "  \""
          "let _ ="
          "  let rec ``let`` = (*loc-74*)"
          "    function 0 -> 1"
          "           | n -> n * ``let`` (n - 1) (*loc-75*)"
          "let id77 = 0"  
          "type C ="
          "  val id77 (*loc-77*) : int"
        ]
      this.SolutionGotoDefinitionTestWithLines lines startLoc exp




    member internal this.SolutionGotoDefinitionTestWithLines lines (startLoc : string)(exp : (string * string) option) : unit =        
      // The test itself
      let (_, _, file) = this.CreateSingleFileProject(lines)
      let fnm = 
        GetNameOfOpenFile file
        |> Path.GetFileName
      MoveCursorToStartOfMarker (file, startLoc)
      let res = GotoDefinitionAtCursor file |> this.GotoDefinitionFixupFilename
      match exp with
      | None              -> this.GotoDefinitionCheckResultAgainst None                     file res
      | Some (endLoc, id) -> this.GotoDefinitionCheckResultAgainst (Some (id, endLoc, fnm)) file res

    /// exp = (<identifier where cursor ought to be>, <name of file that's expected>) option
    member this.GotoDefinitionTestWithLib (startLoc : string)(exp : (string * string) option) : unit =
      let lines = [ "let _ = List.map (*loc-1*)" ]
      let (_, proj, file) = this.CreateSingleFileProject(lines)

      MoveCursorToStartOfMarker (file, startLoc)
      let res = GotoDefinitionAtCursor file
      this.GotoDefinitionCheckResultAgainstAnotherFile proj exp res

    member this.GotoDefinitionFixupFilename (x : GotoDefnResult) : GotoDefnResult =
      if x.Success then 
        GotoDefnResult.MakeSuccess(Path.GetFileName x.Url, x.Span) 
      else 
        GotoDefnResult.MakeError(x.ErrorDescription)

    // the format of the comments for each test displays the desired behaviour,
    // where `$` and `#` indicate the initial and final cursor positions,
    // respectively; if the two are the same then the `#` is omitted

    // the `loc-<number>` comments in the source used for the tests are to
    // ensure that we've found the correct position (i.e., these must be unique
    // in any given test source file)




    /// let #x = () in $x
    [<Fact>]
    member public this.``GotoDefinition.Simple.Binding.TrivialLetRHS`` () =
      this.GotoDefinitionTestWithSimpleFile "x (*loc-1*)" (Some("let x = () (*loc-2*)", "x"))



























































    // ********** Tests of OO Stuff **********













    // ********** GetCompleteIdentifierIsland tests **********

    /// takes a string with a `$` representing the cursor position, gets the
    /// GotoDefinition identifier at that position and compares against expected
    /// result

    member this.GetCompleteIdTest tolerate (s : string)(exp : string option) : unit =
      let n = s.IndexOf '$'
      let s = s.Remove (n, 1)
      match (QuickParse.GetCompleteIdentifierIsland tolerate s n, exp) with
      | (Some (s1, _, _), Some s2) -> 
        printfn "%s" "Received result, as expected."
        Assert.Equal (s1, s2)
      | (None,         None)    -> 
        printfn "%s" "Received no result, as expected."
      | (Some _,       None)    -> 
        Assert.Fail("Received result, but none was expected!")
      | (None,         Some _)  -> 
        Assert.Fail("Expected result, but didn't receive one!")











        
       




// Context project system
type UsingProjectSystem() = 
    inherit UsingMSBuild(VsOpts = LanguageServiceExtension.ProjectSystemTestFlavour)
