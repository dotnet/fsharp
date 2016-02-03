// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Tests.LanguageService

open System
open System.IO
open NUnit.Framework
open Microsoft.VisualStudio.FSharp.LanguageService

[<TestFixture>] 
[<Category("LanguageService.MSBuild")>]
[<Category("LanguageService.ProjectSystem")>]
type QuickParse() = 

    let CheckIsland(tolerateJustAfter:bool, s : string, p : int, expected) =
        let actual =
            match QuickParse.GetCompleteIdentifierIsland tolerateJustAfter s p with
            | Some (s, col, _) -> Some (s, col)
            | None -> None
        Assert.AreEqual(expected, actual)
        
    [<Test>]
    member public qp.CheckGetPartialLongName() = 
        let CheckAt(line,index,expected) = 
            let actual = QuickParse.GetPartialLongNameEx(line,index)
            if actual <> expected then
                failwithf "Expected %A but got %A" expected actual
            
        let Check(line,expected) = 
            CheckAt(line, line.Length-1, expected)
    
        do Microsoft.FSharp.Compiler.AbstractIL.Diagnostics.setDiagnosticsChannel(Some(Console.Out));
        Check("let y = List.",(["List"], ""))
        Check("let y = List.conc",(["List"], "conc"))
        Check("let y = S", ([], "S"))
        Check("S", ([], "S"))
        Check("let y=", ([], ""))
        Check("Console.Wr", (["Console"], "Wr"))
        Check(" .", ([""], ""))
        Check(".", ([""], ""))
        Check("System.Console.Wr", (["System";"Console"],"Wr"))
        Check("let y=f'", ([], "f'"))
        Check("let y=SomeModule.f'", (["SomeModule"], "f'"))
        Check("let y=Some.OtherModule.f'", (["Some";"OtherModule"], "f'"))
        Check("let y=f'g", ([], "f'g"))
        Check("let y=SomeModule.f'g", (["SomeModule"], "f'g"))
        Check("let y=Some.OtherModule.f'g", (["Some";"OtherModule"], "f'g"))
        Check("let y=FSharp.Data.File.``msft-prices.csv``", ([], ""))
        Check("let y=FSharp.Data.File.``msft-prices.csv", (["FSharp";"Data";"File"], "msft-prices.csv"))
        Check("let y=SomeModule.  f", (["SomeModule"], "f"))
        Check("let y=SomeModule  .f", (["SomeModule"], "f"))
        Check("let y=SomeModule  .  f", (["SomeModule"], "f"))
        Check("let y=SomeModule  .", (["SomeModule"], ""))
        Check("let y=SomeModule  .  ", (["SomeModule"], ""))
        
        
    [<Test>] 
    member public qp.CheckIsland0() = CheckIsland(true, "", -1, None)       
    [<Test>] 
    member public qp.CheckIsland1() = CheckIsland(false, "", -1, None)
        
    [<Test>] 
    member public qp.CheckIsland2() = CheckIsland(true, "", 0, None)
    [<Test>] 
    member public qp.CheckIsland3() = CheckIsland(false, "", 0, None)
        
    [<Test>] 
    member public qp.CheckIsland4() = CheckIsland(true, null, 0, None)
    [<Test>] 
    member public qp.CheckIsland5() = CheckIsland(false, null, 0, None)
        
    [<Test>] 
    member public qp.CheckIsland6() = CheckIsland(false, "identifier", 0, Some("identifier",10))
    [<Test>] 
    member public qp.CheckIsland7() = CheckIsland(false, "identifier", 8, Some("identifier",10))
        
    [<Test>] 
    member public qp.CheckIsland8() = CheckIsland(true, "identifier", 0, Some("identifier",10))
    [<Test>] 
    member public qp.CheckIsland9() = CheckIsland(true, "identifier", 8, Some("identifier",10))

    // A place where tolerateJustAfter matters
    [<Test>] 
    member public qp.CheckIsland10() = CheckIsland(false, "identifier", 10, None)
    [<Test>] 
    member public qp.CheckIsland11() = CheckIsland(true, "identifier", 10, Some("identifier",10))

    // Index which overflows the line
    [<Test>] 
    member public qp.CheckIsland12() = CheckIsland(true, "identifier", 11, None)
    [<Test>] 
    member public qp.CheckIsland13() = CheckIsland(false, "identifier", 11, None)
    
    // Match active pattern identifiers
    [<Test>]
    member public qp.CheckIsland14() = CheckIsland(false, "|Identifier|", 0, Some("|Identifier|",12))    
    [<Test>]
    member public qp.CheckIsland15() = CheckIsland(true, "|Identifier|", 0, Some("|Identifier|",12))        
    [<Test>]
    member public qp.CheckIsland16() = CheckIsland(false, "|Identifier|", 12, None)    
    [<Test>]
    member public qp.CheckIsland17() = CheckIsland(true, "|Identifier|", 12, Some("|Identifier|",12))        
    [<Test>] 
    member public qp.CheckIsland18() = CheckIsland(false, "|Identifier|", 13, None)    
    [<Test>] 
    member public qp.CheckIsland19() = CheckIsland(true, "|Identifier|", 13, None)        
    
    // ``Quoted`` identifiers
    [<Test>]
    member public qp.CheckIsland20() = CheckIsland(false, "``Space Man``", 0, Some("``Space Man``",13))    
    [<Test>]
    member public qp.CheckIsland21() = CheckIsland(true, "``Space Man``", 0, Some("``Space Man``",13))    
    [<Test>]
    member public qp.CheckIsland22() = CheckIsland(false, "``Space Man``", 10, Some("``Space Man``",13))    
    [<Test>]
    member public qp.CheckIsland23() = CheckIsland(true, "``Space Man``", 10, Some("``Space Man``",13))    
    [<Test>]
    member public qp.CheckIsland24() = CheckIsland(false, "``Space Man``", 11, Some("``Space Man``",13))    
    // [<Test>]
    // member public qp.CheckIsland25() = CheckIsland(true, "``Space Man``", 11, Some("Man",11))  // This is probably not what the user wanted. Not enforcing this test.
    [<Test>]
    member public qp.CheckIsland26() = CheckIsland(false, "``Space Man``", 12, Some("``Space Man``",13))    
    [<Test>]
    member public qp.CheckIsland27() = CheckIsland(true, "``Space Man``", 12, Some("``Space Man``",13))    
    [<Test>]
    member public qp.CheckIsland28() = CheckIsland(false, "``Space Man``", 13, None)    
    [<Test>]
    member public qp.CheckIsland29() = CheckIsland(true, "``Space Man``", 13, Some("``Space Man``",13))    
    [<Test>] 
    member public qp.CheckIsland30() = CheckIsland(false, "``Space Man``", 14, None)    
    [<Test>] 
    member public qp.CheckIsland31() = CheckIsland(true, "``Space Man``", 14, None)    
    [<Test>] 
    member public qp.CheckIsland32() = CheckIsland(true, "``msft-prices.csv``", 14, Some("``msft-prices.csv``",19))    
    // handle extracting islands from arrays    
    [<Test>] 
    member public qp.CheckIsland33() = CheckIsland(true, "[|abc;def|]", 2, Some("abc",5))    
    [<Test>] 
    member public qp.CheckIsland34() = CheckIsland(true, "[|abc;def|]", 4, Some("abc",5))    
    [<Test>] 
    member public qp.CheckIsland35() = CheckIsland(true, "[|abc;def|]", 5, Some("abc",5))    
    [<Test>] 
    member public qp.CheckIsland36() = CheckIsland(true, "[|abc;def|]", 6, Some("def",9))    
    [<Test>] 
    member public qp.CheckIsland37() = CheckIsland(true, "[|abc;def|]", 8, Some("def",9))    
    [<Test>] 
    member public qp.CheckIsland38() = CheckIsland(true, "[|abc;def|]", 9, Some("def",9))    
    [<Test>] 
    member public qp.CheckIsland39() = CheckIsland(false, "identifier(*boo*)", 0, Some("identifier",10))
    [<Test>] 
    member public qp.CheckIsland40() = CheckIsland(true, "identifier(*boo*)", 0, Some("identifier",10))
    [<Test>] 
    member public qp.CheckIsland41() = CheckIsland(false, "identifier(*boo*)", 10, None)
    [<Test>] 
    member public qp.CheckIsland42() = CheckIsland(true, "identifier(*boo*)", 10, Some("identifier",10))
    [<Test>] 
    member public qp.CheckIsland43() = CheckIsland(false, "identifier(*boo*)", 11, None)
    [<Test>] 
    member public qp.CheckIsland44() = CheckIsland(true, "identifier(*boo*)", 11, None)
    [<Test>] 
    member public qp.CheckIsland45() = CheckIsland(false, "``Space Man (*boo*)``", 13, Some("``Space Man (*boo*)``",21))
    [<Test>] 
    member public qp.CheckIsland46() = CheckIsland(true, "``Space Man (*boo*)``", 13, Some("``Space Man (*boo*)``",21))
    [<Test>] 
    member public qp.CheckIsland47() = CheckIsland(false, "(*boo*)identifier", 11, Some("identifier",17))
    [<Test>] 
    member public qp.CheckIsland48() = CheckIsland(true, "(*boo*)identifier", 11, Some("identifier",17))
    [<Test>] 
    member public qp.CheckIsland49() = CheckIsland(false, "identifier(*(*  *)boo*)", 0, Some("identifier",10))
    [<Test>] 
    member public qp.CheckIsland50() = CheckIsland(true, "identifier(*(*  *)boo*)", 0, Some("identifier",10))
