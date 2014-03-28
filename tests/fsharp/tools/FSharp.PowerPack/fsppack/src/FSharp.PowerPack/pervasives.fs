module Microsoft.FSharp.Compatibility.OCaml.Pervasives

#nowarn "62" // compatibility warnings
#nowarn "35"  // 'deprecated' warning about redefining '<' etc.
#nowarn "86"  // 'deprecated' warning about redefining '<' etc.
#nowarn "60"  // override implementations in intrinsic extensions
#nowarn "69"  // interface implementations in intrinsic extensions

open System.Collections.Generic

exception Match_failure = Microsoft.FSharp.Core.MatchFailureException
exception Assert_failure of string * int * int 

exception Undefined 

exception End_of_file      = System.IO.EndOfStreamException
exception Out_of_memory    = System.OutOfMemoryException
exception Division_by_zero = System.DivideByZeroException
exception Stack_overflow   = System.StackOverflowException 

let Not_found<'a> = (new KeyNotFoundException("The item was not found during a search or in a collection") :> exn)
let (|Not_found|_|) (inp:exn) = match inp with :? KeyNotFoundException -> Some() | _ -> None

let Invalid_argument (msg:string) = (new System.ArgumentException(msg) :> exn)
let (|Invalid_argument|_|) (inp:exn) = match inp with :? System.ArgumentException as e -> Some(e.Message) | _ -> None

let invalid_arg s = raise (System.ArgumentException(s))

let not_found() = raise Not_found

let inline (==)    (x:'a) (y:'a) = LanguagePrimitives.PhysicalEquality x y
let inline (!=)    (x:'a) (y:'a) = not (LanguagePrimitives.PhysicalEquality x y)
let inline (mod)  (x:int) (y:int)  = Operators.(%) x y
let inline (land) (x:int) (y:int)  = Operators.(&&&) x y
let inline (lor)  (x:int) (y:int)  = Operators.(|||) x y
let inline (lxor) (x:int) (y:int)  = Operators.(^^^) x y
let inline lnot   (x:int)          = Operators.(~~~) x
let inline (lsl)  (x:int) (y:int)  = Operators.(<<<) x y
let inline (lsr)  (x:int) (y:int)  = int32 (uint32 x >>> y)
let inline (asr)  (x:int) (y:int)  = Operators.(>>>) x y

let int_neg (x:int) = -x
let (~-.)  (x:float)           =  -x
let (~+.)  (x:float)           =  x
let (+.)   (x:float) (y:float) =  x+y
let (-.)   (x:float) (y:float) =  x-y
let ( *.)  (x:float) (y:float) =  x*y
let ( /.)  (x:float) (y:float) =  x/y

let inline (.())   (arr: _[]) n = arr.[n]
let inline (.()<-) (arr: _[]) n x = arr.[n] <- x

let succ (x:int) = x + 1
let pred (x:int) = x - 1
let max_int = 0x7FFFFFFF // 2147483647 
let min_int = 0x80000000 // -2147483648 

(*  mod_float x y = x - y * q where q = truncate(a/b) and truncate x removes fractional part of x *)
let truncate (x:float) : int = int32 x

#if FX_NO_TRUNCATE
let truncatef (x:float) = 
    if x >= 0.0 then 
        System.Math.Floor x
    else
        System.Math.Ceiling x
#else
let truncatef (x:float) = System.Math.Truncate x
#endif
let mod_float x y = x - y * truncatef(x/y)
let float_of_int (x:int) =  float x
let ldexp x (n:int) = x * (2.0 ** float n)
let modf x = let integral = Operators.floor x in (integral, x - integral)
let int_of_float x =  truncate x

let neg_infinity = System.Double.NegativeInfinity
let max_float    = System.Double.MaxValue 
let min_float    =  0x0010000000000000LF
let epsilon_float = 0x3CB0000000000000LF // Int64.float_of_bits 4372995238176751616L

type fpclass = FP_normal (* | FP_subnormal *)  | FP_zero| FP_infinite | FP_nan      

