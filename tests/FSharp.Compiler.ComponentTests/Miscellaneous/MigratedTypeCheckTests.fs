// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
module Miscellaneous.FsharpSuiteMigrated_TypeCheckTests

open Xunit
open FSharp.Test
open FSharp.Test.ScriptHelpers 
open Miscellaneous.FsharpSuiteMigrated.TestFrameworkAdapter  

[<Fact>]
let misc () = singleTestBuildAndRunVersion "typecheck/misc" FSC_OPTIMIZED LangVersion.SupportsMl

[<FactForDESKTOP(Skip = "Failing in new test framework")>]
let ``type check neg01`` () = singleNegTest ( "typecheck/sigs") "neg01"

[<FactForDESKTOP>]
let ``type check neg02`` () = singleVersionedNegTest ( "typecheck/sigs") LangVersion.V60 "neg02"

[<FactForDESKTOP(Skip = "Failing in new test framework")>]
let ``type check neg03`` () = singleVersionedNegTest ( "typecheck/sigs") LangVersion.SupportsMl "neg03"

[<FactForDESKTOP>]
let ``type check neg04`` () = singleNegTest ( "typecheck/sigs") "neg04"

[<FactForDESKTOP>]
let ``type check neg05`` () = singleNegTest ( "typecheck/sigs") "neg05"

[<FactForDESKTOP>]
let ``type check neg06`` () = singleNegTest ( "typecheck/sigs") "neg06"

[<FactForDESKTOP>]
let ``type check neg06_a`` () = singleNegTest ( "typecheck/sigs") "neg06_a"

[<FactForDESKTOP>]
let ``type check neg06_b`` () = singleNegTest ( "typecheck/sigs") "neg06_b"

[<FactForDESKTOP>]
let ``type check neg07`` () = singleNegTest ( "typecheck/sigs") "neg07"

[<FactForDESKTOP(Skip = "Failing in new test framework")>]
let ``type check neg08`` () = singleNegTest ( "typecheck/sigs") "neg08"

[<FactForDESKTOP(Skip = "Failing in new test framework")>]
let ``type check neg09`` () = singleNegTest ( "typecheck/sigs") "neg09"

[<FactForDESKTOP(Skip = "Failing in new test framework")>]
let ``type check neg10`` () = singleNegTest ( "typecheck/sigs") "neg10"

[<FactForDESKTOP>]
let ``type check neg10_a`` () = singleNegTest ( "typecheck/sigs") "neg10_a"

[<FactForDESKTOP>]
let ``type check neg11`` () = singleNegTest ( "typecheck/sigs") "neg11"

[<FactForDESKTOP>]
let ``type check neg12`` () = singleNegTest ( "typecheck/sigs") "neg12"

[<FactForDESKTOP>]
let ``type check neg13`` () = singleNegTest ( "typecheck/sigs") "neg13"

[<FactForDESKTOP(Skip = "Failing in new test framework")>]
let ``type check neg14`` () = singleNegTest ( "typecheck/sigs") "neg14"

[<FactForDESKTOP>]
let ``type check neg15`` () = singleNegTest ( "typecheck/sigs") "neg15"

[<FactForDESKTOP>]
let ``type check neg16`` () = singleNegTest ( "typecheck/sigs") "neg16"

[<FactForDESKTOP(Skip = "Failing in new test framework")>]
let ``type check neg17`` () = singleNegTest ( "typecheck/sigs") "neg17"

[<FactForDESKTOP>]
let ``type check neg18`` () = singleNegTest ( "typecheck/sigs") "neg18"

[<FactForDESKTOP>]
let ``type check neg19`` () = singleNegTest ( "typecheck/sigs") "neg19"

[<FactForDESKTOP>]
let ``type check neg20`` () = singleNegTest ( "typecheck/sigs") "neg20"

[<FactForDESKTOP>]
let ``type check neg20 version 5_0`` () =
    let cfg =  "typecheck/sigs/version50"
    singleVersionedNegTest cfg LangVersion.V50 "neg20"

[<FactForDESKTOP>]
let ``type check neg21`` () = singleNegTest ( "typecheck/sigs") "neg21"

[<FactForDESKTOP>]
let ``type check neg23`` () = singleNegTest ( "typecheck/sigs") "neg23"


[<FactForDESKTOP>]
let ``type check neg25`` () = singleNegTest ( "typecheck/sigs") "neg25"

[<FactForDESKTOP>]
let ``type check neg26`` () = singleNegTest ( "typecheck/sigs") "neg26"

