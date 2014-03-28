// #NoMT #CompilerOptions 
// Simple tool to check the correctness of the compiler
// --platform option. The nice thing about this tool is
// that is uses reflection only, so it allows cross-compilation
// verification (since there is no need to run a binary that
// we would not be able to run (e.g. if we compile for x64 and
// we are on IA64).

#light

let SCFLAGS = 
               let tmp = System.Environment.GetEnvironmentVariable("SCFLAGS")
               if tmp = null then "" else tmp

let SOURCE = System.Environment.GetEnvironmentVariable("SOURCE")

let m = System.Text.RegularExpressions.Regex.Match(SCFLAGS, ".*--platform:([^ ]+) *$", System.Text.RegularExpressions.RegexOptions.IgnoreCase)

type Platform = 
    | P of System.Reflection.PortableExecutableKinds * System.Reflection.ImageFileMachine
    
// let fsiCommandLineArgs = [|"fsi.exe"; "--platform"; "x64"|]

let expectedValues = printfn "fsi.CommandLineArgs=%A" fsi.CommandLineArgs
                     printfn "SCFLAGS=%A" SCFLAGS

                     match (if (fsi.CommandLineArgs.Length>1) then fsi.CommandLineArgs.[1] else m.Groups.[1].Value.ToLower()) with
                     | "x86" -> P(System.Reflection.PortableExecutableKinds.ILOnly ||| System.Reflection.PortableExecutableKinds.Required32Bit, 
                                  System.Reflection.ImageFileMachine.I386)
                     | "itanium"  -> P(System.Reflection.PortableExecutableKinds.ILOnly ||| System.Reflection.PortableExecutableKinds.PE32Plus,
                                       System.Reflection.ImageFileMachine.IA64)
                     | "x64" -> P(System.Reflection.PortableExecutableKinds.ILOnly ||| System.Reflection.PortableExecutableKinds.PE32Plus,
                                  System.Reflection.ImageFileMachine.AMD64)
                     | "arm" -> P(System.Reflection.PortableExecutableKinds.ILOnly, System.Reflection.ImageFileMachine.ARM)
                     | "anycpu32bitpreferred" -> P(System.Reflection.PortableExecutableKinds.ILOnly ||| System.Reflection.PortableExecutableKinds.Preferred32Bit,
                                                   System.Reflection.ImageFileMachine.I386)
                     | _ -> P(System.Reflection.PortableExecutableKinds.ILOnly, System.Reflection.ImageFileMachine.I386)

let PE_name =
    // Parse SCFLAGS to find out the expected PE name
    let name = System.Text.RegularExpressions.Regex.Match(SCFLAGS, "--out:([^ ]+).*$", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
    
    if (name.Groups.[1].Value.Length = 0) then
          // Parse SCFLAGS to find out the expected target extension 
          let mm = System.Text.RegularExpressions.Regex.Match(SCFLAGS, "--target:([^ ]+).*$", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
          let PE_ext = match mm.Groups.[1].Value.ToLower() with
                       | "library" -> ".dll" 
                       | "module"  -> ".netmodule"
                       | _         -> ".exe"
          SOURCE.Replace(".fs", PE_ext)
    else
          name.Groups.[1].Value

let fullPath = System.IO.Path.Combine(System.Environment.CurrentDirectory,PE_name)
          
let assm = System.Reflection.Assembly.ReflectionOnlyLoadFrom(fullPath)
let mods = assm.GetModules(false);

if (mods.Length > 1) then failwithf "Bitness: Unexpected module count";

let actualValues = 
    let mutable pekind  = Unchecked.defaultof<System.Reflection.PortableExecutableKinds>
    let mutable machine = Unchecked.defaultof<System.Reflection.ImageFileMachine>

    mods.[0].GetPEKind(&pekind, &machine);
    P(pekind,machine)

if actualValues = expectedValues then
    printfn "PASS"
    0
else 
    printfn "FAIL"
    printfn "Actual  : %A" actualValues
    printfn "Expected: %A" expectedValues
    1
|> exit
