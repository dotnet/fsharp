open System
open System.Reflection
open MetadataSubstitutions

[<EntryPoint>]
let main _ =
    if Library.Value() < 2020 then failwith "Library was not executed"

#if STATIC_LINKED
    if typeof<Library>.Assembly <> Assembly.GetExecutingAssembly() then failwith "Library was not statically linked"
#endif

    for assembly in [ Assembly.GetExecutingAssembly(); typeof<Library>.Assembly ] |> List.distinct do
        let names = assembly.GetManifestResourceNames()
        if Array.contains "marker" names then failwith "User substitution was not applied"
        let hasMetadata =
            names |> Array.exists (fun name -> name.StartsWith("FSharpSignature", StringComparison.Ordinal) || name.StartsWith("FSharpOptimization", StringComparison.Ordinal))
#if KEEP_METADATA
        if not hasMetadata then failwith "Metadata stripping was not disabled"
#else
        if hasMetadata then failwith "F# metadata was not stripped"
#endif

    printfn "All tests passed"
    0
