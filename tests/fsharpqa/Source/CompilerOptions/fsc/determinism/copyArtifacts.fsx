// Copy all dummy files (except the source) to a dummy2 version so we can compare them
System.IO.Directory.EnumerateFiles(__SOURCE_DIRECTORY__, "dummy.*") 
|> Seq.filter (fun x -> x.EndsWith(".fs") |> not)
|> Seq.iter (fun filename -> System.IO.File.Copy(filename, filename.Replace("dummy", "dummy2"), true))

// pause a second at the end to deal with the potential race condiction of
// too quick compiles back to back ending up in same exe, even when non-deterministic
System.Threading.Thread.Sleep(1000)
