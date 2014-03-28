// #Conformance #PatternMatching #Constants 
#light

// Match floating point numbers
type Number = NotPI | SortOfPI | CloseToPI | CloseEnoughToPI

let piCheck x =
    match x with
    | 3.1    -> NotPI
    | 3.14   -> SortOfPI
    | 3.141  -> CloseToPI
    | 3.1415 -> CloseEnoughToPI
    | _ when System.Math.Abs((x - 3.1415)) < 0.0001 -> CloseEnoughToPI
    | _ -> NotPI

if piCheck 3.1      <> NotPI           then exit 1
if piCheck 3.14     <> SortOfPI        then exit 1
if piCheck 3.141    <> CloseToPI       then exit 1
if piCheck 3.1415   <> CloseEnoughToPI then exit 1
if piCheck 3.14159  <> CloseEnoughToPI then exit 1
if piCheck 3.141592 <> CloseEnoughToPI then exit 1

if piCheck 3.0   <> NotPI then exit 1
if piCheck -42.0 <> NotPI then exit 1

exit 0