let classify_float (x:float) = 
    if System.Double.IsNaN x then FP_nan
    elif System.Double.IsNegativeInfinity x then FP_infinite
    elif System.Double.IsPositiveInfinity x then FP_infinite
    elif x = 0.0 then FP_zero
    else FP_normal
       
let abs_float (x:float)           = Operators.abs x

let int_of_char (c:char) = System.Convert.ToInt32(c)
let char_of_int (x:int) = System.Convert.ToChar(x)

let string_of_bool b = if b then "true" else "false"
let bool_of_string s = 
  match s with 
  | "true" -> true 
  | "false" -> false
  | _ -> raise (System.ArgumentException("bool_of_string"))

let string_of_int (x:int) = x.ToString()
let int_of_string (s:string) = try int32 s with _ -> failwith "int_of_string"
let string_of_float (x:float) = x.ToString()
let float_of_string (s:string) = try float s with _ -> failwith "float_of_string"

let string_to_int   x = int_of_string x


//--------------------------------------------------------------------------
// I/O
//
// OCaml-compatible channels conflate binary and text IO. It is very inconvenient to introduce
// out_channel as a new abstract type, as this means functions like fprintf can't be used in 
// conjunction with TextWriter values.  Hence we pretend that OCaml channels are TextWriters, and 
// implement TextWriters in such a way the the implementation contains an optional binary stream
// which is utilized by the OCaml binary I/O methods.
//
// Notes on the implementation: We discriminate between three kinds of 
// readers/writers since various operations are possible on each kind.
// StreamReaders/StreamWriters inherit from text readers/writers and
// thus support more functionality.  We could just support two 
// constructors (Binary and Text) and use dynamic type tests on the underlying .NET
// objects to detect the cases where we have StreamWriters.
//--------------------------------------------------------------------------
open System.Text
open System.IO

type writer = 
  | StreamW of StreamWriter
  | TextW of (unit -> TextWriter)
  | BinaryW of BinaryWriter


[<assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Scope="member", Target="Internal.Utilities.Pervasives+OutChannelImpl.#.ctor(Internal.Utilities.Pervasives+writer)", MessageId="System.IO.TextWriter.#ctor")>]
do()

let defaultEncoding =
#if FX_NO_DEFAULT_ENCODING
        // default encoding on Silverlight is UTF8 (to aling with e.g. System.IO.StreamReader)
        Encoding.UTF8
#else
        Encoding.Default
#endif

type out_channel = TextWriter
type OutChannelImpl(w: writer) = 
    inherit TextWriter()
    let mutable writer = w
    member x.Writer with get() = writer and set(w) = writer <- w
    
    member x.Stream = 
        match writer with 
        | TextW _ -> failwith "cannot access a stream for this channel"
        | BinaryW bw -> bw.BaseStream 
        | StreamW sw -> sw.BaseStream
    member x.TextWriter = 
        match writer with 
        | StreamW sw -> (sw :> TextWriter)
        | TextW tw -> tw()
        | _ -> failwith "binary channels created using the OCaml-compatible Pervasives.open_out_bin cannot be used as TextWriters.  Consider using 'System.IO.BinaryWriter' in preference to creating channels using open_out_bin."
        
    member x.StreamWriter = 
        match writer with 
        | StreamW sw -> sw
        | _ -> failwith "cannot access a stream writer for this channel"
    member x.BinaryWriter = 
        match writer with 
        | BinaryW w -> w
        | _ -> failwith "cannot access a binary writer for this channel"

type open_flag =
  | Open_rdonly | Open_wronly | Open_append
  | Open_creat | Open_trunc | Open_excl
  | Open_binary | Open_text 
#if FX_NO_NONBLOCK_IO
#else
  | Open_nonblock
#endif
  | Open_encoding of Encoding

type reader = 
  | StreamR of StreamReader
  | TextR of (unit -> TextReader)
  | BinaryR of BinaryReader

