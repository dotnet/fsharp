let failures = ref []

let report_failure (s : string) = 
    stderr.Write" NO: "
    stderr.WriteLine s
    failures.Value <- failures.Value @ [s]

let test (s : string) b = 
    stderr.Write(s)
    if b then stderr.WriteLine " OK"
    else report_failure (s)

let check s b1 b2 = test s (b1 = b2)

module Library =     

    let x5 = nonNull<String> "" // Should not give a Nullness warning
    check "ekjnceoiwe5" x5 ""
    let x6 = nonNull<String __withnull> "" // **Expected to give a Nullness warning, expected also to give a warning with nullness checking off
    check "ekjnceoiwe6" x6 ""
    let x7 = nonNull ""
    check "ekjnceoiwe7" x7 ""
    let _x7 : String = x7
    let x8 = nonNull<String[]> Array.empty
    check "ekjnceoiwe8" x8 [| |]
    let x9 = nonNull [| "" |]
    check "ekjnceoiwe9" x9 [| "" |]
    let x10 = nonNullV (Nullable(3))
    check "ekjnceoiwe10" x10 3
    let x11 = try nonNullV (Nullable<int>()) with :? System.NullReferenceException -> 10
    check "ekjnceoiwe11" x11 10
    let x12 = nullV<int>
    check "ekjnceoiwe12" x12 (Nullable())
    let x13 = nullV<int64>
    check "ekjnceoiwe13" x13 (Nullable())
    let x14 = withNullV 6L
    check "ekjnceoiwe14" x14 (Nullable(6L))
    let x15 : String __withnull = withNull x4
    check "ekjnceoiwe15" x15 ""
    let x15a : String __withnull = withNull ""
    check "ekjnceoiwe15a" x15a ""
    let x15b : String __withnull = withNull<String> x4
    check "ekjnceoiwe15b" x15b ""
    let x15c : String __withnull = withNull<String __withnull> x4 // **Expected to give a Nullness warning
    check "ekjnceoiwe15c" x15c ""
    let x16 : Nullable<int> = withNullV 3
    check "ekjnceoiwe16" x16 (Nullable(3))
    
    let y0 = isNull null // Should not give a Nullness warning (obj)
    check "ekjnceoiwey0" y0 true
    let y1 = isNull (null: obj __withnull) // Should not give a Nullness warning
    check "ekjnceoiwey1" y1 true
    let y1b = isNull (null: String __withnull) // Should not give a Nullness warning
    check "ekjnceoiwey1b" y1b true
    let y2 = isNull "" // **Expected to give a Nullness warning - type instantiation of a nullable type is non-nullable String
    check "ekjnceoiwey2" y2 false
    let y9 = isNull<String> "" // **Expected to give a Nullness warning - type instantiation of a nullable type is non-nullable String
    check "ekjnceoiwey9" y9 false
    let y10 = isNull<String __withnull> "" // Should not give a Nullness warning.
    check "ekjnceoiwey10" y10 false
    // Not yet allowed 
    //let f7 () : 'T __withnull when 'T : struct = null
    let f7b () : Nullable<'T> = nullV // BUG: Incorrectly gives a warning about System.ValueType with /test:AssumeNullOnImport

    let f0b line = 
        let add (s:String) = ()
        match line with 
        | null  -> ()
        | _ -> add (nonNull<String> line) // Exected to give a nullness warning

    let add (s:String) = ()
    let f0c line = 
        add (nonNull<String> "") // WRONG: should not give a nullness warning


