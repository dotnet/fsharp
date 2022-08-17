﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace VisualFSharp.UnitTests.Editor

open System
open System.Threading

open NUnit.Framework

open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis
open FSharp.Compiler.CodeAnalysis
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.LanguageService
open UnitTests.TestLib.Utils
open UnitTests.TestLib.LanguageService

[<TestFixture>][<Category "Roslyn Services">]
type HelpContextServiceTests() =

    let filePath = "C:\\test.fs"
    let makeOptions args = 
        { 
            ProjectFileName = "C:\\test.fsproj"
            ProjectId = None
            SourceFiles =  [| filePath |]
            ReferencedProjects = [| |]
            OtherOptions = args
            IsIncompleteTypeCheckEnvironment = true
            UseScriptResolutionRules = false
            LoadTime = DateTime.MaxValue
            OriginalLoadReferences = []
            UnresolvedReferences = None
            Stamp = None
        }

    let getMarkers (source:string) = 
       let mutable cnt = 0
       [ for i in 0 .. (source.Length - 1) do
             if source.[i] = '$' then 
                 yield (i - cnt)
                 cnt <- cnt + 1
       ]

    let TestF1KeywordsWithOptions(expectedKeywords: string option list, lines : string list, opts : string[]) =
        let options = makeOptions opts

        let fileContentsWithMarkers = String.Join("\r\n", lines)
        let fileContents = fileContentsWithMarkers.Replace("$", "")
        let document, sourceText = RoslynTestHelpers.CreateSingleDocumentSolution(filePath, fileContents, options = options)

        let markers = getMarkers fileContentsWithMarkers
        let res =
            [ for marker in markers do
                let span = Microsoft.CodeAnalysis.Text.TextSpan(marker, 0)
                let textLine = sourceText.Lines.GetLineFromPosition(marker)
                let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
                let classifiedSpans = Tokenizer.getClassifiedSpans(documentId, sourceText, textLine.Span, Some "test.fs", [], CancellationToken.None)

                FSharpHelpContextService.GetHelpTerm(document, span, classifiedSpans) |> Async.RunSynchronously
            ]
        let equalLength = (expectedKeywords.Length = res.Length)
        Assert.True(equalLength)

        for (exp, res) in List.zip expectedKeywords res do
           Assert.AreEqual(exp, res)

    let TestF1Keywords(expectedKeywords, lines) =
        TestF1KeywordsWithOptions(expectedKeywords, lines, [| |])

    [<Test>]
    member _.``F1 help keyword NoKeyword.Negative`` () =
        let file =
            [   "let s  = \"System.Con$sole\""
                "let n = 999$99"
                "#if UNDEFINED"
                " let w = List.re$v []"
                "#endif"
            ]
        let keywords = [ None; None; None ] 
        TestF1Keywords(keywords, file)
    
    [<Test>]
    member _.``F1 help keyword Preprocessor`` () =
        let file =
            [   "#i$f foobaz"
                "#e$ndif"
            ]
        let keywords = [ Some "#if_FS"; Some "#endif_FS" ] 
        TestF1Keywords(keywords, file)

    [<Test>]
    member _.``F1 help keyword Regression.DotNetMethod.854364``()  =
        let file =
            [   "let i : int = 42"
                "i.ToStri$ng()"
                "i.ToStri$ng(\"format\")"
            ]
        let keywords = 
            [   Some "System.Int32.ToString"
                Some "System.Int32.ToString"
            ]
        TestF1Keywords(keywords, file)
        
    [<Test>]
    member _.``F1 help keyword Namespaces`` () =
        let file =
            [   "open Syst$em.N$et"
                "open System.I$O"
                "open Microsoft.FSharp.Core"
                ""    
                "System.Cons$ole.WriteLine()"
            ]     
        let keywords =
            [   Some "System"
                Some "System.Net"
                Some "System.IO"
                Some "System.Console"
            ]
        TestF1Keywords(keywords, file)

    [<Test>]
    member _.``F1 help keyword Namespaces.BeforeDot`` () =
        let file =
            [   "open System$.Net$"
                "open System$.IO"
                "open System$.Collections$.Generic$"
                "open Microsoft.FSharp.Core"
                ""    
                "System$.Console$.WriteLine()"
            ]     
        let keywords =
            [   Some "System"
                Some "System.Net"                
                Some "System"
                Some "System"
                Some "System.Collections"
                Some "System.Collections.Generic"
                Some "System"
                Some "System.Console"
            ]
        TestF1Keywords(keywords, file)

    [<Test>]
    member _.``F1 help keyword Namespaces.AfterDot`` () =
        let file =
            [   "open $System.$Net"
                "open $System.IO"
                "open $System.$Collections.$Generic"
                "open Microsoft.FSharp.Core"
                ""    
                "$System.$Console.$WriteLine()"
            ]     
        let keywords =
            [   Some "System"
                Some "System.Net"                
                Some "System"
                Some "System"
                Some "System.Collections"
                Some "System.Collections.Generic"
                Some "System"
                Some "System.Console"
                Some "System.Console.WriteLine"
            ]
        TestF1Keywords(keywords, file)
    
    [<Test>]
    member _.``F1 help keyword QuotedIdentifiers``() = 
        let file = 
            [
                "let `$`escaped func`` x y = x + y"
                "let ``escaped value`$` = 1"
                "let x = 1"
                "``escaped func`` x$ ``escaped value``"
                "``escaped func``$ x ``escaped value``"
                "``escaped func`` x $``escaped value``"
                "let ``z$`` = 1"
                "``$z`` |> printfn \"%d\""
            ]
        let keywords = 
            [
                Some "Test.escaped func"
                Some "Test.escaped value"
                Some "Test.x"
                Some "Test.escaped func"
                Some "Test.escaped value"
                Some "Test.z"
                Some "Test.z"
            ]
        TestF1Keywords(keywords, file)        

    [<Test>]
    member _.``F1 help keyword Attributes`` () = 
        let file = 
            [
                "open System.Runtime.InteropServices"
                "open System.Runtime.CompilerServices"
                "[<St$ruct>]"
                "type X = "
                "    [<Default$Value(false)>]"
                "    val mutable f : int"
                "    [<Me$thodImpl(1s)>]"
                "    member _.Run() = ()"
                "[<StructLayout(LayoutKind.Auto, S$ize=1)>]"
                "type Y = class end"
            ]
        let keywords = 
            [
                Some "Microsoft.FSharp.Core.StructAttribute.#ctor"
                Some "Microsoft.FSharp.Core.DefaultValueAttribute.#ctor"
                Some "System.Runtime.CompilerServices.MethodImplAttribute.#ctor"
                Some "System.Runtime.InteropServices.StructLayoutAttribute.Size"
            ]
        TestF1Keywords(keywords, file)


    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.StaticParameters")>]
    //This test case Verify that when F1 is Hit on TypeProvider namespaces it contain the right keyword 
    member _.``F1 help keyword TypeProvider.Namespaces`` () =
        let file =
            [   
                "open N$1"
            ]
        let keywords =
            [  
                Some "N1"
            ]
        TestF1KeywordsWithOptions(keywords, file, [| "-r:" + PathRelativeToTestAssembly(@"DummyProviderForLanguageServiceTesting.dll") |])

    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.StaticParameters")>]
    //This test case Verify that when F1 is Hit on TypeProvider Type it contain the right keyword 
    member _.``F1 help keyword TypeProvider.type`` () =
       
        let file =
            [   
                //Dummy Type Provider exposes a parametric type (N1.T) that takes 2 static params (string * int)
                """let foo = typeof<N1.$T< const "Hello World",2>>"""
            ]
        let keywords =
            [  
                Some "N1.T"
            ]
        TestF1KeywordsWithOptions(keywords, file, [| "-r:"+PathRelativeToTestAssembly(@"DummyProviderForLanguageServiceTesting.dll") |])

    [<Test>]
    member _.``F1 help keyword EndOfLine``() =
        let file =
            [   "open System.Net$"
                "open System.IO$"
            ]
        let keywords =
            [   Some "System.Net"
                Some "System.IO"
            ]
        TestF1Keywords(keywords, file)

    [<Test>]
    member _.``F1 help keyword EndOfLine2``() =
        let file =
            [   "module M"
                "open System.Net$"
                "open System.IO$"
            ]
        let keywords =
            [   Some "System.Net"
                Some "System.IO"
            ]
        TestF1Keywords(keywords, file)

     
    [<Test>]
    member _.``F1 help keyword Comments``() =
        let file =
            [   "($* co$mment *$)"
                "/$/ com$ment"
            ]
        let keywords =
            [ Some "comment_FS"; Some "comment_FS"; Some "comment_FS"; Some "comment_FS"; Some "comment_FS"; ]
        TestF1Keywords(keywords, file)
    
    [<Test>]
    member _.``F1 help keyword FSharpEntities`` () =
        let file =
            [   "let (KeyValu$e(k,v)) = null"
                "let w : int lis$t = []"
                "let b = w.IsEm$pty"
                "let m : map<int,int> = Map.empty"
                "m.A$dd(1,1)"
                "let z = Li$st.r$ev w"
                "let o = No$ne"
                "let o1 = So$me 1"
                "let c : System.IO.Str$eam = null"
                "c.Async$Read(10)"
                "let r = r$ef 0"
                "r.conten$ts"
            ]
        let keywords =
            [   Some "Microsoft.FSharp.Core.Operators.KeyValuePattern``2"
                Some "Microsoft.FSharp.Collections.FSharpList`1"
                Some "Microsoft.FSharp.Collections.FSharpList`1.IsEmpty"
                Some "Microsoft.FSharp.Collections.FSharpMap`2.Add"
                Some "Microsoft.FSharp.Collections.ListModule"
                Some "Microsoft.FSharp.Collections.ListModule.Reverse``1" // per F1 keyword spec - one tick for classes, two ticks for members 
                Some "Microsoft.FSharp.Core.FSharpOption`1.None"
                Some "Microsoft.FSharp.Core.FSharpOption`1.Some"
                Some "System.IO.Stream" 
                Some "Microsoft.FSharp.Control.CommonExtensions.AsyncReadBytes"
                Some "Microsoft.FSharp.Core.Operators.Ref``1"
                Some "Microsoft.FSharp.Core.FSharpRef`1.contents"
            ]
        TestF1Keywords(keywords, file)
        
    [<Test>]
    member _.``F1 help keyword Keywords`` () =
        let file =
            [   "l$et r = ref 0"
                "r :$= 1"
                "let mut$able foo = 1"
                "foo <$- 2"
                "let$ z = 1"
            ]
        let keywords =
            [   Some "let_FS"
                Some ":=_FS"
                Some "mutable_FS"
                Some "<-_FS"
                Some "let_FS"
            ]
        TestF1Keywords(keywords, file)

    [<Test>]
    member _.``F1 help keyword Regression.NewInstance.854367`` () =
        let file =
            [   "let q : System.Runtime.Remoting.TypeE$ntry = null" ]
        let keywords = 
            [   Some "System.Runtime.Remoting.TypeEntry" ]
        TestF1Keywords(keywords, file)

    [<Test>]
    member _.``F1 help keyword Regression.NewInstance.854367.2`` () =
        let file =
            [   "let q1 = new System.Runtime.Remoting.Type$Entry()" // this consutrctor exists but is not accessible (it is protected), but the help entry still goes to the type
            ]
        let keywords = 
            [   Some "System.Runtime.Remoting.TypeEntry" ]
        TestF1Keywords(keywords, file)


    [<Test>]
    member _.``F1 help keyword Classes.WebClient`` () =
        let file =
            [   "let w : System.Net.Web$Client = new System.Net.Web$Client()" ]
        let keywords = 
            [   Some "System.Net.WebClient"
                Some "System.Net.WebClient.#ctor"
             ]
        TestF1Keywords(keywords, file)


    [<Test>]
    member _.``F1 help keyword Classes.Object`` () =
        let file =
            [   "let w : System.Ob$ject = new System.Obj$ect()" ]
        let keywords = 
            [   Some "System.Object"
                Some "System.Object.#ctor"
             ]
        TestF1Keywords(keywords, file)


    [<Test>]
    member _.``F1 help keyword Classes.Generic`` () =
        let file =
            [   "let x : System.Collections.Generic.L$ist<int> = null" ]
        let keywords = 
            [   Some "System.Collections.Generic.List`1" ]
        TestF1Keywords(keywords, file)

    [<Test>]
    member _.``F1 help keyword Classes.Abbrev`` () =
        let file =
            [   "let z : Resi$zeArray<int> = null" ]
        let keywords = 
            [   Some "System.Collections.Generic.List`1" ]
        TestF1Keywords(keywords, file)

    [<Test>]
    member _.``F1 help keyword Members`` () =        
        let file =
            [ "open System.Linq"
              "open System"
              "let l = new ResizeArray<int>()"
              "let i = l.Cou$nt"
              "l.Ad$d(1)"
              "let m = new System.IO.MemoryStream()"
              "m.BeginRe$ad()"
              "l.Se$lect(fun i -> i + 1)"
              "let d = new System.DateTi$me()"
              "let s = String.Empty"
              "s.Equ$als(null)"
              "let i = 12"
              "i.ToStr$ing()"
            ]
        let keywords = 
            [ Some "System.Collections.Generic.List`1.Count"
              Some "System.Collections.Generic.List`1.Add"
              Some "System.IO.Stream.BeginRead"
              Some "System.Linq.Enumerable.Select``2" // per F1 keyword spec - one tick for classes, two ticks for members 
              Some "System.DateTime.#ctor"
              Some "System.String.Equals"
              Some "System.Int32.ToString"
            ]
        TestF1Keywords(keywords, file)

    [<Test>]
    member _.``F1 help keyword static abstract interface method`` () =
        let file =
            ["type IStaticProperty<'T when 'T :> IStaticProperty<'T>> ="
             "    static abstract StaticProperty: 'T"
             ""
             "let f_IWSAM_flex_StaticProperty(x: #IStaticProperty<'T>) ="
             "    'T.StaticProp$erty" ]
        let keywords = 
            [   Some "Test.IStaticProperty`1.StaticProperty" ]
        TestF1Keywords(keywords, file)