/// See OutChannelImpl
type in_channel = System.IO.TextReader
type InChannelImpl(r : reader) = 
    inherit TextReader()
    let mutable reader = r
    member x.Reader with get() = reader and set(r) = reader <- r
    
    member x.Stream =
        match reader with 
        | TextR _ -> failwith "cannot access a stream for this channel"
        | BinaryR bw -> bw.BaseStream 
        | StreamR sw -> sw.BaseStream
    member x.TextReader = 
        match reader with 
        | StreamR sw -> (sw :> TextReader)
        | TextR tw -> tw()
        | _ -> failwith "binary channels created using the OCaml-compatible Pervasives.open_in_bin cannot be used as TextReaders  If necessary use the OCaml compatible binary input methods Pervasvies.input etc. to read from this channel. Consider using 'System.IO.BinaryReader' in preference to channels created using open_in_bin."
        
    member x.StreamReader = 
        match reader with 
        | StreamR sw -> sw
        | _ -> failwith "cannot access a stream writer for this channel"
    member x.BinaryReader = 
        match reader with 
        | BinaryR w -> w
        | _ -> failwith "cannot access a binary writer for this channel"

let (!!) (os : out_channel) = 
    match os with 
    | :? OutChannelImpl as os -> os.Writer
    | :? StreamWriter as sw -> StreamW sw
    | _ -> TextW (fun () -> os)
let (<--) (os: out_channel) os' = 
    match os with 
    | :? OutChannelImpl as os -> os.Writer <- os'
    | _ -> failwith "the mode may not be adjusted on a writer not created with one of the Pervasives.open_* functions"
    
let stream_to_BinaryWriter s    = BinaryW (new BinaryWriter(s))
let stream_to_StreamWriter (encoding : Encoding) (s : Stream) =   StreamW (new StreamWriter(s,encoding))

module OutChannel = 
    let to_Stream (os:out_channel) =
      match !!os with 
      | BinaryW bw -> bw.BaseStream 
      | StreamW sw -> sw.BaseStream
      | TextW _ -> failwith "to_Stream: cannot access a stream for this channel"
      
    let to_StreamWriter (os:out_channel) =
      match !!os with 
      | StreamW sw -> sw
      | _ -> failwith "to_StreamWriter: cannot access a stream writer for this channel"
      
    let to_TextWriter (os:out_channel) =
      match !!os with 
      | StreamW sw -> (sw :> TextWriter)
      | TextW tw -> tw()
      | _ -> os
      
    let of_StreamWriter w =
      new OutChannelImpl(StreamW(w)) :> out_channel

    let to_BinaryWriter (os:out_channel) =
      match !!os with 
      | BinaryW bw -> bw
      | _ -> failwith "to_BinaryWriter: cannot access a binary writer for this channel"
      
    let of_BinaryWriter w =
      new OutChannelImpl(BinaryW w) :> out_channel
      
    let of_TextWriter (w:TextWriter) =
      let absw = 
        match w with 
        | :? StreamWriter as sw -> StreamW sw
        | tw -> TextW (fun () -> tw) in
      new OutChannelImpl(absw) :> out_channel
        
    let of_Stream encoding (s : Stream) =   new OutChannelImpl(stream_to_StreamWriter encoding s) :> out_channel
      
let listContains x l = List.exists (fun y -> x = y) l

let open_out_gen flags (_perm:int) (s:string) = 
    // permissions are ignored 
    let app = listContains Open_append flags in 
    let access = 
        match listContains Open_rdonly flags, listContains Open_wronly flags with
        | true, true -> invalidArg "flags" "invalid access for reading"
        | true, false -> invalidArg "flags" "invalid access for writing" // FileAccess.Read 
        | false, true ->  FileAccess.Write
        | false, false -> (if app then FileAccess.Write else FileAccess.ReadWrite)  
    let mode =
        match (listContains Open_excl flags,app, listContains Open_creat flags, listContains Open_trunc flags) with
        | (true,false,false,false) -> FileMode.CreateNew
        | false,false,true,false -> FileMode.Create
        | false,false,false,false -> FileMode.OpenOrCreate
        | false,false,false,true -> FileMode.Truncate
        | false,false,true,true -> FileMode.OpenOrCreate
        | false,true,false,false -> FileMode.Append
        | _ -> invalidArg "flags" "invalid mode" 
    let share = FileShare.Read 
    let bufferSize = 0x1000 
