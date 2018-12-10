// #Conformance #Globalization 
#if TESTS_AS_APP
module Core_unicode
#endif

open System.IO
let failures = ref []

let reportFailure (s : string) = 
    stderr.Write" NO: "
    stderr.WriteLine s
    failures := !failures @ [s]

let test (s : string) b = 
    stderr.Write(s)
    if b then stderr.WriteLine " OK"
    else reportFailure (s)

(* TEST SUITE FOR UNICODE CHARS *)


#if !TESTS_AS_APP && !NETCOREAPP
let input_byte (x : System.IO.FileStream) = 
    let b = x.ReadByte() 
    if b = -1 then raise (System.IO.EndOfStreamException()) else b

let test2925 () = 
  printfn "test2925...";
  (* This writes a file in the standard utf8 encoding.  Probably needs to be adjusted if default encodings differ *)
  let os = new System.IO.StreamWriter(new FileStream("out1.txt", FileMode.Create), System.Text.Encoding.UTF8) in
  os.Write "\u00a9"; (* copyright *)
  os.Write "\u2260"; (* not equals *)
  os.Dispose();
  use is1 = System.IO.File.OpenRead "out1.txt" in 
  use is2 = System.IO.File.OpenRead "out.bsl" in 
  try 
    while true do 
      let c2 = input_byte is2 in 
      let c1 = try input_byte is1 with :? System.IO.EndOfStreamException -> reportFailure "end of file reached"; 0 in 
      if c1 <> c2 then reportFailure "test3732: file contents differ";
    done;
  with :? System.IO.EndOfStreamException -> 
    is1.Dispose();
    is2.Dispose();
    ()

let _ = test2925 ()

let test2925b () = 
  printfn "test2925b...";
  (* This writes a file in the standard utf8 encoding.  Probably needs to be adjusted if default encodings differ *)
  let os = new System.IO.StreamWriter(new FileStream("out1.txt", FileMode.Create), System.Text.Encoding.UTF8) in
  os.Write "\U000000a9"; (* copyright *)
  os.Write "\U00002260"; (* not equals *)
  os.Dispose();
  let is1 = System.IO.File.OpenRead "out1.txt" in 
  let is2 = System.IO.File.OpenRead "out.bsl" in 
  try 
    while true do 
      let c2 = input_byte is2 in 
      let c1 = try input_byte is1 with :? System.IO.EndOfStreamException -> reportFailure "end of file reached"; 0 in 
      if c1 <> c2 then reportFailure "test373289: file contents differ";
    done;
  with :? System.IO.EndOfStreamException -> 
    is1.Dispose();
    is2.Dispose();
    ()

let _ = test2925b ()

let test2926 () = 
  printfn "test2926...";
  (* This writes a file in the standard utf8 encoding.  Probably needs to be adjusted if default encodings differ *)
  let os = new System.IO.StreamWriter(new FileStream("out2.txt", FileMode.Create), System.Text.Encoding.UTF8) in
  let s = "©≠" in   (* <<<<<  UNICODE STRING for "\u00a9\u2260" HERE!!! *)
  os.Write s;
  Printf.printf "length s = %d\n" (String.length s);
  os.Dispose();
  let is1 = System.IO.File.OpenRead "out2.txt" in 
  let is2 = System.IO.File.OpenRead "out.bsl" in 
  try 
    while true do 
      let c2 = input_byte is2 in 
      let c1 = try input_byte is1 with :? System.IO.EndOfStreamException -> reportFailure "end of file reached"; 0 in 
      if c1 <> c2 then reportFailure (sprintf "test37d2392: file contents differ, got '%x', expected '%x'" c1 c2);
    done;
  with :? System.IO.EndOfStreamException -> 
    is1.Dispose();
    is2.Dispose();
    ()

let _ = test2926 ()


let test2927 () = 
  printfn "test2927...";
  (* This writes a unicode string to stdout using the encoding in System.Console.OutputEncoding.  *)
  let s = "©≠" in   (* <<<<<  UNICODE STRING for "\u00a9\u2260" HERE!!! *)
  stdout.WriteLine s

let _ = test2927 ()

let test2928 () = 
  printfn "test2928...";
  (* This writes a file in the default encoding. *)
  let os = new System.IO.StreamWriter(new FileStream("out3.txt", FileMode.Create), System.Text.Encoding.UTF8) in
  let s = "©≠" in   (* <<<<<  UNICODE STRING for "\u00a9\u2260" HERE!!! *)
  os.Write s;
  os.Dispose()



let _ = test2928 ()
#endif

let test2929 () = 
  printfn "test2929...";
  let s = "©≠" in   (* <<<<<  UNICODE STRING for "\u00a9\u2260" HERE!!! *)
  if (Printf.sprintf "%s" s <> s) then reportFailure "sprintf did not roundtrip (1)";
  if (Printf.sprintf "©≠" <> s) then reportFailure "sprintf did not roundtrip (2)"

let _ = test2929 ()


let ÄËÖÏÜâæçñõö = 3 + 4

let МНОПРСТУФХЦẀẁẂќ = 5 + 6

let αβΛΘΩΨΧΣδζȚŶǺ = 22/7

let π = 3.1415

#if TESTS_AS_APP
let RUN() = !failures
#else
let aa =
  match !failures with 
  | [] -> 
      stdout.WriteLine "Test Passed"
      System.IO.File.WriteAllText("test.ok","ok")
      exit 0
  | _ -> 
      stdout.WriteLine "Test Failed"
      exit 1
#endif