[<FactForDESKTOP(Skip = "Failing in new test framework")>]
let ``type check neg27`` () = singleNegTest ( "typecheck/sigs") "neg27"

[<FactForDESKTOP>]
let ``type check neg28`` () = singleNegTest ( "typecheck/sigs") "neg28"

[<FactForDESKTOP>]
let ``type check neg29`` () = singleNegTest ( "typecheck/sigs") "neg29"

[<FactForDESKTOP>]
let ``type check neg30`` () = singleNegTest ( "typecheck/sigs") "neg30"

[<FactForDESKTOP(Skip = "Failing in new test framework")>]
let ``type check neg31`` () = singleNegTest ( "typecheck/sigs") "neg31"

[<FactForDESKTOP>]
let ``type check neg32`` () = singleNegTest ( "typecheck/sigs") "neg32"

[<FactForDESKTOP(Skip = "Failing in new test framework")>]
let ``type check neg33`` () = singleNegTest ( "typecheck/sigs") "neg33"

[<FactForDESKTOP>]
let ``type check neg34`` () = singleNegTest ( "typecheck/sigs") "neg34"

[<FactForDESKTOP>]
let ``type check neg35`` () = singleNegTest ( "typecheck/sigs") "neg35"

[<FactForDESKTOP>]
let ``type check neg36`` () = singleNegTest ( "typecheck/sigs") "neg36"

[<FactForDESKTOP>]
let ``type check neg37`` () = singleNegTest ( "typecheck/sigs") "neg37"

[<FactForDESKTOP>]
let ``type check neg37_a`` () = singleNegTest ( "typecheck/sigs") "neg37_a"

[<FactForDESKTOP>]
let ``type check neg38`` () = singleNegTest ( "typecheck/sigs") "neg38"

[<FactForDESKTOP>]
let ``type check neg39`` () = singleNegTest ( "typecheck/sigs") "neg39"

[<FactForDESKTOP>]
let ``type check neg40`` () = singleNegTest ( "typecheck/sigs") "neg40"

[<FactForDESKTOP>]
let ``type check neg41`` () = singleNegTest ( "typecheck/sigs") "neg41"

[<FactForDESKTOP>]
let ``type check neg42`` () = singleNegTest ( "typecheck/sigs") "neg42"

[<FactForDESKTOP(Skip = "Failing in new test framework")>]
let ``type check neg43`` () = singleNegTest ( "typecheck/sigs") "neg43"

[<FactForDESKTOP>]
let ``type check neg44`` () = singleNegTest ( "typecheck/sigs") "neg44"

#if !DEBUG // requires release version of compiler to avoid very deep stacks
[<FactForDESKTOP>]
let ``type check neg45`` () = singleNegTest ( "typecheck/sigs") "neg45"
#endif

[<FactForDESKTOP>]
let ``type check neg46`` () = singleNegTest ( "typecheck/sigs") "neg46"

[<FactForDESKTOP>]
let ``type check neg47`` () = singleNegTest ( "typecheck/sigs") "neg47"

[<FactForDESKTOP>]
let ``type check neg48`` () = singleNegTest ( "typecheck/sigs") "neg48"

[<FactForDESKTOP(Skip = "Failing in new test framework")>]
let ``type check neg49`` () = singleNegTest ( "typecheck/sigs") "neg49"

[<FactForDESKTOP>]
let ``type check neg50`` () = singleNegTest ( "typecheck/sigs") "neg50"

[<FactForDESKTOP>]
let ``type check neg51`` () = singleNegTest ( "typecheck/sigs") "neg51"

[<FactForDESKTOP>]
let ``type check neg52`` () = singleNegTest ( "typecheck/sigs") "neg52"

[<FactForDESKTOP>]
let ``type check neg53`` () = singleNegTest ( "typecheck/sigs") "neg53"

[<FactForDESKTOP>]
let ``type check neg54`` () = singleNegTest ( "typecheck/sigs") "neg54"

[<FactForDESKTOP>]
let ``type check neg55`` () = singleNegTest ( "typecheck/sigs") "neg55"

[<FactForDESKTOP>]
let ``type check neg56`` () = singleNegTest ( "typecheck/sigs") "neg56"

[<FactForDESKTOP(Skip = "Failing in new test framework")>]
let ``type check neg56_a`` () = singleNegTest ( "typecheck/sigs") "neg56_a"

[<FactForDESKTOP>]
let ``type check neg56_b`` () = singleNegTest ( "typecheck/sigs") "neg56_b"

[<FactForDESKTOP(Skip = "Failing in new test framework")>]
let ``type check neg57`` () = singleNegTest ( "typecheck/sigs") "neg57"