#if FX_NO_NONBLOCK_IO
    let stream = (new FileStream(s,mode,access,share,bufferSize)) 
#else
    let allowAsync = listContains Open_nonblock flags 
    let stream = (new FileStream(s,mode,access,share,bufferSize,allowAsync)) 
#endif
    match listContains Open_binary flags, listContains Open_text flags with 
    | true,true -> invalidArg "flags" "mixed text/binary flags"
    | true,false -> (new OutChannelImpl(stream_to_BinaryWriter stream ) :> out_channel)
    | false,_ ->
        let encoding = List.tryPick (function Open_encoding e -> Some(e) | _ -> None) flags 
        let encoding = match encoding with None -> defaultEncoding | Some e -> e 
        OutChannel.of_Stream encoding (stream :> Stream)
        
let open_out (s:string) = open_out_gen [Open_text; Open_wronly; Open_creat] 777 s

// NOTE: equiv to
//       new BinaryWriter(new FileStream(s,FileMode.OpenOrCreate,FileAccess.Write,FileShare.Read ,0x1000,false)) 
let open_out_bin (s:string) = open_out_gen [Open_binary; Open_wronly; Open_creat] 777 s


let flush (os:out_channel) = 
    match !!os with 
    | TextW tw   -> (tw()).Flush() // the default method does not flush, is it overriden for the console? 
    | BinaryW bw -> bw.Flush()
    | StreamW sw -> sw.Flush()

let close_out (os:out_channel) = 
    match !!os with 
    | TextW tw -> (tw()).Close()
    | BinaryW bw -> bw.Close()
    | StreamW sw -> sw.Close()

let prim_output_newline (os:out_channel) = 
    match !!os with 
    | TextW tw -> (tw()).WriteLine()
    | BinaryW _ -> invalidArg "os" "the channel is a binary channel"
    | StreamW sw -> sw.WriteLine()

let output_string (os:out_channel) (s:string) = 
    match !!os with 
    | TextW tw -> (tw()).Write(s)
    | BinaryW bw -> 
         // Write using a char array - writing a string writes it length-prefixed! 
         bw.Write (Array.init s.Length (fun i -> s.[i]) )
    | StreamW sw -> sw.Write(s)

let prim_output_int (os:out_channel) (s:int) = 
    match !!os with 
    | TextW tw -> (tw()).Write(s)
    | BinaryW _ -> invalidArg "os" "the channel is a binary channel"
    | StreamW sw -> sw.Write(s)

let prim_output_float (os:out_channel) (s:float) = 
    match !!os with 
    | TextW tw -> (tw()).Write(s)
    | BinaryW _ -> invalidArg "os" "the channel is a binary channel"
    | StreamW sw -> sw.Write(s)

let output_char (os:out_channel) (c:char) = 
    match !!os with 
    | TextW tw -> 
        (tw()).Write(c)
    | BinaryW bw -> 
        if int c > 255 then invalidArg "c" "unicode characters of value > 255 may not be written to binary channels"
        bw.Write(byte c)
    | StreamW sw -> 
        sw.Write(c)

let output_chars (os:out_channel) (c:char[]) start len  = 
    match !!os with 
    | TextW tw -> (tw()).Write(c,start,len)
    | BinaryW bw -> bw.Write(c,start,len)
    | StreamW sw -> sw.Write(c,start,len)

