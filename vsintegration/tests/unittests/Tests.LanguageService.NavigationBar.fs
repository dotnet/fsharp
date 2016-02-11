// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Tests.LanguageService.NavigationBar

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

        (* Files for testing and tests --------------------------------------------------------- *)
                
    let NavigationFile1 = 
      [ "#light"
        "module Example.Module"
        ""
        "module SomeModule = "
        "    type Rec = (*1s*){"
        "      RFirst : int "
        "      RSecond : string }(*1e*)"
        ""
        "    type Rec2 = "
        "    (*2s*){ R2First : int "
        "            R2Second : string "
        "          }(*2e*)"
        ""
        "    type Abbrev = (*3s*)Microsoft.FSharp.Collections.List<int * string>(*3e*)"
        ""
        "    type Enum = "
        "(*4s*)| Aaa = 0"
        "      | Bbb = 3(*4e*)"
        ""
        "    type EnumOneLine = (*5s*)| OUAaa = 0 | OUBbb = 3(*5e*)" ]

    let NavigationFile2 = 
      [ "#light"
        "module A =   "
        ""
        "    type RecWith = (*7s*){ "
        "        WRFirst : int "
        "        WRSecond : string } with"
        "      member zz.RecMember(s:string) = "
        "        printfn \"recdmember\""
        "        3(*7e*)"
        ""
        "module B =   "
        "    type Union = "
        "(*8s*)| UFirst"
        "      | USecond of int * string"
        "      member x.A = 0(*8e*)"
        ""
        "    type A() = "
        "(*9s*)let rec a = (fun () -> b)"
        "      and b = a() + 1"
        "      member xx.Prop = 10"
        "      member x.Func() = ()"
        "      interface System.IDisposable with"
        "        member x.Dispose() = ()(*9e*)"
        ""
        "    exception SomeCrash of (*10s*)int * string * list<unit>(*10e*)"
        "    "
        "    module MyList = (*11s*)Microsoft.FSharp.Collections.List(*11e*)"
        ""
        "    module MoreNested = "
        "        type MyEnum ="
        "     (*12s*)| One = 1"
        "            | Two = 2"
        "            | Three = 3(*12e*)"
        "      "
        "    type AExt with // this isn't valid extension, but it's ok for the parser"
        "(*13s*)member x.Extension1() = 2"
        "       member x.Extension2() = 2(*13e*) // TODO: this doesn't work correctly"
        "          "
        "    type Z() ="
        "(*14s*)let mutable cache = 0"
        "       member x.Z : int =  // type annotation can cause issues"
        "         0(*14e*)"
        ""
        "    module (*15s*)FooBaz = // collapsed immediately after the identifier"
        "      let toplevel () = "
        "        let nested = 10"
        "        0"
        "      "
        "      module (*16s*)NestedModule = "
        "        let nestedThing () ="
        "          0(*16e*)"
        "          "
        "      module OtherNested ="
        "        let aa () ="
        "          1(*15e*)        " ]
    

    (* Unit tests for collapse region & navigation bar ------------------------------------- *)
    
    member private this.TestNavigationBar (file : string list) findDecl expectedMembers =
        
        let (_, _, file) = this.CreateSingleFileProject(file)

        // Verify that the types&modules list contains 'findDecl'
        let navigation = GetNavigationContentAtCursor(file)
        AssertNavigationContains(navigation.TypesAndModules, findDecl)
        let idxDecl = navigation.TypesAndModules |> Array.findIndex (fun nav -> nav.Label = findDecl)
        let decl = navigation.TypesAndModules.[idxDecl]
        
        // Navigate to the definition and get the contents again
        MoveCursorTo(file, decl.Span.iStartLine + 1, decl.Span.iStartIndex) // line index is in the 0-based form
        let navigation = GetNavigationContentAtCursor(file)
        // Ensure that the right thing is selected in the dropdown
        AssertEqual(idxDecl, navigation.SelectedType)
        // Ensure that member list contains expected members
        AssertNavigationContainsAll(navigation.Members, expectedMembers)
        
        // Find member declaration, go to the location & test the identifier island
        for memb in expectedMembers do
            let decl = navigation.Members |> Array.find (fun nav -> nav.Label = memb)
            MoveCursorTo(file, decl.Span.iStartLine + 1, decl.Span.iStartIndex + 1)
            match GetIdentifierAtCursor file with
            | None -> 
                Assert.Fail("No identifier at cursor!")
            | Some (id, _) -> 
                if not (id.Contains(memb)) then Assert.Fail(sprintf "Found '%s' which isn't substring of the expected '%s'." id memb)
               
    member private this.TestHiddenRegions (file : list<string>) regionMarkers =        
        let (_, _, file) = this.CreateSingleFileProject(file)

        // Find locations of the regions based on markers provided
        let expectedLocations = 
          [ for ms, me in regionMarkers do
              MoveCursorToEndOfMarker(file, ms)
              let sl, sc = GetCursorLocation(file)
              MoveCursorToStartOfMarker(file, me)
              let el, ec = GetCursorLocation(file)
              // -1 to adjust line to VS format
              // -1 to adjust columns (not quite sure why..)
              yield (sl-1, sc-1), (el-1, ec-1) ]
              
        // Test whether the regions are same and that no 'update' commands are created (no regions exist prior to this call)
        let toCreate, toUpdate = GetHiddenRegionCommands(file)
        if (toUpdate <> Map.empty<_,_>) then 
            Assert.Fail("Hidden regions, first call. Didn't expect any regions to update.")
        AssertEqualWithMessage(expectedLocations.Length, toCreate.Length, "Different number of regions!")
        AssertRegionListContains(expectedLocations, toCreate)

    [<Test>]
    member public this.``Regions.NavigationFile1``() =        
        this.TestHiddenRegions NavigationFile1
          [ "(*1s*)", "(*1e*)"
            "(*2s*)", "(*2e*)" 
            "(*3s*)", "(*3e*)"
            "(*4s*)", "(*4e*)"
            "(*5s*)", "(*5e*)"
            "SomeModule", "(*5e*)" (* entire module *) ]
                          
    [<Test>]
    member public this.``Record1``() =        
        this.TestNavigationBar NavigationFile1 "SomeModule.Rec" ["RFirst"; "RSecond"]
        
    [<Test>]
    member public this.``Record2``() =        
        this.TestNavigationBar NavigationFile1 "SomeModule.Rec2" ["R2First"; "R2Second"]
          
    [<Test>]
    member public this.``Abbreviation``() =        
        this.TestNavigationBar NavigationFile1 "SomeModule.Abbrev" []

    [<Test>]
    member public this.``Enum``() =        
        this.TestNavigationBar NavigationFile1 "SomeModule.Enum" [ "Aaa"; "Bbb" ]

    [<Test>]
    member public this.``Enum.OneLine``() =        
        this.TestNavigationBar NavigationFile1 "SomeModule.EnumOneLine" [ "OUAaa"; "OUBbb" ]

    [<Test>]
    member public this.``Record.WithMembers``() =        
        this.TestNavigationBar NavigationFile2 "A.RecWith" ["WRFirst"; "WRSecond"; "RecMember" ]
        
    [<Test>]
    member public this.``Union.WithMembers``() =        
        this.TestNavigationBar NavigationFile2 "B.Union" ["UFirst"; "USecond"; "A"]
          
    [<Test>]
    member public this.``Class``() =        
        this.TestNavigationBar NavigationFile2 "B.A" ["Prop"; "Func"; "Dispose"] // perhaps IDisposable.Dispose

    [<Test>]
    member public this.``Exception``() =        
        this.TestNavigationBar NavigationFile2 "B.SomeCrash" [ ]
        
    [<Test>]
    member public this.``Module.Alias``() =        
        this.TestNavigationBar NavigationFile2 "B.MyList" [ ]

    [<Test>]
    member public this.``NestedEnum``() =        
        this.TestNavigationBar NavigationFile2 "B.MoreNested.MyEnum" [ "One"; "Two"; "Three" ]

    [<Test>]
    member public this.``Extension``() =        
        this.TestNavigationBar NavigationFile2 "B.AExt" [ "Extension1"; "Extension2" ]
        
    [<Test>]
    member public this.``Type.EndingWithProperty.WithTypeAnnotation``() =        
        this.TestNavigationBar NavigationFile2 "B.Z" [ "Z" ]   
             
    [<Test>]
    member public this.``Module.Nested``() =        
        this.TestNavigationBar NavigationFile2 "B.FooBaz" [ "toplevel" ]

    [<Test>]
    member public this.``Module.Nested.More``() =        
        this.TestNavigationBar NavigationFile2 "B.FooBaz.NestedModule" [ "nestedThing" ]   


// Context project system
[<TestFixture>] 
type UsingProjectSystem() = 
    inherit UsingMSBuild(VsOpts = LanguageServiceExtension.ProjectSystemTestFlavour)