[<FactForDESKTOP(Skip = "Failing in new test framework")>]
let ``type check neg58`` () = singleNegTest ( "typecheck/sigs") "neg58"

[<FactForDESKTOP>]
let ``type check neg59`` () = singleNegTest ( "typecheck/sigs") "neg59"

[<FactForDESKTOP>]
let ``type check neg60`` () = singleNegTest ( "typecheck/sigs") "neg60"

[<FactForDESKTOP>]
let ``type check neg61`` () = singleNegTest ( "typecheck/sigs") "neg61"

[<FactForDESKTOP>]
let ``type check neg62`` () = singleNegTest ( "typecheck/sigs") "neg62"

[<FactForDESKTOP>]
let ``type check neg63`` () = singleNegTest ( "typecheck/sigs") "neg63"

[<FactForDESKTOP>]
let ``type check neg64`` () = singleNegTest ( "typecheck/sigs") "neg64"

[<FactForDESKTOP>]
let ``type check neg65`` () = singleNegTest ( "typecheck/sigs") "neg65"

[<FactForDESKTOP>]
let ``type check neg66`` () = singleNegTest ( "typecheck/sigs") "neg66"

[<FactForDESKTOP>]
let ``type check neg67`` () = singleNegTest ( "typecheck/sigs") "neg67"

[<FactForDESKTOP>]
let ``type check neg68`` () = singleNegTest ( "typecheck/sigs") "neg68"

[<FactForDESKTOP>]
let ``type check neg69`` () = singleNegTest ( "typecheck/sigs") "neg69"

[<FactForDESKTOP>]
let ``type check neg70`` () = singleNegTest ( "typecheck/sigs") "neg70"

[<FactForDESKTOP>]
let ``type check neg71`` () = singleNegTest ( "typecheck/sigs") "neg71"

[<FactForDESKTOP>]
let ``type check neg72`` () = singleNegTest ( "typecheck/sigs") "neg72"

[<FactForDESKTOP>]
let ``type check neg73`` () = singleNegTest ( "typecheck/sigs") "neg73"

[<FactForDESKTOP>]
let ``type check neg74`` () = singleNegTest ( "typecheck/sigs") "neg74"

[<FactForDESKTOP>]
let ``type check neg75`` () = singleNegTest ( "typecheck/sigs") "neg75"

[<FactForDESKTOP>]
let ``type check neg76`` () = singleNegTest ( "typecheck/sigs") "neg76"

[<FactForDESKTOP>]
let ``type check neg77`` () = singleNegTest ( "typecheck/sigs") "neg77"

[<FactForDESKTOP>]
let ``type check neg78`` () = singleNegTest ( "typecheck/sigs") "neg78"

[<FactForDESKTOP>]
let ``type check neg79`` () = singleNegTest ( "typecheck/sigs") "neg79"

[<FactForDESKTOP>]
let ``type check neg80`` () = singleNegTest ( "typecheck/sigs") "neg80"

[<FactForDESKTOP>]
let ``type check neg81`` () = singleNegTest ( "typecheck/sigs") "neg81"

[<FactForDESKTOP>]
let ``type check neg82`` () = singleNegTest ( "typecheck/sigs") "neg82"

[<FactForDESKTOP>]
let ``type check neg83`` () = singleNegTest ( "typecheck/sigs") "neg83"

[<FactForDESKTOP>]
let ``type check neg84`` () = singleNegTest ( "typecheck/sigs") "neg84"

[<FactForDESKTOP>]
let ``type check neg85`` () = singleNegTest ( "typecheck/sigs") "neg85"

[<FactForDESKTOP>]
let ``type check neg86`` () = singleNegTest ( "typecheck/sigs") "neg86"

[<FactForDESKTOP>]
let ``type check neg88`` () = singleNegTest ( "typecheck/sigs") "neg88"

[<FactForDESKTOP>]
let ``type check neg89`` () = singleNegTest ( "typecheck/sigs") "neg89"

[<FactForDESKTOP>]
let ``type check neg90`` () = singleNegTest ( "typecheck/sigs") "neg90"

[<FactForDESKTOP>]
let ``type check neg91`` () = singleNegTest ( "typecheck/sigs") "neg91"

[<FactForDESKTOP>]
let ``type check neg92`` () = singleNegTest ( "typecheck/sigs") "neg92"

[<FactForDESKTOP>]
let ``type check neg93`` () = singleNegTest ( "typecheck/sigs") "neg93"