let seek_out (os:out_channel) (n:int) = 
    match !!os with 
    | StreamW sw -> 
        sw.Flush();   
        (OutChannel.to_Stream os).Seek(int64 n,SeekOrigin.Begin) |> ignore
    | TextW _ ->
        (OutChannel.to_Stream os).Seek(int64 n,SeekOrigin.Begin) |> ignore
    | BinaryW bw -> 
        bw.Flush();
        bw.Seek(n,SeekOrigin.Begin) |> ignore

let pos_out (os:out_channel) = flush os; int32 ((OutChannel.to_Stream os).Position)
let out_channel_length (os:out_channel) = flush os; int32 ((OutChannel.to_Stream os).Length)

let output (os:out_channel) (buf: byte[]) (x:int) (len: int) = 
    match !!os with 
    | BinaryW bw -> bw.Write(buf,x,len)
    | TextW _ | StreamW _ -> 
        output_string os (defaultEncoding.GetString(buf,x,len))

let output_byte (os:out_channel) (x:int) = 
    match !!os with 
    | BinaryW bw -> bw.Write(byte (x % 256))
    | TextW _  | StreamW _ -> output_char os (char (x % 256))

let output_binary_int (os:out_channel) (x:int) = 
    match !!os with 
    | BinaryW bw -> bw.Write x
    | _ -> failwith "output_binary_int: not a binary stream"

let set_binary_mode_out (os:out_channel) b = 
    match !!os with 
    | StreamW _ when not b -> ()
    | BinaryW _ when b -> ()
    | BinaryW bw -> 
            os <--  stream_to_StreamWriter defaultEncoding (OutChannel.to_Stream os)
    | StreamW bw -> os <-- stream_to_BinaryWriter (OutChannel.to_Stream os)
    | TextW _ when b -> failwith "cannot set this stream to binary mode"
    | TextW _ -> ()

let print_int (x:int)        = prim_output_int stdout x
let print_float (x:float)    = prim_output_float stdout x
let print_string (x:string)  = output_string stdout x
let print_newline ()         = prim_output_newline stdout
let print_endline (x:string) = print_string x; print_newline ()
let print_char (c:char)      = output_char stdout c

let prerr_int (x:int)        = prim_output_int stderr x
let prerr_float (x:float)    = prim_output_float stderr x
let prerr_string (x:string)  = output_string stderr x
let prerr_newline ()         = prim_output_newline stderr
let prerr_endline (x:string) = prerr_string x; prerr_newline ()
let prerr_char (c:char)      = output_char stderr c

