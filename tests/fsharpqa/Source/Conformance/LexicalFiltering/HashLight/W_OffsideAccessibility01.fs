// #Regression #Conformance #LexFilter 
// FSB 2150, Offside rule for #light code should set offside to left of accessibility modifier if present

//<Expects status="warning" span="(18,5-18,8)" id="FS0058">Possible incorrect indentation: this token is offside of context started at position \(17:5\)\. Try indenting this token further or using standard formatting conventions\.$</Expects>
//<Expects status="warning" span="(19,5-19,8)" id="FS0058">Possible incorrect indentation: this token is offside of context started at position \(17:5\)\. Try indenting this token further or using standard formatting conventions\.$</Expects>
//<Expects status="warning" span="(20,5-20,6)" id="FS0058">Possible incorrect indentation: this token is offside of context started at position \(17:5\)\. Try indenting this token further or using standard formatting conventions\.$</Expects>

open System
open System.IO

(* CodeFile *)
type CodeFile =
    
    val m_code  : string
    val m_lines : string[]
    
    public new(filename : string) =
    let allCode = ""        // File.ReadAllText(filename)
    let lines   = [| |]     // Array.concat [ [|"[Dummy line which take lines being 1-indexed into account]"|]; (File.ReadAllLines(filename)) ]
    { m_code = allCode; m_lines = lines}

// Code should compile successfully
exit 0
