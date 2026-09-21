module Program

open System
open System.IO
open System.Reflection
open System.Resources

[<EntryPoint>]
let main _ =
    let assembly = Assembly.GetExecutingAssembly()
    let name = assembly.GetName().Name
    let marker = assembly.GetCustomAttributes<AssemblyMetadataAttribute>() |> Seq.exactlyOne
    if marker.Key <> "ProjectMarker" || marker.Value <> name then failwith "Assembly metadata leaked"
    let names = assembly.GetManifestResourceNames()
    let values =
        [ for resource in names do
              if resource.EndsWith(".resources", StringComparison.Ordinal) then
                  use reader = new ResourceReader(assembly.GetManifestResourceStream resource)
                  for entry in reader |> Seq.cast<System.Collections.DictionaryEntry> do
                      match entry.Value with
                      | :? string as value -> yield value
                      | _ -> () ]
    for prefix in [ "RESX_"; "TEXT_" ] do
        if not (List.contains (prefix + name) values) then failwithf "Missing %s%s: %A" prefix name names
    if values |> List.exists (fun value -> not (value.EndsWith(name, StringComparison.Ordinal))) then
        failwithf "Foreign resource marker: %A" values
    use dependent = new StreamReader(assembly.GetManifestResourceStream(name + ".dependent.txt"))
    if dependent.ReadToEnd() <> "DEPENDENT_" + name then failwith "Dependent resource mismatch"
    printfn "PASS %s resources=%d" name names.Length
    0
