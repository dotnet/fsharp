module Foo

type Context = { Name: string }

let veryLongFunctionNameWithATupleAsArgumentThatWillReallyUseALotOfSpaceInTheGeneratedSignatureFile
    (justAString: string,
     suuuuuuuuuuuuuuuperLoooooooooooooooooooooooooooooooooooooooooooooooooongIntegerNaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaame: int)
    (ctx: Context)
    : Context -> Context =
    id

type TType = obj

let SampleFunctionTupledAllBreakA
    (
        longLongLongArgName1: string,
        longLongLongArgName2: TType,
        longLongLongArgName3: TType,
        longLongLongArgName4: TType
    ) : TType list =
    []

let SampleFunctionCurriedAllBreaksA
    (longLongLongArgName1: string)
    (longLongLongArgName2: TType)
    (longLongLongArgName3: TType)
    (longLongLongArgName4: TType)
    : TType list =
    []

let SampleFunctionMixedA
    (longLongLongArgName1: string, longLongLongArgName2: string)
    (longLongLongArgName3: string, longLongLongArgName4: string, longLongLongArgName5: TType)
    (longLongLongArgName6: TType, longLongLongArgName7: TType)
    (longLongLongArgName8: TType, longLongLongArgName9: TType, longLongLongArgName10: TType)
    : TType list =
    []

type Meh =
    abstract member ResolveDependencies:
        scriptDirectory1: string *
        scriptDirectory2: string *
        scriptDirectory3: string *
        scriptDirectory4: string *
        scriptName: string *
        scriptExt: string *
        timeout: int ->
            obj

    abstract member SomethingElse:
        int ->
        string ->
        string ->
        System.DateTime ->
        System.Action ->
        int array ->
        obj ->
            System.Collections.Generic.Comparer<int>

type AlternativeMeh() =
    member this.ResolveDependencies
        (scriptDirectory: string, scriptName: string)
        (otherDirectory1: string, otherDirectory2: string, otherDirectory3: string, otherDirectory4: string)
        (nicerScriptName: string, scriptExt: string, timeout: int)
        : obj =
        null

let somethingTupledReturingSomethingTupled
    (
        a: int,
        b: int,
        c: int,
        d: int,
        e: int,
        f: int,
        g: int,
        h: int,
        j: int,
        k: int
    ) =
    (a, b, c)
