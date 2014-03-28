#if INTERNALIZED_POWER_PACK
module internal Internal.Utilities.Arg
#else
module Microsoft.FSharp.Compatibility.Arg
#endif


type spec =
  | Clear of (bool ref)       
  | Float of (float -> unit)
  | Int of (int -> unit)    
  | Rest of (string -> unit)
  | Set of (bool ref)
  | String of (string -> unit)
  | Unit of (unit -> unit)  

let getUsage specs u =  
  let sbuf = new System.Text.StringBuilder 100  
  let pstring (s:string) = sbuf.Append s |> ignore 
  let pendline s = pstring s; pstring "\n" 
  pendline u;
  List.iter (function 
    | (s, (Unit _ | Set _ | Clear _), helpText) -> pstring "\t"; pstring s; pstring ": "; pendline helpText
    | (s, String _, helpText) -> pstring "\t"; pstring s; pstring " <string>: "; pendline helpText
    | (s, Int _, helpText) -> pstring "\t"; pstring s; pstring " <int>: "; pendline helpText
    | (s, Float _, helpText) ->  pstring "\t"; pstring s; pstring " <float>: "; pendline helpText
    | (s, Rest _, helpText) -> pstring "\t"; pstring s; pstring " ...: "; pendline helpText)
    specs;
  pstring "\t"; pstring "--help"; pstring ": "; pendline "display this list of options";
  pstring "\t"; pstring "-help"; pstring ": "; pendline "display this list of options";
  sbuf.ToString()

let incr r = r := !r + 1
let usage specs u = System.Console.Error.WriteLine (getUsage specs u)
type argspec = (string * spec * string) 

exception Help of string
exception Bad of string

let parse_argv cursor argv specs other usageText =
  let nargs = Array.length argv in 
  incr cursor;
  while !cursor < nargs do
    let arg = argv.[!cursor] in 
    let rec findMatchingArg (l: argspec  list) = 
      match l with 
      | ((s, action, _) :: _) when s = arg -> 
         let getSecondArg () = 
           if !cursor + 1 >= nargs then 
             raise(Bad("option "+s+" needs an argument.\n"+getUsage specs usageText));
           argv.[!cursor+1] 
           
         match action with 
         | Unit f -> 
           f (); 
           incr cursor
         | Set f ->
           f := true; 
           incr cursor
         | Clear f -> 
           f := false; 
           incr cursor
         | String f-> 
           let arg2 = getSecondArg() in 
           f arg2; 
           cursor := !cursor + 2
         | Int f -> 
           let arg2 = getSecondArg () in 
           let arg2 = try int32 arg2 with _ -> raise(Bad(getUsage specs usageText)) in  
           f arg2;
           cursor := !cursor + 2;
         | Float f -> 
           let arg2 = getSecondArg() in 
           let arg2 = try float arg2 with _ -> raise(Bad(getUsage specs usageText)) in 
           f arg2; 
           cursor := !cursor + 2;
         | Rest f -> 
           incr cursor;
           while !cursor < nargs do
               f (argv.[!cursor]);
               incr cursor;

      | (_ :: more)  -> findMatchingArg more 
      | [] -> 
          if arg = "-help" || arg = "--help" then
              raise (Help (getUsage specs usageText))
          elif arg.Length>0 && arg.[0] = '-' then
              raise (Bad ("unrecognized argument: "+ arg + "\n" + getUsage specs usageText))
          else 
             other arg;
             incr cursor
    findMatchingArg specs 
  done

#if FX_NO_COMMAND_LINE_ARGS
#else
let current = ref 0
let parse specs other usage = 
  let argv = System.Environment.GetCommandLineArgs() in 
  try parse_argv current argv specs other usage
  with 
    | Bad h 
    | Help h -> 
        System.Console.Error.WriteLine h; 
        System.Console.Error.Flush();  
        System.Environment.Exit(1); 
    | e -> 
        reraise()
#endif
