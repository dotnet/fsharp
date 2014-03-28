namespace Microsoft.FSharp.Compatibility

[<RequireQualifiedAccess>]
module String = 
    let test_null arg = match arg with null -> raise (new System.ArgumentNullException("arg")) | _ -> ()
    let invalidArg arg msg = raise (new System.ArgumentException((msg:string),(arg:string)))        

    let get (str:string) i =
        test_null str
        try str.[i]
        with :? System.ArgumentException -> invalidArg "i" "index out of bounds" 

    let length (str:string) =
        test_null str
        str.Length

    let sub (s:string) (start:int) (len:int) =
        test_null s
        try s.Substring(start,len)
        with :? System.ArgumentException -> failwith "String.sub" 

    let compare (x:string) y = compare x y

    let fast_get (s:string) n =
        test_null s
        s.[n]

    let of_char (c:char) = System.Char.ToString(c)
    let make (n: int) (c: char) : string = new System.String(c, n)

    let indexNotFound() = raise (new System.Collections.Generic.KeyNotFoundException("An index for the character was not found in the string"))
    
    let index_from (s:string) (start:int) (c:char) =  
        test_null s
        try let r = s.IndexOf(c,start) in if r = -1 then indexNotFound() else r
        with :? System.ArgumentException -> invalidArg "start" "String.index_from" 
      
    let rindex_from (s:string) (start:int) (c:char) =
        test_null s
        try let r =  s.LastIndexOf(c,start) in if r = -1 then indexNotFound() else r
        with :? System.ArgumentException -> invalidArg "start" "String.rindex_from" 
      

    let index (s:string) (c:char) =
        test_null s
        index_from s 0 c

    let rindex (s:string) (c:char) =
        test_null s
        rindex_from s (length s - 1) c

    let contains_between (s:string) (start:int) (stop:int) (c:char) =
        test_null s
        try s.IndexOf(c,start,(stop-start+1)) <> -1
        with :? System.ArgumentException -> invalidArg "start" "String.contains_between" 

    let contains_from (s:string) (start:int) (c:char) =
        test_null s
        let stop = length s - 1 in 
        try s.IndexOf(c,start,(stop-start+1)) <> -1
        with :? System.ArgumentException -> invalidArg "start" "String.contains_from" 

    let rcontains_from (s:string) (stop:int) (c:char) =
        test_null s
        let start = 0 in
        try s.IndexOf(c,start,(stop-start+1)) <> -1
        with :? System.ArgumentException -> invalidArg "stop" "String.rcontains_from" 
      
    let contains (s:string) (c:char) = contains_from s 0 c

    let uppercase (s:string) =
        test_null s
#if FX_NO_TO_LOWER_INVARIANT
        s.ToUpper(System.Globalization.CultureInfo.InvariantCulture)
#else
        s.ToUpperInvariant()
#endif

    let lowercase (s:string) =
        test_null s
#if FX_NO_TO_LOWER_INVARIANT
        s.ToLower(System.Globalization.CultureInfo.InvariantCulture)
#else
        s.ToLowerInvariant()
#endif

    let capitalize (s:string) =
        test_null s
        if s.Length = 0 then "" 
        else String.concat "" [uppercase (sub s 0 1); sub s 1 (length s - 1)]

    let uncapitalize (s:string) =
        test_null s
        if s.Length = 0 then  ""
        else String.concat "" [lowercase (sub s 0 1); sub s 1 (length s - 1)]

#if FX_NO_STRING_SPLIT_OPTIONS
#else
    let split (c : char list) =
        let ca = Array.ofList c 
        fun (s:string) ->
            test_null s
            Array.toList(s.Split(ca, System.StringSplitOptions.RemoveEmptyEntries))
#endif

    let trim (c : char list) =
        let ca = Array.ofList c 
        fun (s:string) -> s.Trim(ca)
