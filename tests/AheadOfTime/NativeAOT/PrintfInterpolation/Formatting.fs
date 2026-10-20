module Formatting

open System
open System.Globalization

let check (expected: string) (actual: string) =
    if actual <> expected then
        raise (InvalidOperationException("Expected '" + expected + "', got '" + actual + "'"))

let timestamp (now: DateTime) =
    $"%02d{now.Hour}:%02d{now.Minute}:%02d{now.Second}"

let padded n = $"%02d{n}"
let inline paddedInteger (n: ^T) = $"%02d{n}"

type Rendered(trace: ResizeArray<int>) =
    override _.ToString() =
        trace.Add 11
        "x"

let runChecks () =
    let now = DateTime(2026, 10, 13, 1, 2, 3)
    check "01" $"%02d{now.Hour}"
    check "0102" $"%02d{now.Hour}%02d{now.Minute}"
    check "010203" $"%02d{now.Hour}%02d{now.Minute}%02d{now.Second}"
    check "01:02:03" (timestamp now)
    check "01:02:03" (PrintfInterpolationLibrary.timestamp now)
    let asObject: obj = $"""%t{fun () -> "ok"}%02d{1}"""
    let asComparable: IComparable = $"""%t{fun () -> "ok"}%02d{1}"""
    let asCloneable: ICloneable = $"""%t{fun () -> "ok"}%02d{1}"""
    for value in [asObject; asComparable; asCloneable] do
        check "ok01" (string value)
    for n, expected in
        [ 0, "00"; 9, "09"; 10, "10"; -1, "-1"; -9, "-9"; -10, "-10"
          Int32.MinValue, "-2147483648"; Int32.MaxValue, "2147483647" ] do
        check expected (padded n)
    check "01" (paddedInteger 1y)
    check "01" (paddedInteger 1uy)
    check "01" (paddedInteger 1s)
    check "01" (paddedInteger 1us)
    check "01" (paddedInteger 1L)
    check "01" (paddedInteger 1u)
    check "01" (paddedInteger 1UL)
    check "    1" $"%5d{1}"
    check "001" $"%03d{1}"
    check "-01" $"%03d{-1}"
    check "3.14" $"%.2f{3.14159}"
    check "true false" $"%b{true} %b{false}"
    check "4294967295" $"%u{-1}"

    let originalCulture = CultureInfo.CurrentCulture
    try
        CultureInfo.CurrentCulture <- CultureInfo("fr-FR")
        let n, text, initial, amount, flag = 2, "s", 'c', 1.5M, true
        let missing: string = null
        for actual in
            [ $"%02d{1}%d{n}|%d{n}%02d{1}%d{n}|%d{n}%02d{1}"
              $"%02d{1}%i{n}|%i{n}%02d{1}%i{n}|%i{n}%02d{1}"
              $"%02d{1}%s{string n}|%s{string n}%02d{1}%s{string n}|%s{string n}%02d{1}"
              $"%02d{1}%c{'2'}|%c{'2'}%02d{1}%c{'2'}|%c{'2'}%02d{1}"
              $"%02d{1}%M{2M}|%M{2M}%02d{1}%M{2M}|%M{2M}%02d{1}"
              $"%02d{1}{n}|{n}%02d{1}{n}|{n}%02d{1}" ] do
            check "012|2012|201" actual
        check "s01c" $"%s{text}%02d{1}%c{initial}"
        check "011.5" $"%02d{1}%M{amount}"
        check "1.501" $"%M{amount}%02d{1}"
        check "True01true" $"{flag}%02d{1}%b{flag}"
        check "01" $"%s{missing}%02d{1}{missing}"
        check "01:02:03" $"{now.Hour:D2}:{now.Minute:D2}:{now.Second:D2}"
        check "    42" $"{42,6}"
        check "011.50" $"%02d{1}{amount:F2}"
        check "1.5001" $"{1.5:F2}%02d{1}"
    finally
        CultureInfo.CurrentCulture <- originalCulture

    let trace = ResizeArray<int>()
    let hole id value =
        trace.Add id
        value
    check "01203" $"%02d{hole 1 1}{hole 2 2}%02d{hole 3 3}"
    if Seq.toList trace <> [ 1; 2; 3 ] then failwith "Hole evaluation order"
    trace.Clear()
    check "x02" $"{hole 1 (Rendered trace)}%02d{hole 2 2}"
    if Seq.toList trace <> [ 1; 11; 2 ] then failwith "Formatting moved past a later hole"
    trace.Clear()
    let throwing () =
        trace.Add 2
        raise (InvalidOperationException("hole"))
    try
        let _ = $"%02d{hole 1 1}%02d{throwing ()}%02d{hole 3 3}"
        failwith "Expected the second hole to throw"
    with
    | :? InvalidOperationException as error when error.Message = "hole" -> ()
    if Seq.toList trace <> [ 1; 2 ] then failwith "Evaluated a hole after an exception"
