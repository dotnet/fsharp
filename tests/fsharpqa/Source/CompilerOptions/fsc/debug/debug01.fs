// #Regression #NoMT #CompilerOptions #NoMono 
// Regression test for FSHARP1.0:5080
// Verify that the assembly contains the full path to the .pdb file (we are compiling with --debug+)
//<Expects status="success"></Expects>

/// Search a sequence of char (the string 's') in a binary file (the 'assemblyFullPath')
let f (assemblyFullPath:string, s:string) = 

            printfn "Searching '%s' in '%s'" s assemblyFullPath

            /// Make an array out of the string (will be used later to compare fragments of the file)
            let expectedStringAsArray = seq { for i in s -> int i } |> Seq.toArray
            
            /// Open binary file
            use assemblyStream = new System.IO.StreamReader( assemblyFullPath )
            
            /// Makes sliding windows out of the sequence of bytes that make up the binary file
            let z = seq { while not assemblyStream.EndOfStream do yield assemblyStream.Read() } |> Seq.windowed expectedStringAsArray.Length
            
            /// Try to find a matching sequence
            let p = z |> Seq.tryFindIndex (fun t -> (Seq.toArray t) = expectedStringAsArray)
            
            /// Dump the result
            p.IsSome
                        
/// Fully qualified path to ourselves 
let assemblyFullPath = System.Reflection.Assembly.GetExecutingAssembly().Location

/// Fully qualified path to pdb - we expect this info to be in the binary!
let pdbFullPath = System.IO.Path.ChangeExtension(assemblyFullPath, "pdb")

if f(assemblyFullPath, pdbFullPath) then
                                         printfn "OK: .pdb file found in assembly"
                                         exit 0
                                    else
                                         printfn "ERROR: No .pdb file found in assembly"
                                         exit 1
