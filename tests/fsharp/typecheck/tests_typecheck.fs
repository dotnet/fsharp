module ``FSharp-Tests-Typecheck``

open System
open System.IO
open NUnit.Framework

open FSharpTestSuiteTypes
open NUnitConf
open PlatformHelpers
open SingleTest


[<Test>]
let ``full-rank-arrays`` () = check (attempt {
    let cfg = FSharpTestSuite.testConfig "typecheck/full-rank-arrays"

    do! csc cfg "/target:library /out:HighRankArrayTests.dll" ["Class1.cs"]

    do! SingleTest.singleTestBuildAndRunAux cfg FSC_OPT_PLUS_DEBUG
    })


[<Test>]
let misc () = singleTestBuildAndRun "typecheck/misc" FSC_OPT_PLUS_DEBUG

[<Test>]
let ``sigs pos24`` () = check (attempt {
        let cfg = FSharpTestSuite.testConfig "typecheck/sigs"
        do! fsc cfg "%s --target:exe -o:pos24.exe" cfg.fsc_flags ["pos24.fs"]
        do! peverify cfg "pos24.exe"
    })

[<Test>]
let ``sigs pos23`` () = check (attempt {
        let cfg = FSharpTestSuite.testConfig "typecheck/sigs"
        do! fsc cfg "%s --target:exe -o:pos23.exe" cfg.fsc_flags ["pos23.fs"]
        do! peverify cfg "pos23.exe"
        do! exec cfg ("."/"pos23.exe") ""

    })

[<Test>]
let ``sigs pos20`` () = check (attempt {
        let cfg = FSharpTestSuite.testConfig "typecheck/sigs"
        do! fsc cfg "%s --target:exe -o:pos20.exe" cfg.fsc_flags ["pos20.fs"]
        do! peverify cfg "pos20.exe"
        do! exec cfg ("."/"pos20.exe") ""

    })

[<Test>]
let ``sigs pos19`` () = check (attempt {
        let cfg = FSharpTestSuite.testConfig "typecheck/sigs"
        do! fsc cfg "%s --target:exe -o:pos19.exe" cfg.fsc_flags ["pos19.fs"]
        do! peverify cfg "pos19.exe"
        do! exec cfg ("."/"pos19.exe") ""

    })

[<Test>]
let ``sigs pos18`` () = check (attempt {
        let cfg = FSharpTestSuite.testConfig "typecheck/sigs"
        do! fsc cfg "%s --target:exe -o:pos18.exe" cfg.fsc_flags ["pos18.fs"]
        do! peverify cfg "pos18.exe"
        do! exec cfg ("."/"pos18.exe") ""

    })

[<Test>]
let ``sigs pos16`` () = check (attempt {
        let cfg = FSharpTestSuite.testConfig "typecheck/sigs"
        do! fsc cfg "%s --target:exe -o:pos16.exe" cfg.fsc_flags ["pos16.fs"]
        do! peverify cfg "pos16.exe"
        do! exec cfg ("."/"pos16.exe") ""

    })

[<Test>]
let ``sigs pos17`` () = check (attempt {
        let cfg = FSharpTestSuite.testConfig "typecheck/sigs"
        do! fsc cfg "%s --target:exe -o:pos17.exe" cfg.fsc_flags ["pos17.fs"]

        do! peverify cfg "pos17.exe"

        do! exec cfg ("."/"pos17.exe") ""

    })

[<Test>]
let ``sigs pos15`` () = check (attempt {
        let cfg = FSharpTestSuite.testConfig "typecheck/sigs"
        do! fsc cfg "%s --target:exe -o:pos15.exe" cfg.fsc_flags ["pos15.fs"]

        do! peverify cfg "pos15.exe"

        do! exec cfg ("."/"pos15.exe") ""

    })

[<Test>]
let ``sigs pos14`` () = check (attempt {
        let cfg = FSharpTestSuite.testConfig "typecheck/sigs"
        do! fsc cfg "%s --target:exe -o:pos14.exe" cfg.fsc_flags ["pos14.fs"]

        do! peverify cfg "pos14.exe"

        do! exec cfg ("."/"pos14.exe") ""

    })

[<Test>]
let ``sigs pos13`` () = check (attempt {
        let cfg = FSharpTestSuite.testConfig "typecheck/sigs"
        do! fsc cfg "%s --target:exe -o:pos13.exe" cfg.fsc_flags ["pos13.fs"]

        do! peverify cfg "pos13.exe"

        do! exec cfg ("."/"pos13.exe") ""

    })

[<Test>]
let ``sigs pos12 `` () = check (attempt {
        let cfg = FSharpTestSuite.testConfig "typecheck/sigs"
        do! fsc cfg "%s -a -o:pos12.dll" cfg.fsc_flags ["pos12.fs"]

    })

[<Test>]
let ``sigs pos11`` () = check (attempt {
        let cfg = FSharpTestSuite.testConfig "typecheck/sigs"
        do! fsc cfg "%s -a -o:pos11.dll" cfg.fsc_flags ["pos11.fs"]

    })

[<Test>]
let ``sigs pos10`` () = check (attempt {
        let cfg = FSharpTestSuite.testConfig "typecheck/sigs"
        do! fsc cfg "%s -a -o:pos10.dll" cfg.fsc_flags ["pos10.fs"]

        do! peverify cfg "pos10.dll"

    })

[<Test>]
let ``sigs pos09`` () = check (attempt {
        let cfg = FSharpTestSuite.testConfig "typecheck/sigs"
        do! fsc cfg "%s -a -o:pos09.dll" cfg.fsc_flags ["pos09.fs"]

        do! peverify cfg "pos09.dll"

    })