[<FactForDESKTOP(Skip = "Failing in new test framework")>]
let ``type check neg94`` () = singleNegTest ( "typecheck/sigs") "neg94"

[<FactForDESKTOP>]
let ``type check neg95`` () = singleNegTest ( "typecheck/sigs") "neg95"

[<FactForDESKTOP>]
let ``type check neg96`` () = singleNegTest ( "typecheck/sigs") "neg96"

[<FactForDESKTOP>]
let ``type check neg97`` () = singleNegTest ( "typecheck/sigs") "neg97"

[<FactForDESKTOP>]
let ``type check neg98`` () = singleNegTest ( "typecheck/sigs") "neg98"

[<FactForDESKTOP>]
let ``type check neg99`` () = singleNegTest ( "typecheck/sigs") "neg99"

[<FactForDESKTOP(Skip = "Failing in new test framework")>]
let ``type check neg100`` () = singleVersionedNegTestAux "typecheck/sigs"  ["--warnon:3218" ] LangVersion.Latest "neg100"  

[<FactForDESKTOP>]
let ``type check neg101`` () = singleNegTest ( "typecheck/sigs") "neg101"

[<FactForDESKTOP>]
let ``type check neg102`` () = singleNegTest ( "typecheck/sigs") "neg102"

[<FactForDESKTOP>]
let ``type check neg103`` () = singleNegTest ( "typecheck/sigs") "neg103"

[<FactForDESKTOP>]
let ``type check neg104`` () = singleNegTest ( "typecheck/sigs") "neg104"

[<FactForDESKTOP>]
let ``type check neg106`` () = singleNegTest ( "typecheck/sigs") "neg106"

[<FactForDESKTOP(Skip = "Failing in new test framework")>]
let ``type check neg107`` () = singleNegTest ( "typecheck/sigs") "neg107"

[<FactForDESKTOP>]
let ``type check neg108`` () = singleNegTest ( "typecheck/sigs") "neg108"

[<FactForDESKTOP>]
let ``type check neg109`` () = singleNegTest ( "typecheck/sigs") "neg109"

[<FactForDESKTOP>]
let ``type check neg110`` () = singleNegTest ( "typecheck/sigs") "neg110"

[<FactForDESKTOP>]
let ``type check neg111`` () = singleNegTest ( "typecheck/sigs") "neg111"

[<FactForDESKTOP>] 
let ``type check neg112`` () = singleNegTest ( "typecheck/sigs") "neg112"
    
[<FactForDESKTOP>]
let ``type check neg113`` () = singleNegTest ( "typecheck/sigs") "neg113"

[<FactForDESKTOP>]
let ``type check neg114`` () = singleNegTest ( "typecheck/sigs") "neg114"

[<FactForDESKTOP>]
let ``type check neg115`` () = singleNegTest ( "typecheck/sigs") "neg115"

[<FactForDESKTOP(Skip = "Failing in new test framework")>]
let ``type check neg116`` () = singleNegTest ( "typecheck/sigs") "neg116"

[<FactForDESKTOP(Skip = "Failing in new test framework")>]
let ``type check neg117`` () = singleNegTest ( "typecheck/sigs") "neg117"

[<FactForDESKTOP>]
let ``type check neg118`` () = singleNegTest ( "typecheck/sigs") "neg118"

[<FactForDESKTOP>]
let ``type check neg119a`` () = singleVersionedNegTest ( "typecheck/sigs") LangVersion.V60 "neg119a"

[<FactForDESKTOP>]
let ``type check neg119b`` () = singleVersionedNegTest ( "typecheck/sigs") LangVersion.V70 "neg119b"

[<FactForDESKTOP>]
let ``type check neg120`` () = singleNegTest ( "typecheck/sigs") "neg120"

[<FactForDESKTOP>]
let ``type check neg121`` () = singleNegTest ( "typecheck/sigs") "neg121"

[<FactForDESKTOP>]
let ``type check neg122`` () = singleNegTest ( "typecheck/sigs") "neg122"

[<FactForDESKTOP>]
let ``type check neg123`` () = singleNegTest ( "typecheck/sigs") "neg123"

[<FactForDESKTOP>]
let ``type check neg124`` () = singleNegTest ( "typecheck/sigs") "neg124"

[<FactForDESKTOP>]
let ``type check neg125`` () = singleNegTest ( "typecheck/sigs") "neg125"

[<FactForDESKTOP>]
let ``type check neg126`` () = singleNegTest ( "typecheck/sigs") "neg126"

[<FactForDESKTOP>]
let ``type check neg127`` () = singleNegTest ( "typecheck/sigs") "neg127"

