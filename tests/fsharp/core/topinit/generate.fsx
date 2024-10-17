
let generateTests() = 
    let n = ref 0 
    let commandLine = ref "fsc " 
    let lines = 
        [| yield "let checkNotInitialized s isInitialized = if isInitialized then (printf \"FAILED: %s, expected module not to be initialized\" s; exit 1)";
           yield "let checkInitialized s isInitialized = if not isInitialized then (printf \"FAILED: %s, expected module to be initialized\" s; exit 1)";
                                               
           for (decl1, triggers1) in 
                                      [ "let x = System.DayOfWeek.Friday", false;
                                        "let x = 1.0", false;
                                        "let x = 1.0f", false;
                                        "let x = 1.0M", false;
                                        "let x = 1", false;
                                        "let x = 1y", false;
                                        "let x = 1uy", false;
                                        "let x = 1s", false;
                                        "let x = 1us", false;
                                        "let x = 1n", false;
                                        "let x = 1un", false;
                                        "let x = 1L", false;
                                        "let x = c", false;
                                        "let x = \"two\"", false;
                                        "let x = 1UL", false;
                                        "let x = enum<System.DayOfWeek>(0) ", false;
                                        "let x = 1 + 1", false;
                                        "let x = 1 < 1", false;
                                        "let x = 1 > 1", false;
                                        "let x = 1 <= 1", false;
                                        "let x = 1 >= 1", false;
                                        "let x = not b", false;
                                        "let x = 1 <> 1", false;
                                        "let x = compare 1 1", false;
                                        "let x = 1 - 1", false;
                                        "let x = 1 * 1", false;
                                        "let x = 2 <<< 2", false;
                                        "let x = 2 >>> 2", false;
                                        "let x = 2 ||| 2", false;
                                        "let x = 2 &&& 2", false;
                                        "let x = 2 ^^^ 2", false;
                                        "let x = ~~~ 2", false;
                                        "let x = 2", false;
                                        "let x = true && true", false;
                                        "let x = true && false", false;
                                        "let x = false && false", false;
                                        "let x = b && false", false;
                                        "let x : int list = []", false;
                                        "let x : (int * int) list = []", false;
                                        "let x : int option = None", false;
                                        "let x : (int * int) option = None", false;
                                        "let x = if true then 1 else 2", false;
                                        "let x = if b then 1 else 2", false;
                                        "let x = match true with true -> 1 | false -> 2", false;
                                        "let x = match b with true -> 1 | false -> 2", false;
                                        "let x = match b with v -> 1", false;
                                        "let x = match 1 with 1 -> 1 | _ -> 2", false;
                                        "let x = let a = 1 in a", false;
                                        "let x = if b then true else false", false;
                                        "let x = (fun () -> ())", false;
                                        "let x = (fun a -> a)", false;
                                        "let x = typeof<int>", false;
                                        "let x = [[]]", false;  // this is generalized
                                        "let x = Unchecked.defaultof<int>", false;
                                        "let x = Unchecked.defaultof<string>", false;
                                        "let x = Unchecked.defaultof<System.Windows.Forms.Form>", false;
                                        "let x = sizeof<int>", false;
                                        // These DO cause initialization
                                        "let x = \"two\" + \"three\"", true;
                                        "let x = \"two\" < \"three\"", true;
                                        "let x = \"two\" > \"three\"", true;
                                        "let x = \"two\" = \"three\"", true;
                                        "let x = \"two\" >= \"three\"", true;
                                        "let x = \"two\" <= \"three\"", true;
                                        "let x = \"two\" <> \"three\"", true;
                                        "let x = 1.0M + 2.0M", true;
                                        "let x = 1.0M - 2.0M", true;
                                        "let x = 1.0M * 2.0M", true;
                                        "let x = lazy (1+1)", true;
                                        "let x = 1.0M < 2.0M", true;
                                        "let x = 1.0M = 2.0M", true;
                                        "let x = 1.0M > 2.0M", true;
                                        "let x = 1.0M <> 2.0M", true;
                                        "let x = 1.0M >= 2.0M", true;
                                        "let x = 1.0M <= 2.0M", true;
                                        "let x = 1I", true;
                                        "let x = typedefof<int>", true;
                                        "let x = 1 / 1", true;
                                        "let x = 2 % 2", true;
                                        "let mutable x = 1", true;
                                        "let (x,_) = (1,2)", true;
                                        "let x = (1,2)", true;
                                        "let x = Some 1", true;
                                        "let x = [1]", true;
                                        "let x : Choice<int,string> = Choice1Of2 3", true;
                                        "let x = aFunction", true;
                                        "let x = ((); 1)", true; ] do
                  
                  incr n
                  commandLine :=  commandLine.Value + (sprintf " flag_deterministic_init%d.fs" n.Value)
                  commandLine :=  commandLine.Value + (sprintf " lib_deterministic_init%d.fs" n.Value)
                  System.IO.File.WriteAllLines(sprintf "flag_deterministic_init%d.fs" n.Value, 
                                               [| "// This file was autogenerated by running the script in this directory"
                                                  sprintf "module InitFlag%d" n.Value
                                                  "let mutable init = false"; |])
                  System.IO.File.WriteAllLines(sprintf "lib_deterministic_init%d.fs" n.Value, 
                                               [| yield "// This file was autogenerated by running the script in this directory"
                                                  yield sprintf "module Lib%d" n.Value
                                                  yield "// These are some constant expressions which can be accessed from context"
                                                  yield "let c = 1";
                                                  yield "let b = true";
                                                  yield "let aFunction (x:int) = x ";
                                                  yield "// This is a value we will access from the outside."
                                                  if triggers1 then 
                                                      yield "// We expect accessing the value 'x' to trigger initialization of this file"
                                                  else
                                                      yield "// We expect accessing the value 'x' will _not_ trigger initialization of this module"
                                                  yield decl1;
                                                  yield "// This is a value we can access from the outside to definitely force initialization of the module"
                                                  yield "let mutable forceInit = 1";
                                                  yield "// This sets a value in another module to indicate that initialization has happened"
                                                  yield sprintf "InitFlag%d.init <- true" n.Value |])
                  yield sprintf "//-----------------"
                  yield sprintf "printfn \"Touching value in module Lib%d...\"" n.Value 
                  yield sprintf "printfn \"    --> Lib%d.x = %%A\" Lib%d.x" n.Value n.Value
                  if triggers1 then 
                      yield sprintf "printfn \"Checking this did cause initialization of module Lib%d...\"" n.Value 
                      yield sprintf "checkInitialized \"Lib%d\" InitFlag%d.init" n.Value n.Value 
                  else 
                      yield sprintf "printfn \"Checking this did not cause initialization of module Lib%d...\"" n.Value 
                      yield sprintf "printfn \"Touching a mutable value in module Lib%d...\"" n.Value 
                      yield sprintf "checkNotInitialized \"Lib%d\" InitFlag%d.init" n.Value n.Value 
                      // Touching the 'init' value should trigger initialization
                      yield sprintf "printfn \"Lib%d.forceInit = %%A\" Lib%d.forceInit" n.Value n.Value
                      yield sprintf "checkInitialized \"Lib%d\" InitFlag%d.init" n.Value n.Value
           yield "System.IO.File.WriteAllText(\"test.ok\",\"ok\")"
           yield "exit 0" |]
    System.IO.File.WriteAllLines("test_deterministic_init.fs", lines)
    commandLine :=  commandLine.Value + " test_deterministic_init.fs"
    System.IO.File.WriteAllLines("commandLine.bat", [| commandLine.Value |])
                                               
generateTests()

