// #NoMono #NoMT #CodeGen #EmittedIL
// Verify the compiler generates SEH filter blocks when --exnfilters+ is specified.
try
  ()
 with
  | e when e.GetHashCode() = 0 -> ()
  | _ -> ()