#if FX_NO_BINARY_SERIALIZATION
#else
let output_value (os:out_channel) (x: 'a) = 
    let formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter() 
    formatter.Serialize(OutChannel.to_Stream os,box [x]);
    flush os
#endif

let (!!!) (c : in_channel) = 
    match c with 
    | :? InChannelImpl as c -> c.Reader
    | :? StreamReader as sr -> StreamR sr
    | _ -> TextR (fun () -> c)
let (<---) (c: in_channel) r = 
    match c with 
    | :? InChannelImpl as c -> c.Reader<- r
    | _ -> failwith "the mode may only be adjusted channels created with one of the Pervasives.open_* functions"

let mk_BinaryReader (s: Stream) = BinaryR (new BinaryReader(s))
let mk_StreamReader e (s: Stream) = StreamR (new StreamReader(s, e,false))
module InChannel = 

    let of_Stream (e:Encoding) (s: Stream) =   new InChannelImpl(mk_StreamReader e s) :> in_channel
    let of_StreamReader w =
      new InChannelImpl (StreamR w) :> in_channel

    let of_BinaryReader r =
      new InChannelImpl (BinaryR r) :> in_channel
      
    let of_TextReader (r: TextReader) =
      let absr = 
        match r with 
        | :? StreamReader as sr -> StreamR sr
        | tr -> TextR (fun () -> tr) 
      new InChannelImpl(absr) :> in_channel

    let to_StreamReader (c:in_channel) =
      match !!!c with 
      | StreamR sr -> sr
      | _ -> failwith "to_StreamReader: cannot access a stream reader for this channel"
      
    let to_BinaryReader (is:in_channel) =
      match !!!is with 
      | BinaryR sr -> sr
      | _ -> failwith "to_BinaryReader: cannot access a binary reader for this channel"
      
    let to_TextReader (is:in_channel) =
      match !!!is with 
      | TextR tr ->tr()
      | _ -> is
      
    let to_Stream (is:in_channel) =
      match !!!is with 
      | BinaryR bw -> bw.BaseStream
      | StreamR sw -> sw.BaseStream
      | _ -> failwith "cannot seek, set position or calculate length of this stream"

// permissions are ignored 
let open_in_gen flags (_perm:int) (s:string) = 
    let access = 
      match listContains Open_rdonly flags, listContains Open_wronly flags with
      | true, true -> invalidArg "flags" "invalid access"
      | true, false -> FileAccess.Read 
      | false, true -> invalidArg "flags" "invalid access for reading"
      | false, false -> FileAccess.ReadWrite 
    let mode = 
      match listContains Open_excl flags,listContains Open_append flags, listContains Open_creat flags, listContains Open_trunc flags with
      | false,false,false,false -> FileMode.Open
      | _ -> invalidArg "flags" "invalid mode for reading" 
    let share = FileShare.Read 
    let bufferSize = 0x1000 
#if FX_NO_NONBLOCK_IO
    let stream = new FileStream(s,mode,access,share,bufferSize) :> Stream 
#else
    let allowAsync = listContains Open_nonblock flags 
    let stream = new FileStream(s,mode,access,share,bufferSize,allowAsync) :> Stream 
#endif
    match listContains Open_binary flags, listContains Open_text flags with 
    | true,true -> invalidArg "flags" "mixed text/binary flags specified"
    | true,false -> new InChannelImpl (mk_BinaryReader stream ) :> in_channel
    | false,_ ->
        let encoding = List.tryPick (function Open_encoding e -> Some(e) | _ -> None) flags 
        let encoding = match encoding with None -> defaultEncoding | Some e -> e 
        InChannel.of_Stream encoding stream
  
let open_in_encoded (e:Encoding) (s:string) = open_in_gen [Open_text; Open_rdonly; Open_encoding e] 777 s
let open_in (s:string) = open_in_gen [Open_text; Open_rdonly] 777 s

// NOTE: equivalent to
//     new BinaryReader(new FileStream(s,FileMode.Open,FileAccess.Read,FileShare.Read,0x1000,false))
let open_in_bin (s:string) = open_in_gen [Open_binary; Open_rdonly] 777 s

let close_in (is:in_channel) = 
  match !!!is with 
  | TextR tw -> (tw()).Close()
  | BinaryR bw -> bw.Close()
  | StreamR sw -> sw.Close()

let input_line (is:in_channel) = 
    match !!!is with 
    | BinaryR _ -> failwith "input_line: binary mode"
    | TextR tw -> match tw().ReadLine() with null -> raise End_of_file | res -> res
    | StreamR sw -> match sw.ReadLine() with null -> raise End_of_file | res -> res

let input_byte (is:in_channel) = 
    match !!!is with 
    | BinaryR bw ->  int (bw.ReadByte())
    | TextR tr -> let b = (tr()).Read() in if b = -1 then raise End_of_file else int (byte b)
    | StreamR sr -> let b = sr.Read() in if b = -1 then raise End_of_file else int (byte b)

let prim_peek (is:in_channel) = 
    match !!!is with 
    | BinaryR bw ->  bw.PeekChar()
    | TextR tr -> (tr()).Peek()
    | StreamR sr -> sr.Peek()

let prim_input_char (is:in_channel) = 
    match !!!is with 
    | BinaryR bw ->  (try int(bw.ReadByte()) with End_of_file -> -1)
    | TextR tr -> (tr()).Read() 
    | StreamR sr -> sr.Read()

let input_char (is:in_channel) = 
    match !!!is with 
    | BinaryR _ ->  char_of_int (input_byte is)
    | TextR tr -> let b = (tr()).Read() in if b = -1 then raise End_of_file else (char b)
    | StreamR sr -> let b = sr.Read() in if b = -1 then raise End_of_file else (char b)

let input_chars (is:in_channel) (buf:char[]) start len = 
    match !!!is with 
    | BinaryR bw ->  bw.Read(buf,start,len)
    | TextR trf -> let tr = trf() in tr.Read(buf,start,len) 
    | StreamR sr -> sr.Read(buf,start,len) 

let seek_in (is:in_channel) (n:int) = 
    begin match !!!is with 
    | StreamR sw -> sw.DiscardBufferedData() 
    | TextR _ | BinaryR _ -> ()
    end;
    ignore ((InChannel.to_Stream is).Seek(int64 n,SeekOrigin.Begin))

let pos_in (is:in_channel)  = int32 ((InChannel.to_Stream is).Position)
let in_channel_length (is:in_channel)  = int32 ((InChannel.to_Stream is).Length)

let input_bytes_from_TextReader (tr : TextReader) (enc : Encoding) (buf: byte[]) (x:int) (len: int) = 
    /// Don't read too many characters!
    let lenc = (len * 99) / enc.GetMaxByteCount(100) 
    let charbuf : char[] = Array.zeroCreate lenc 
    let nRead = tr.Read(charbuf,x,lenc) 
    let count = enc.GetBytes(charbuf,x,nRead,buf,x) 
    count

let input (is: in_channel) (buf: byte[]) (x:int) (len: int) = 
    match !!!is with 
    | StreamR _  ->  (InChannel.to_Stream is).Read(buf,x,len)
    | TextR trf   -> 
        input_bytes_from_TextReader (trf()) defaultEncoding buf x len  
    | BinaryR br -> br.Read(buf,x,len)

let really_input (is:in_channel) (buf: byte[]) (x:int) (len: int) = 
    let mutable n = 0 
    let mutable i = 1 
    while (if i > 0 then n < len else false) do 
        i <- input is buf (x+n) (len-n);
        n <- n + i
    
let unsafe_really_input (is:in_channel) buf x len = really_input is buf x len

let input_binary_int (is:in_channel) = 
    match !!!is with 
    | BinaryR bw -> bw.ReadInt32()
    | _ -> failwith "input_binary_int: not a binary stream"

let set_binary_mode_in (is:in_channel) b = 
    match !!!is with 
    | StreamR _ when not b -> ()
    | BinaryR _ when b -> ()
    | BinaryR bw -> is <---  mk_StreamReader defaultEncoding (InChannel.to_Stream is)
    | StreamR bw -> is <--- mk_BinaryReader (InChannel.to_Stream is)
    | TextR _ when b -> failwith "set_binary_mode_in: cannot set this stream to binary mode"
    | TextR _ -> ()

let read_line ()  = stdout.Flush(); input_line stdin
let read_int ()   = int_of_string (read_line())
let read_float () = float_of_string (read_line ())

#if FX_NO_BINARY_SERIALIZATION
#else
let input_value (is:in_channel) = 
    let formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter() 
    let res = formatter.Deserialize(InChannel.to_Stream is) 
    match ((unbox res) : 'a list) with 
    | [x] -> x
    | _ -> failwith "input_value: expected one item"
#endif

type InChannelImpl with 
    override x.Dispose(deep:bool) = if deep then close_in (x :> in_channel)
    override x.Peek() = prim_peek (x :> in_channel) 
    override x.Read() = prim_input_char (x :> in_channel) 
    override x.Read((buffer:char[]),(index:int),(count:int)) = input_chars (x :> in_channel) buffer index count
    

type OutChannelImpl with 
    override x.Dispose(deep:bool) = if deep then close_out (x :> out_channel)
    override x.Encoding = x.TextWriter.Encoding
    override x.FormatProvider = x.TextWriter.FormatProvider
    override x.NewLine = x.TextWriter.NewLine
    override x.Write(s:string) = output_string (x :> out_channel) s
    override x.Write(c:char) = output_char (x :> out_channel) c
    override x.Write(c:char[]) = output_chars (x :> out_channel) c 0 c.Length
    override x.Write((c:char[]),(index:int),(count:int)) = output_chars (x :> out_channel) c index count
    
type ('a,'b,'c,'d) format4 = Microsoft.FSharp.Core.Format<'a,'b,'c,'d>
type ('a,'b,'c) format = Microsoft.FSharp.Core.Format<'a,'b,'c,'c>

exception Exit


module Pervasives = 

    let hash  (x: 'a) = LanguagePrimitives.GenericHash x

#if FX_NO_EXIT
#else
    let exit (n:int) = Operators.exit n
#endif

    let incr x = x.contents <- x.contents + 1
    let decr x = x.contents <- x.contents - 1

    let (@) l1 l2 = l1 @ l2
    // NOTE: inline to call site since LanguagePrimitives.<funs> have static type optimisation 
    let (=)     (x:'a) (y:'a) = Operators.(=) x y
    let (<>)    (x:'a) (y:'a) = Operators.(<>) x y 
    let (<)     (x:'a) (y:'a) = Operators.(<) x y
    let (>)     (x:'a) (y:'a) = Operators.(>) x y
    let (<=)    (x:'a) (y:'a) = Operators.(<=) x y
    let (>=)    (x:'a) (y:'a) = Operators.(>=) x y
    let min     (x:'a) (y:'a) = Operators.min x y
    let max     (x:'a) (y:'a) = Operators.max x y
    let compare (x:'a) (y:'a) = LanguagePrimitives.GenericComparison x y

    let (~+) x = Operators.(~+) x
    //  inline (~-) x = LanguagePrimitives.(~-) x 

    let (+) (x:int) (y:int)   = Operators.(+) x y
    let (-) (x:int) (y:int)   = Operators.(-) x y
    let ( * ) (x:int) (y:int) = Operators.( * ) x y
    let (/) (x:int) (y:int)   = Operators.(/) x y
    let not (b:bool) = Operators.not b
    type 'a ref = Microsoft.FSharp.Core.Ref<'a>
    type 'a option = Microsoft.FSharp.Core.Option<'a>
    type 'a list = Microsoft.FSharp.Collections.List<'a>
    type exn = System.Exception
    let raise (e:exn) = Operators.raise e
    let failwith s = raise (Failure s)
    let fst (x,_y) = x
    let snd (_x,y) = y

    let ref x = { contents=x }
    let (!) x = x.contents
    let (:=) x y = x.contents <- y

    let float (x:int) =  Operators.float x
    let float32 (n:int) =  Operators.float32 n
    let abs (x:int) = Operators.abs x
    let ignore _x = ()
    let invalid_arg s = raise (System.ArgumentException(s))
    let (^) (x:string) (y:string) = System.String.Concat(x,y)
    let sqrt      (x:float)           = Operators.sqrt x
    let exp       (x:float)           = Operators.exp x
    let log       (x:float)           = Operators.log x
    let log10     (x:float)           = Operators.log10 x
    let cos       (x:float)           = Operators.cos x
    let sin       (x:float)           = Operators.sin x
    let tan       (x:float)           = Operators.tan x
    let acos      (x:float)           = Operators.acos x
    let asin      (x:float)           = Operators.asin x
    let atan      (x:float)           = Operators.atan x
    let atan2     (x:float) (y:float) = Operators.atan2 x y
    let cosh      (x:float)           = Operators.cosh x
    let sinh      (x:float)           = Operators.sinh x
    let tanh      (x:float)           = Operators.tanh x
    let ceil      (x:float)           = Operators.ceil x
    let floor     (x:float)           = Operators.floor x

    let ( ** ) (x:float) (y:float) = Operators.( ** ) x y
    let truncate (x:float) = Operators.int32 x
    let nan          = System.Double.NaN 
    let infinity     = System.Double.PositiveInfinity
