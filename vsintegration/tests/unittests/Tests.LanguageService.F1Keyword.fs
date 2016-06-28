// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Tests.LanguageService.F1Keyword

open System
open NUnit.Framework
open Salsa.Salsa
open Salsa.VsOpsUtils
open UnitTests.TestLib.Salsa
open UnitTests.TestLib.Utils
open UnitTests.TestLib.LanguageService
open UnitTests.TestLib.ProjectSystem

[<TestFixture>] 
type UsingMSBuild() =
    inherit LanguageServiceBaseTests()

    member private this.TestF1Keywords(expectedKeywords, testLines, ?addtlRefAssy : list<string>) =
        let lines, poslist = // poslist is in _cursor_ postions. Cursor postions are 1-based.
            let rec extractPos (col:int) row (s:string) poslist =
                    let next = s.IndexOf("$", col)
                    if next < 0 then
                        s, poslist
                    else                     
                        let s = s.Remove(next, 1)
                        extractPos next row s ((row,next+1)::poslist)
            let _, l, p =
                List.fold (fun (row,lines,poslist) s -> 
                                let (s', poslist') = extractPos 0 row s poslist
                                (row+1, (s'::lines),poslist')) (1,[],[]) testLines 
            List.rev l , List.rev p

        let refs = 
            let standard = ["mscorlib"; "System"; "System.Core"]
            match addtlRefAssy with
            | Some r -> standard @ r
            | _ -> standard

        let (_,_, file) = this.CreateSingleFileProject(lines, references = refs)
        Assert.IsTrue(List.length expectedKeywords = List.length poslist, sprintf "number of keywords (%d) does not match positions (%d)" (List.length expectedKeywords) (List.length poslist))
        List.iter2 
            (fun expectedKeyword (row,col) ->
                MoveCursorTo(file,row,col)
                let keyword = GetF1KeywordAtCursor file
                Assert.AreEqual(expectedKeyword, keyword)) expectedKeywords poslist
        ()
        
    [<Test>]
    member public this.``NoKeyword.Negative`` () =
        let file =
            [   "let s  = \"System.Con$sole\""
                "let n = 999$99"
                "#if UNDEFINED"
                " let w = List.re$v []"
                "#endif"
            ]
        let keywords = [ None; None; None ] 
        this.TestF1Keywords(keywords, file)
    
    [<Test>]
    member public this.``Preprocessor`` () =
        let file =
            [   "#i$f foobaz"
                "#e$ndif"
            ]
        let keywords = [ Some "#if_FS"; Some "#endif_FS" ] 
        this.TestF1Keywords(keywords, file)

    [<Test>]
    member public this.``Regression.DotNetMethod.854364``()  =
        let file =
            [   "let i : int = 42"
                "i.ToStri$ng()"
                "i.ToStri$ng(\"format\")"
            ]
        let keywords = 
            [   Some "System.Int32.ToString"
                Some "System.Int32.ToString"
            ]
        this.TestF1Keywords(keywords, file)
        
    [<Test>]
    member public this.``Namespaces`` () =
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
        this.TestF1Keywords(keywords, file)

    [<Test>]
    member public this.``Namespaces.BeforeDot`` () =
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
        this.TestF1Keywords(keywords, file)

    [<Test>]
    member public this.``Namespaces.AfterDot`` () =
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
        this.TestF1Keywords(keywords, file)
    
    [<Test>]
    member public this.``QuotedIdentifiers``() = 
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
                Some "File1.escaped func"
                Some "File1.escaped value"
                Some "File1.x"
                Some "File1.escaped func"
                Some "File1.escaped value"
                Some "File1.z"
                Some "File1.z"
            ]
        this.TestF1Keywords(keywords, file)        

    [<Test>]
    member public this.``Attributes`` () = 
        let file = 
            [
                "open System.Runtime.InteropServices"
                "open System.Runtime.CompilerServices"
                "[<St$ruct>]"
                "type X = "
                "    [<Default$Value(false)>]"
                "    val mutable f : int"
                "    [<Me$thodImpl(1s)>]"
                "    member this.Run() = ()"
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
        this.TestF1Keywords(keywords, file)


    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.StaticParameters")>]
    //This test case Verify that when F1 is Hit on TypeProvider namespaces it contain the right keyword 
    member public this.``TypeProvider.Namespaces`` () =
        let file =
            [   
                "open N$1"
            ]
        let keywords =
            [  
                Some "N1"
            ]
        this.TestF1Keywords(keywords, file, 
            addtlRefAssy = [PathRelativeToTestAssembly(@"UnitTestsResources\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])

    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.StaticParameters")>]
    //This test case Verify that when F1 is Hit on TypeProvider Type it contain the right keyword 
    member public this.``TypeProvider.type`` () =
       
        let file =
            [   
                //Dummy Type Provider exposes a parametric type (N1.T) that takes 2 static params (string * int)
                """let foo = typeof<N1.$T< const "Hello World",2>>"""
            ]
        let keywords =
            [  
                Some "N1.T"
            ]
        this.TestF1Keywords(keywords, file, 
            addtlRefAssy = [PathRelativeToTestAssembly(@"UnitTestsResources\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])


    [<Test>]
    member public this.``EndOfLine``() =
        let file =
            [   "open System.Net$"
                "open System.IO$"
            ]
        let keywords =
            [   Some "System.Net"
                Some "System.IO"
            ]
        this.TestF1Keywords(keywords, file)

    [<Test>]
    member public this.``EndOfLine2``() =
        let file =
            [   "module M"
                "open System.Net$"
                "open System.IO$"
            ]
        let keywords =
            [   Some "System.Net"
                Some "System.IO"
            ]
        this.TestF1Keywords(keywords, file)

     
    [<Test>]
    member public this.``Comments``() =
        let file =
            [   "($* co$mment *$)"
                "/$/ com$ment"
            ]
        let keywords =
            [ Some "comment_FS"; Some "comment_FS"; Some "comment_FS"; Some "comment_FS"; Some "comment_FS"; ]
        this.TestF1Keywords(keywords, file)
    
    [<Test>]
    member public this.``FSharpEntities`` () =
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
        this.TestF1Keywords(keywords, file)
        
    [<Test>]
    member public this.``Keywords`` () =
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
        this.TestF1Keywords(keywords, file)

    [<Test>]
    member public this.``Regression.NewInstance.854367`` () =
        let file =
            [   "let q1 = new System.Runtime.Remoting.Type$Entry()"
            ]
        let keywords = 
            [   Some "System.Runtime.Remoting.TypeEntry"
            ]
        this.TestF1Keywords(keywords, file)


    [<Test>]
    member public this.``Classes`` () =
        let file =
            [   "let q : System.Runtime.Remoting.TypeE$ntry = null"
                "let q1 = new System.Runtime.Remoting.Type$Entry()"
                "let w : System.Net.Web$Client = new System.Net.Web$Client()"
                "let x : System.Collections.Generic.L$ist<int> = null"
                "let z : Resi$zeArray<int> = null" 
             ]
        let keywords = 
            [   Some "System.Runtime.Remoting.TypeEntry"
                Some "System.Runtime.Remoting.TypeEntry"
                Some "System.Net.WebClient"
                Some "System.Net.WebClient.#ctor"
                Some "System.Collections.Generic.List`1"
                Some "System.Collections.Generic.List`1"
             ]
        this.TestF1Keywords(keywords, file)

    [<Test>]
    member public this.``Members`` () =        
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
        this.TestF1Keywords(keywords, file)


// Context project system
[<TestFixture>] 
type UsingProjectSystem() = 
    inherit UsingMSBuild(VsOpts = LanguageServiceExtension.ProjectSystemTestFlavour)