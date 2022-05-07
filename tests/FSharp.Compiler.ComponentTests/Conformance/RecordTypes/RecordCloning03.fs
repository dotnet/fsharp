// #Conformance #TypesAndModules #Records 
#light

// Verify ability to clone a record with mutable fields

type MutableRec = { mutable A : int; mutable B : string }

let t = { A = 1; B = "1" }
if t.A <> 1 || t.B <> "1" then failwith "Failed"

// Verify ability to mutate record fields
t.A <- 2
t.B <- "2"
if t.A <> 2 || t.B <> "2" then failwith "Failed"

// Make a copy
let copy = { t with A = -1 }
if copy.A <> -1 || copy.B <> "2" then failwith "Failed"

// Mutate the copy, verify it does NOT affect origional
copy.A <- -3
copy.B <- "-3"

if t.A <> 2 || t.B <> "2" then failwith "Failed"
if copy.A <> -3 || copy.B <> "-3" then failwith "Failed"
