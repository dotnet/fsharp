// #Conformance #TypesAndModules #Records 
#light

// Verify ability to clone a record with mutable fields

[<Struct>] type MutableRec = { mutable A : int; mutable B : string }

let mutable t = { A = 1; B = "1" }
if t.A <> 1 || t.B <> "1" then exit 1

// Verify ability to mutate record fields
t.A <- 2
t.B <- "2"
if t.A <> 2 || t.B <> "2" then exit 1

// Make a copy
let mutable copy = { t with A = -1 }
if copy.A <> -1 || copy.B <> "2" then exit 1

// Mutate the copy, verify it does NOT affect origional
copy.A <- -3
copy.B <- "-3"

if t.A <> 2 || t.B <> "2" then exit 1
if copy.A <> -3 || copy.B <> "-3" then exit 1

exit 0


