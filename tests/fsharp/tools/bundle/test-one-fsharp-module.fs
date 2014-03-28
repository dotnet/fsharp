// #Misc #Modules 
#indent "off"

module Test_one_fsharp_module

let _ = List.iter (fun s -> eprintf "%s" s) ["hello"; " "; "world"]
let _ = eprintfn "%s" "."
let _ = exit 0

