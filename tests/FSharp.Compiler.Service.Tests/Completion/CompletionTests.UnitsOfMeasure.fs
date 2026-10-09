module FSharp.Compiler.Service.Tests.CompletionUnitsOfMeasureTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``UnitMeasure.Bug78932_1`` () =
    let info =
        Checker.getCompletionInfo
            """
            module M1 =
               [<Measure>] type Kg
 
            module M2 = 
                let f = 1<M1.{caret} >  // <- type . between M1 and ' >'   => works"""

    assertHasItemWithNames [ "Kg" ] info

[<Fact>]
let ``UnitMeasure.Bug78932_2`` () =
    let info =
        Checker.getCompletionInfo
            """
            module M1 =
               [<Measure>] type Kg
 
            module M2 = 
                let f = 1<M1.{caret}>  // <- type . between M1 and '>'     => no popup intellisense"""

    assertHasItemWithNames [ "Kg" ] info

[<Fact>]
let ``UnitMeasure.UnitNames`` () =
    let info =
        Checker.getCompletionInfo
            """Microsoft.FSharp.Data.UnitSystems.SI.UnitNames.{caret}"""

    assertHasItemWithNames
        [ "ampere"; "becquerel"; "candela"; "coulomb"; "farad"; "gray"; "henry"; "hertz"; "joule"; "katal"; "kelvin"; "kilogram"; "lumen"; "lux"; "metre"; "mole"; "newton"; "ohm"; "pascal"; "second"; "siemens"; "sievert"; "tesla"; "volt"; "watt"; "weber" ]
        info

[<Fact>]
let ``UnitMeasure.UnitSymbols`` () =
    let info =
        Checker.getCompletionInfo
            """Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols.{caret}"""

    assertHasItemWithNames
        [ "A"; "Bq"; "C"; "F"; "Gy"; "H"; "Hz"; "J"; "K"; "N"; "Pa"; "S"; "Sv"; "T"; "V"; "W"; "Wb"; "cd"; "kat"; "kg"; "lm"; "lx"; "m"; "mol"; "ohm"; "s" ]
        info

[<Theory>]
[<InlineData("""
                namespace MyNamespace1
                module MyModule =
                    type DuType =
                         | Tag of int         
                    let f (DuType(*Maftervariable1*).Tag(x)) = 10 
                    type Pet() = 
                        member x.Name = "pet"
                        member x.Speak() = "this is a pet"
                    type Dog() = 
                        inherit Pet()
                        do base(*Maftervariable3*).GetType()
                    let dog = new Dog()
                namespace MyNamespace2
                module MyModule2 = 
                    let typeFunc<[<MyNamespace1.MyModule.{caret}>] 'a> = [1; 2; 3]
                    let f (x:MyNamespace1.MyModule(*Maftervariable4*)) = 10
                    let y = int System.IO(*Maftervariable5*)""")>]
[<InlineData("""
                namespace MyNamespace1
                module MyModule =
                    type DuType =
                         | Tag of int          
                    type Pet() = 
                        member x.Name = "pet"
                        member x.Speak() = "this is a pet"  
                    type Dog() = 
                        inherit Pet() 
                namespace MyNamespace2
                module MyModule2 =     
                    let foo = MyNamespace1.MyModule(*Mtypeparameter1*)
                    let f (x:int) = MyNamespace1.MyModule.DuType(*Mtypeparameter2*)    
                    let typeFunc<[<MyNamespace1.MyModule.{caret}>] 'a> = 10""")>]
let ``UnitMeasure.AsTypeParameter.DefFromDiffNamespace`` (markedSource: string) =
    let info = Checker.getCompletionInfo markedSource

    assertHasItemWithNames [ "DuType"; "Pet"; "Dog" ] info
