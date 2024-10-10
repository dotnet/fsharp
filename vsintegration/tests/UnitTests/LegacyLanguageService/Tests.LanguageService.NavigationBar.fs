// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests.LanguageService.NavigationBar

open System
open Xunit
open Salsa.Salsa
open Salsa.VsOpsUtils
open UnitTests.TestLib.Salsa
open UnitTests.TestLib.Utils
open UnitTests.TestLib.LanguageService
open UnitTests.TestLib.ProjectSystem

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
    

// Context project system
type UsingProjectSystem() = 
    inherit UsingMSBuild(VsOpts = LanguageServiceExtension.ProjectSystemTestFlavour)