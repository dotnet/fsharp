module Microsoft.FSharp.Compatibility.OCaml.Buffer

open Microsoft.FSharp.Compatibility.OCaml
open Microsoft.FSharp.Compatibility.OCaml.Pervasives

#nowarn "62" // use of ocaml compat functions from Pervasives

type t = System.Text.StringBuilder

let create (n:int) = new System.Text.StringBuilder(n)
    
let contents (t:t) = t.ToString()
let length (t:t) = t.Length
let clear (t:t) = ignore (t.Remove(0,length t))
let reset (t:t) = ignore (t.Remove(0,length t))
let add_char (t:t) (c:char) = ignore (t.Append(c))
let add_string (t:t) (s:string) = ignore (t.Append(s))
let add_substring (t:t) (s:string) n m = ignore (t.Append(s.[n..n+m-1]))
let add_buffer (t:t) (t2:t) = ignore (t.Append(t2.ToString()))

#if FX_NO_ASCII_ENCODING
#else
let add_channel (t:t) (c:in_channel) n = 
  let b = Array.zeroCreate n  in 
  really_input c b 0 n;
  ignore (t.Append(System.Text.Encoding.ASCII.GetString(b, 0, n)))
#endif

let output_buffer (os:out_channel) (t:t) = 
  output_string os (t.ToString())