[<Test>]
let ``sigs pos07`` () = check (attempt {
        let cfg = FSharpTestSuite.testConfig "typecheck/sigs"
        do! fsc cfg "%s -a -o:pos07.dll" cfg.fsc_flags ["pos07.fs"]

        do! peverify cfg "pos07.dll"

    })

[<Test>]
let ``sigs pos08`` () = check (attempt {
        let cfg = FSharpTestSuite.testConfig "typecheck/sigs"
        do! fsc cfg "%s -a -o:pos08.dll" cfg.fsc_flags ["pos08.fs"]

        do! peverify cfg "pos08.dll"

    })

[<Test>]
let ``sigs pos06`` () = check (attempt {
        let cfg = FSharpTestSuite.testConfig "typecheck/sigs"
        do! fsc cfg "%s -a -o:pos06.dll" cfg.fsc_flags ["pos06.fs"]

        do! peverify cfg "pos06.dll"

    })

[<Test>]
let ``sigs pos03`` () = check (attempt {
        let cfg = FSharpTestSuite.testConfig "typecheck/sigs"
        do! fsc cfg "%s -a -o:pos03.dll" cfg.fsc_flags ["pos03.fs"]

        do! peverify cfg "pos03.dll"

        do! fsc cfg "%s -a -o:pos03a.dll" cfg.fsc_flags ["pos03a.fsi"; "pos03a.fs"]

        do! peverify cfg "pos03a.dll"

    })

[<Test>]
let ``sigs pos01a`` () = check (attempt {
        let cfg = FSharpTestSuite.testConfig "typecheck/sigs"
        do! fsc cfg "%s -a -o:pos01a.dll" cfg.fsc_flags ["pos01a.fsi"; "pos01a.fs"]

        do! peverify cfg "pos01a.dll"

    })

[<Test>]
let ``sigs pos02`` () = check (attempt {
        let cfg = FSharpTestSuite.testConfig "typecheck/sigs"
        do! fsc cfg "%s -a -o:pos02.dll" cfg.fsc_flags ["pos02.fs"]

        do! peverify cfg "pos02.dll"

    })


[<Test>]
let ``sigs pos05`` () = check (attempt {
        let cfg = FSharpTestSuite.testConfig "typecheck/sigs"
        do! fsc cfg "%s -a -o:pos05.dll" cfg.fsc_flags ["pos05.fs"]

        })

let negGroup negs = check (attempt {
        let cfg = FSharpTestSuite.testConfig "typecheck/sigs"
        do! attempt.For (negs, singleNegTest cfg)

    })


[<Test>]
let ``sigs neg group1`` () = negGroup ["neg97"; "neg96"; "neg95"; "neg94"; "neg93"; "neg92"; "neg91" ]

[<Test>]
let ``sigs neg group2`` () = negGroup ["neg90"; "neg89"; "neg88"; "neg35" ]

[<Test>]
let ``sigs neg group3`` () = negGroup ["neg87"; "neg86"; "neg85"; "neg84"; "neg83"; "neg82"; "neg81"; "neg80"; "neg79"; "neg78"; "neg77"; "neg76"; "neg75"; ]

[<Test>]
let ``sigs neg group4`` () = negGroup ["neg74"; "neg73"; "neg72"; "neg71"; "neg70"; "neg69"; "neg68"; "neg67"; "neg66"; "neg65"; "neg64"; "neg61"; "neg63"; ]

[<Test>]
let ``sigs neg group5`` () = negGroup ["neg62"; "neg20"; "neg24"; "neg32"; "neg37"; "neg37_a"; "neg60"; "neg59"; "neg58"; "neg57"; "neg56"; "neg56_a"; "neg56_b" ]
[<Test>]
let ``sigs neg group6`` () = negGroup ["neg55"; "neg54"; "neg53"; "neg52"; "neg51"; "neg50"; "neg49"; "neg48"; "neg47"; "neg46"; "neg10"; "neg10_a"; "neg45"; ]

[<Test>]
let ``sigs neg group7`` () = negGroup ["neg44"; "neg43"; "neg38"; "neg39"; "neg40"; "neg41"; "neg42"]

[<Test>]
let ``sigs neg group8`` () = negGroup ["neg34"; "neg33"; "neg30"; "neg31"; "neg29"; "neg28"; "neg07"; "neg_byref_20";  ]

[<Test>]
let ``sigs neg group9`` () = negGroup [ "neg_byref_1"; "neg_byref_2"; "neg_byref_3"; "neg_byref_4"; "neg_byref_5"; "neg_byref_6"; "neg_byref_7"; "neg_byref_8";  ]

[<Test>]
let ``sigs neg group10`` () = negGroup ["neg_byref_10"; "neg_byref_11"; "neg_byref_12"; "neg_byref_13"; "neg_byref_14"; "neg_byref_15"; "neg_byref_16";   ]

[<Test>]
let ``sigs neg group11`` () = negGroup [ "neg_byref_17"; "neg_byref_18"; "neg_byref_19"; "neg_byref_21"; "neg_byref_22"; "neg_byref_23"; "neg36"; "neg17"; "neg26";  ]

[<Test>]
let ``sigs neg group12`` () = negGroup [ "neg27"; "neg25"; "neg03"; "neg23"; "neg22"; "neg21"; "neg04"; "neg05"; "neg06"; "neg06_a"; "neg06_b"; "neg08"; "neg09";  ]

[<Test>]
let ``sigs neg group13`` () = negGroup [ "neg11"; "neg12"; "neg13"; "neg14"; "neg16"; "neg18"; "neg19"; "neg01"; "neg02"; "neg15"  ]

