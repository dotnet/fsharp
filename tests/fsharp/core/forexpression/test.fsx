#if TESTS_AS_APP
module Core_forexpression
#endif

let failures = ref false
let report_failure () = 
  stderr.WriteLine " NO"; failures := true
let test s b = stderr.Write(s:string);  if b then stderr.WriteLine " OK" else report_failure() 

open System
open System.Collections.Generic

let count       = 1000
let testString  = "19740531"
let testData    =
    [|
        for i in 0..count -> [| for inner in 0..i -> inner |]
    |]
let expectedArraySum        = 167167000 // Find an expression for this sum from count
let expectedRangeSum        = ((count + 1) * count) / 2
let expectedStringSum       = 30
let expectedWildCard        = count + 1

let getTestData (inner : int [] -> #seq<int>) (outer : #seq<int> [] -> #seq<'U>) =
    (testData |> Array.map inner) |> outer

// sumOverArray covers the case when the sequence type
// is an Array`1
let sumOverArray () =
    let inner xs = xs
    let outer xs = xs
    let testData : int [] [] = getTestData inner outer
    let mutable sum = 0
    for outer in testData do
        for inner in outer do
            sum <- sum + inner
    sum

// sum over seq covers the case when no optimizations can be
// found and F# has to rely on .GetEnumerator()
let sumOverSeq () =
    let inner = Array.toSeq
    let outer = Array.toSeq
    let testData : seq<seq<int>> = getTestData inner outer
    let mutable sum = 0
    for outer in testData do
        for inner in outer do
            sum <- sum + inner
    sum

// sum over seq covers a regression case when Dictionary<int,int>
// shouldn'be mistaken for an IList
let sumOverDictionary () =
    let inner xs =
        let d = Dictionary<int,int>()
        xs |> Array.iteri (fun i x -> d.Add(i,x))
        d
    let testData : Dictionary<int,int> [] = testData |> Array.map inner
    let mutable sum = 0
    for outer in testData do
        for inner in outer do
            sum <- sum + inner.Value
    sum

// sumOverResizeArray covers the case when the sequence type
// extends an IList`1
let sumOverResizeArray () =
    let inner (xs : int []) = ResizeArray<_>(xs)
    let outer (xs : #seq<ResizeArray<int>>) = ResizeArray<_>(xs)
    let testData : ResizeArray<ResizeArray<int>> = getTestData inner outer
    let mutable sum = 0
    for outer in testData do
        for inner in outer do
            sum <- sum + inner
    sum
// sumOverIList covers the case when the sequence type
// is IList`1
let sumOverIList () =
    let inner (xs : int []) = ResizeArray<_>(xs) :> IList<_>
    let outer (xs : #seq<IList<int>>) = ResizeArray<_>(xs) :> IList<_>
    let testData : IList<IList<int>> = getTestData inner outer
    let mutable sum = 0
    for outer in testData do
        for inner in outer do
            sum <- sum + inner
    sum

// sumOverList invokes the case when the sequence type
// is a 'T list
let sumOverList () =
    let inner = Array.toList
    let outer = Array.toList
    let testData : int list list = getTestData inner outer
    let mutable sum = 0
    for outer in testData do
        for inner in outer do
            sum <- sum + inner
    sum

// sumOverRange invokes the case when the sequence type is
// of the form n..m
let sumOverRange () =
    let mutable sum = 0
    for i in 0..count do
        sum <- sum + i
    sum

// sumOverString invokes the case when the sequence type is
// a string
let sumOverString () =
    let mutable sum = 0
    for i in testString do
        sum <- sum + ((int i) - (int '0'))
    sum

let arraySum                = sumOverArray ()
let seqSum                  = sumOverSeq ()
let dictionarySum           = sumOverDictionary ()
let ResizeArraySum          = sumOverResizeArray ()
let listSum                 = sumOverList ()
let ilistSum                = sumOverIList ()
let rangeSum                = sumOverRange ()
let stringSum               = sumOverString ()

do test "arraySum"          (expectedArraySum   = arraySum      )
do test "seqSum"            (expectedArraySum   = seqSum        )
do test "dictionarySum"     (expectedArraySum   = dictionarySum )
do test "ResizeArraySum"    (expectedArraySum   = ResizeArraySum)
do test "listSum"           (expectedArraySum   = listSum       )
do test "ilistSum"          (expectedArraySum   = ilistSum      )
do test "rangeSum"          (expectedRangeSum   = rangeSum      )
do test "stringSum"         (expectedStringSum  = stringSum     )

module RegressionCase =
    [<Struct>]
    type ColorF =
        val r: float32
        val g: float32
        val b: float32

        new (r, g, b) = {
            ColorF.r = r
            ColorF.g = g
            ColorF.b = b
        }

        static member (+) (first: ColorF, second: ColorF) =
            ColorF(first.r + second.r, first.g + second.g, first.b + second.b)

    let Issue12333 () =
        for x = 0 to 1 do
            printfn "iter %d" x
            let mutable color: ColorF = ColorF()
            printfn "init color %g %g %g" color.r color.g color.b
            test "lclwejcjwl1" (color.r = 0.0f)
            test "lclwejcjwl2" (color.g = 0.0f)
            test "lclwejcjwl3" (color.b = 0.0f)
            for s = 0 to 1 do
                let temp = ColorF(0.1f, 0.1f, 0.1f)
                color <- color + temp
                printfn "color %g %g %g" color.r color.g color.b


    Issue12333() // should run without exception

#if TESTS_AS_APP
let RUN() = !failures
#else
let aa =
    if !failures then stdout.WriteLine "Test Failed"; exit 1
    else stdout.WriteLine "Test Passed"; System.IO.File.WriteAllText("test.ok","ok"); exit 0
#endif
