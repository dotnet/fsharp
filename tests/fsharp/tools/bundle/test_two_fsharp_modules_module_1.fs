// #Misc #Modules 
#indent "off"

module Test_two_fsharp_modules_module_1

let f () = 
  for i = 1 to 10 do 
    List.iter (fun s -> eprintf "%s" s) ["hello"; " "; "world"];
    eprintfn "%s" "."
  done


