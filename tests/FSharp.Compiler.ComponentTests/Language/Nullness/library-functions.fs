module LibraryFunctions

open System

let x4 : string = "plain string"
let x5 = nonNull<String> ("": string | null) // Should not give a Nullness warning  
let x6 = nonNull<String | null> "" // **Expected to give a Nullness warning   
let x7 = nonNull ""  
let _x7 : String = x7
let x8 = nonNull<String[]> Array.empty  
let x9 = nonNull [| "" |]   
let x10 = nonNullV (Nullable(3))   
let x11 = try nonNullV (Nullable<int>()) with :? System.NullReferenceException -> 10
let x12 = nullV<int>
let x13 = nullV<int64>
let x14 = withNullV 6L
let x15 : String | null = withNull x4 // This should not warn (is OK if typar is passed, this is wrong)
let x15a : String | null = withNull "" // This should not warn (is OK if typar is passed, this is wrong)
let x15b : String | null = withNull<String> x4
let x15c : String | null = withNull<String | null> x4 // **Expected to give a Nullness warning
let x16 : Nullable<int> = withNullV 3

let y0 = isNull null // Should not give a Nullness warning (obj)
let y1 = isNull (null: obj | null) // Should not give a Nullness warning
let y1b = isNull (null: String | null) // Should not give a Nullness warning
let y2 = isNull "" // **Expected to give a Nullness warning - type instantiation of a nullable type is non-nullable String
let y9 = isNull<String> "" // **Expected to give a Nullness warning - type instantiation of a nullable type is non-nullable String
let y10 = isNull<String | null> "" // Should not give a Nullness warning.
// Not yet allowed 
//let f7 () : 'T | null when 'T : struct = null
let f7b () : Nullable<'T> = nullV 

let f0b (line:string | null) = 
    let add (s:String) = ()
    match line with 
    | null  -> ()
    | _ -> add (line) // warning expected

let add (s:String) = ()
let f0c line = 
    add (nonNull<String> "")