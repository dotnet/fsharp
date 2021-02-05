// (c) Microsoft Corporation 2005-2009. 

#if INTERNALIZED_FSLEXYACC_RUNTIME
namespace Internal.Utilities
#else
namespace Microsoft.FSharp.Text
#endif


type ArgType = 
  | ClearArg of bool ref
  | FloatArg of (float -> unit)
  | IntArg of (int -> unit)
  | RestArg of (string -> unit)
  | SetArg of bool ref
  | StringArg of (string -> unit)
  | UnitArg of (unit -> unit)
  static member Clear  r = ClearArg r
  static member Float  r = FloatArg r
  static member Int    r = IntArg r
  static member Rest   r = RestArg r
  static member Set    r = SetArg r
  static member String r = StringArg r
  static member Unit   r = UnitArg r


type ArgInfo (name,action,help) = 
  member x.Name = name
  member x.ArgType = action
  member x.HelpText = help
  
exception Bad of string
exception HelpText of string

[<Sealed>]
type ArgParser() = 
    static let getUsage specs u =  
      let sbuf = new System.Text.StringBuilder 100  
      let pstring (s:string) = sbuf.Append s |> ignore 
      let pendline s = pstring s; pstring "\n" 
      pendline u;
      List.iter (fun (arg:ArgInfo) -> 
        match arg.Name, arg.ArgType, arg.HelpText with
        | (s, (UnitArg _ | SetArg _ | ClearArg _), helpText) -> pstring "\t"; pstring s; pstring ": "; pendline helpText
        | (s, StringArg _, helpText) -> pstring "\t"; pstring s; pstring " <string>: "; pendline helpText
        | (s, IntArg _, helpText) -> pstring "\t"; pstring s; pstring " <int>: "; pendline helpText
        | (s, FloatArg _, helpText) ->  pstring "\t"; pstring s; pstring " <float>: "; pendline helpText
        | (s, RestArg _, helpText) -> pstring "\t"; pstring s; pstring " ...: "; pendline helpText)
        specs;
      pstring "\t"; pstring "--help"; pstring ": "; pendline "display this list of options";
      pstring "\t"; pstring "-help"; pstring ": "; pendline "display this list of options";
      sbuf.ToString()


    static member ParsePartial(cursor,argv,arguments:seq<ArgInfo>,?otherArgs,?usageText) =
        let otherArgs = defaultArg otherArgs (fun _ -> ())
        let usageText = defaultArg usageText ""
        let nargs = Array.length argv 
        incr cursor;
        let arguments = arguments |> Seq.toList
        let specs = arguments |> List.map (fun (arg:ArgInfo) -> arg.Name, arg.ArgType)
        while !cursor < nargs do
          let arg = argv.[!cursor] 
          let rec findMatchingArg args = 
            match args with
            | ((s, action) :: _) when s = arg -> 
               let getSecondArg () = 
                   if !cursor + 1 >= nargs then 
                     raise(Bad("option "+s+" needs an argument.\n"+getUsage arguments usageText));
                   argv.[!cursor+1] 
                 
               match action with 
               | UnitArg f -> 
                 f (); 
                 incr cursor
               | SetArg f ->
                 f := true; 
                 incr cursor
               | ClearArg f -> 
                 f := false; 
                 incr cursor
               | StringArg f-> 
                 let arg2 = getSecondArg() 
                 f arg2; 
                 cursor := !cursor + 2
               | IntArg f -> 
                 let arg2 = getSecondArg () 
                 let arg2 = try int32 arg2 with _ -> raise(Bad(getUsage arguments usageText)) in  
                 f arg2;
                 cursor := !cursor + 2;
               | FloatArg f -> 
                 let arg2 = getSecondArg() 
                 let arg2 = try float arg2 with _ -> raise(Bad(getUsage arguments usageText)) in 
                 f arg2; 
                 cursor := !cursor + 2;
               | RestArg f -> 
                 incr cursor;
                 while !cursor < nargs do
                     f (argv.[!cursor]);
                     incr cursor;

            | (_ :: more)  -> findMatchingArg more 
            | [] -> 
                if arg = "-help" || arg = "--help" || arg = "/help" || arg = "/help" || arg = "/?" then
                    raise (HelpText (getUsage arguments usageText))
                // Note: for '/abc/def' does not count as an argument
                // Note: '/abc' does
                elif arg.Length>0 && (arg.[0] = '-' || (arg.[0] = '/' && not (arg.Length > 1 && arg.[1..].Contains ("/")))) then
                    raise (Bad ("unrecognized argument: "+ arg + "\n" + getUsage arguments usageText))
                else 
                   otherArgs arg;
                   incr cursor
          findMatchingArg specs 

    static member Usage (arguments,?usage) = 
        let usage = defaultArg usage ""
        System.Console.Error.WriteLine (getUsage (Seq.toList arguments) usage)

    #if FX_NO_COMMAND_LINE_ARGS
    #else
    static member Parse (arguments,?otherArgs,?usageText) = 
        let current = ref 0
        let argv = System.Environment.GetCommandLineArgs() 
        try ArgParser.ParsePartial (current, argv, arguments, ?otherArgs=otherArgs, ?usageText=usageText)
        with 
          | Bad h 
          | HelpText h -> 
              System.Console.Error.WriteLine h; 
              System.Console.Error.Flush();  
              System.Environment.Exit(1); 
          | e -> 
              reraise()
    #endif
