module FSharpTestSuiteAsserts

open PlatformHelpers
open FSharpTestSuiteTypes

let requireVSUltimate cfg = attempt {
    do! match cfg.INSTALL_SKU with
        | Some (Ultimate) -> Success
        | x ->
            // IF /I "%INSTALL_SKU%" NEQ "ULTIMATE" (
            //     echo Test not supported except on Ultimate
            NUnitConf.skip (sprintf "Test not supported except on Ultimate, was %A" x)
            //     exit /b 0
            // )
    }

let requireENCulture () = attempt {
    do! match System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName with
        | "en" -> Success
        | c ->
            NUnitConf.skip (sprintf "Test not supported except en Culture, was %s" c)
    }
