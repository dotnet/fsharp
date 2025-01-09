namespace EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module GenericComparison =

    let verifyCompilation compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyILBaseline

    // SOURCE=Compare01.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Compare01.dll"    # Compare01.fs -
    [<Theory; FileInlineData("Compare01.fsx")>]
    let ``Compare01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Compare02.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Compare02.dll"    # Compare02.fs -
    [<Theory; FileInlineData("Compare02.fsx")>]
    let ``Compare02_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Compare03.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Compare03.dll"    # Compare03.fs -
    [<Theory; FileInlineData("Compare03.fsx")>]
    let ``Compare03_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Compare04.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Compare04.dll"    # Compare04.fs -
    [<Theory; FileInlineData("Compare04.fsx")>]
    let ``Compare04_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Compare05.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Compare05.dll"    # Compare05.fs -
    [<Theory; FileInlineData("Compare05.fsx")>]
    let ``Compare05_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Compare06.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Compare06.dll"    # Compare06.fs -
    [<Theory; FileInlineData("Compare06.fsx")>]
    let ``Compare06_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Compare07.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Compare07.dll"    # Compare07.fs -
    [<Theory; FileInlineData("Compare07.fsx")>]
    let ``Compare07_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Compare08.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Compare08.dll"    # Compare08.fs
    [<Theory; FileInlineData("Compare08.fsx")>]
    let ``Compare08_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Compare09.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Compare09.dll"    # Compare09.fs
    [<Theory; FileInlineData("Compare09.fsx")>]
    let ``Compare09_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Compare10.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Compare10.dll"    # Compare10.fs -
    [<Theory; FileInlineData("Compare10.fsx")>]
    let ``Compare10_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Compare11.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Compare11.dll"    # Compare11.fs -
    [<Theory; FileInlineData("Compare11.fsx")>]
    let ``Compare11_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation


    // SOURCE=Hash01.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Hash01.dll"          # Hash01.fs -
    [<Theory; FileInlineData("Hash01.fsx")>]
    let ``Hash01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Hash02.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Hash02.dll"          # Hash02.fs -
    [<Theory; FileInlineData("Hash02.fsx")>]
    let ``Hash02_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Hash03.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Hash03.dll"          # Hash03.fs -
    [<Theory; FileInlineData("Hash03.fsx")>]
    let ``Hash03_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Hash04.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Hash04.dll"          # Hash04.fs -
    [<Theory; FileInlineData("Hash04.fsx")>]
    let ``Hash04_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Hash05.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Hash05.dll"          # Hash05.fs -
    [<Theory; FileInlineData("Hash05.fsx")>]
    let ``Hash05_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Hash06.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Hash06.dll"          # Hash06.fs -
    [<Theory; FileInlineData("Hash06.fsx")>]
    let ``Hash06_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Hash07.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Hash07.dll"          # Hash07.fs
    [<Theory; FileInlineData("Hash07.fsx")>]
    let ``Hash07_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Hash08.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Hash08.dll"          # Hash08.fs -
    [<Theory; FileInlineData("Hash08.fsx")>]
    let ``Hash08_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Hash09.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Hash09.dll"          # Hash09.fs -
    [<Theory; FileInlineData("Hash09.fsx")>]
    let ``Hash09_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Hash10.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Hash10.dll"          # Hash10.fs
    [<Theory; FileInlineData("Hash10.fsx")>]
    let ``Hash10_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Hash11.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Hash11.dll"          # Hash11.fs
    [<Theory; FileInlineData("Hash11.fsx")>]
    let ``Hash11_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Hash12.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Hash12.dll"          # Hash12.fs -
    [<Theory; FileInlineData("Hash12.fsx")>]
    let ``Hash12_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation


    // SOURCE=Equals01.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Equals01.dll"	# Equals01.fs -
    [<Theory; FileInlineData("Equals01.fsx")>]
    let ``Equals01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Equals02.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Equals02.dll"	# Equals02.fs -
    [<Theory; FileInlineData("Equals02.fsx")>]
    let ``Equals02_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Equals03.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Equals03.dll"	# Equals03.fs -
    [<Theory; FileInlineData("Equals03.fsx")>]
    let ``Equals03_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Equals04.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Equals04.dll"	# Equals04.fs -
    [<Theory; FileInlineData("Equals04.fsx")>]
    let ``Equals04_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Equals05.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Equals05.dll"	# Equals05.fs -
    [<Theory; FileInlineData("Equals05.fsx")>]
    let ``Equals05_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Equals06.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Equals06.dll"	# Equals06.fs -
    [<Theory; FileInlineData("Equals06.fsx")>]
    let ``Equals06_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Equals07.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Equals07.dll"	# Equals07.fs -
    [<Theory; FileInlineData("Equals07.fsx")>]
    let ``Equals07_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Equals08.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Equals08.dll"	# Equals08.fs -
    [<Theory; FileInlineData("Equals08.fsx")>]
    let ``Equals08_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Equals09.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Equals09.dll"	# Equals09.fs -
    [<Theory; FileInlineData("Equals09.fsx")>]
    let ``Equals09_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Equals10.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Equals10.dll"	# Equals10.fs -
    [<Theory; FileInlineData("Equals10.fsx")>]
    let ``Equals10_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation
        
    // SOURCE=Equals11.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Equals11.dll"	# Equals11.fs -
    [<Theory; FileInlineData("Equals11.fsx")>]
    let ``Equals11_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation
   
    // SOURCE=Equals12.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Equals12.dll"	# Equals12.fs -
    [<Theory; FileInlineData("Equals12.fsx")>]
    let ``Equals12_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Equals13.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Equals13.dll"	# Equals13.fs -
    [<Theory; FileInlineData("Equals13.fsx")>]
    let ``Equals13_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation
        
    // SOURCE=Equals14.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Equals14.dll"	# Equals14.fs -
    [<Theory; FileInlineData("Equals14.fsx")>]
    let ``Equals14_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation
   
    // SOURCE=Equals15.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Equals15.dll"	# Equals15.fs -
    [<Theory; FileInlineData("Equals15.fsx")>]
    let ``Equals15_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Equals16.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Equals16.dll"	# Equals16.fs -
    [<Theory; FileInlineData("Equals16.fsx")>]
    let ``Equals16_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation
        
    // SOURCE=Equals17.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Equals17.dll"	# Equals17.fs -
    [<Theory; FileInlineData("Equals17.fsx")>]
    let ``Equals17_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation
   
    // SOURCE=Equals18.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Equals18.dll"	# Equals18.fs -
    [<Theory; FileInlineData("Equals18.fsx")>]
    let ``Equals18_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Equals19.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Equals19.dll"	# Equals19.fs -
    [<Theory; FileInlineData("Equals19.fsx")>]
    let ``Equals19_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Equals20.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Equals20.dll"	# Equals20.fs -
    [<Theory; FileInlineData("Equals20.fsx")>]
    let ``Equals20_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Equals21.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Equals21.dll"	# Equals21.fs -
    [<Theory; FileInlineData("Equals21.fsx")>]
    let ``Equals21_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; FileInlineData("NativeIntComparison.fs")>]
    let ``NativeIntComparison_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> compileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("VoidPtrComparison.fs")>]
    let ``VoidPtrComparison_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> compileAndRun
        |> shouldSucceed
