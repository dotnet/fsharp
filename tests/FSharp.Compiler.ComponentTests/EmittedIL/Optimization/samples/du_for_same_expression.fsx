// <testmetadata>
// { "optimization": { "reported_in": "#15872", "reported_by": "@kerams", "last_know_version_not_optimizing": "8", "first_known_version_optimizing": null } }
// </testmetadata>
let inline map mapping result =
    match result with
    | Error e -> Error e
    | Ok x -> Ok (mapping x)

let ff x = map (fun y -> y.ToString ()) x

let ffs x = map id x

System.Console.WriteLine(ff (Ok ""))
System.Console.WriteLine(ffs (Ok ""))
System.Console.WriteLine(ff (Error ""))
System.Console.WriteLine(ffs (Error ""))