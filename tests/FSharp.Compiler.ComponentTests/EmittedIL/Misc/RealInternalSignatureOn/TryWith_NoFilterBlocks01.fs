// #NoMono #NoMT #CodeGen #EmittedIL 
// Verify that the deprecated command line option "--no-generate-filter-blocks" is honored
try
  ()
 with
  | e when e.GetHashCode() = 0 -> ()
  | _ -> ()
