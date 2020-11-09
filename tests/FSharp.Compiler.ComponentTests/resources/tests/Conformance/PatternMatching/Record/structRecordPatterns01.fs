// #Conformance #PatternMatching #Records 
#light

// Test ability to match records even if a subset of tags are specified.

type Type = Plant | Animal | Mineral
[<Struct>] type Thing = {Name : string; Age : int; Type : Type}

// Single part
let isAnimal thing = 
    match thing with 
    | {Type=Animal} -> true 
    | _ -> false
    
// Multi part
let isSteve thing =
    match thing with
    | {Name = "Steve"; Age = 2} -> true
    | _ -> false
   

let animal = {Name = "Steve"; Age = 2; Type = Animal}
let plant  = {Name = "Sunflower"; Age = 5; Type = Plant}
let rock   = {Name = "Gold"; Age = 500000; Type = Mineral}

if isAnimal animal <> true  then exit 1
if isAnimal rock   <> false then exit 1

if isSteve animal  <> true  then exit 1
if isSteve plant   <> false then exit 1

exit 0
