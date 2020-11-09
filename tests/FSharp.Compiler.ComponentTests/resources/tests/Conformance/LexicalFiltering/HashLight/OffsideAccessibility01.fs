// #Regression #Conformance #LexFilter 
#light

// FSB 2150, Offside rule for #light code should set offside to left of accessibility modifier if present

open System
open System.IO

(* CodeFile *)
type CodeFile =
    
    val m_code  : string
    val m_lines : string[]
    
    public new(filename : string) =
        let allCode = ""        // File.ReadAllText(filename)
        let lines   = [| "" |]  // Array.concat [ [|"[Dummy line which take lines being 1-indexed into account]"|]; (File.ReadAllLines(filename)) ]
        { m_code = allCode; m_lines = lines}

// Code should compile successfully
exit 0
