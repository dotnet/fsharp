// #NoMT #CompilerOptions 
(if System.IO.File.Exists(fsi.CommandLineArgs.[1]) then 0 else 1) |> exit