[<FactForDESKTOP>]
let ``type check neg128`` () = singleNegTest ( "typecheck/sigs") "neg128"

[<FactForDESKTOP>]
let ``type check neg129`` () = singleNegTest ( "typecheck/sigs") "neg129"

[<FactForDESKTOP>]
let ``type check neg130`` () = singleNegTest ( "typecheck/sigs") "neg130"

[<FactForDESKTOP>]
let ``type check neg131`` () = singleVersionedNegTest ( "typecheck/sigs") LangVersion.V60 "neg131"

[<FactForDESKTOP>]
let ``type check neg132`` () = singleVersionedNegTest ( "typecheck/sigs") LangVersion.V50 "neg132"
    
[<FactForDESKTOP>]
let ``type check neg133`` () = singleNegTest ( "typecheck/sigs") "neg133"

[<FactForDESKTOP>]
let ``type check neg_anon_1`` () = singleNegTest ( "typecheck/sigs") "neg_anon_1"

[<FactForDESKTOP>]
let ``type check neg_anon_2`` () = singleNegTest ( "typecheck/sigs") "neg_anon_2"

[<FactForDESKTOP>]
let ``type check neg_issue_3752`` () = singleNegTest ( "typecheck/sigs") "neg_issue_3752"

[<FactForDESKTOP>]
let ``type check neg_byref_1`` () = singleNegTest ( "typecheck/sigs") "neg_byref_1"

[<FactForDESKTOP>]
let ``type check neg_byref_2`` () = singleNegTest ( "typecheck/sigs") "neg_byref_2"

[<FactForDESKTOP>]
let ``type check neg_byref_3`` () = singleNegTest ( "typecheck/sigs") "neg_byref_3"

[<FactForDESKTOP>]
let ``type check neg_byref_4`` () = singleNegTest ( "typecheck/sigs") "neg_byref_4"

[<FactForDESKTOP>]
let ``type check neg_byref_5`` () = singleNegTest ( "typecheck/sigs") "neg_byref_5"

[<FactForDESKTOP>]
let ``type check neg_byref_6`` () = singleNegTest ( "typecheck/sigs") "neg_byref_6"

[<FactForDESKTOP>]
let ``type check neg_byref_7`` () = singleNegTest ( "typecheck/sigs") "neg_byref_7"

[<FactForDESKTOP>]
let ``type check neg_byref_8`` () = singleNegTest ( "typecheck/sigs") "neg_byref_8"

[<FactForDESKTOP>]
let ``type check neg_byref_10`` () = singleNegTest ( "typecheck/sigs") "neg_byref_10"

[<FactForDESKTOP>]
let ``type check neg_byref_11`` () = singleNegTest ( "typecheck/sigs") "neg_byref_11"

[<FactForDESKTOP>]
let ``type check neg_byref_12`` () = singleNegTest ( "typecheck/sigs") "neg_byref_12"

[<FactForDESKTOP>]
let ``type check neg_byref_13`` () = singleNegTest ( "typecheck/sigs") "neg_byref_13"

[<FactForDESKTOP>]
let ``type check neg_byref_14`` () = singleNegTest ( "typecheck/sigs") "neg_byref_14"

[<FactForDESKTOP>]
let ``type check neg_byref_15`` () = singleNegTest ( "typecheck/sigs") "neg_byref_15"

[<FactForDESKTOP>]
let ``type check neg_byref_16`` () = singleNegTest ( "typecheck/sigs") "neg_byref_16"

[<FactForDESKTOP>]
let ``type check neg_byref_17`` () = singleNegTest ( "typecheck/sigs") "neg_byref_17"

[<FactForDESKTOP>]
let ``type check neg_byref_18`` () = singleNegTest ( "typecheck/sigs") "neg_byref_18"

[<FactForDESKTOP>]
let ``type check neg_byref_19`` () = singleNegTest ( "typecheck/sigs") "neg_byref_19"

[<FactForDESKTOP>]
let ``type check neg_byref_20`` () = singleNegTest ( "typecheck/sigs") "neg_byref_20"

[<FactForDESKTOP>]
let ``type check neg_byref_21`` () = singleNegTest ( "typecheck/sigs") "neg_byref_21"

[<FactForDESKTOP>]
let ``type check neg_byref_22`` () = singleNegTest ( "typecheck/sigs") "neg_byref_22"

[<FactForDESKTOP>]
let ``type check neg_byref_23`` () = singleNegTest ( "typecheck/sigs") "neg_byref_23"