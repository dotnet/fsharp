// #Regression #Misc 
// Verify warnings associated with top level expressions getting discarded

//<Expects id="FS0020" status="warning">This expression should have type 'unit', but has type '\( \^a -> unit\) \* int'</Expects>
//<Expects id="FS0020" status="warning">This expression should have type 'unit', but has type '\('a \[\] -> unit \[\]\) \* string \[\]'</Expects>

// Note the comma between printf "%A", this results in a tuple expr which probabaly wasn't intended.
let arr = [|"Foo"; "Bar"|]
printf "%d", arr.Length

// Again, note the comma
Array.map (fun a -> printf "%A" a), arr

// printf "%s\n", String.Join(",", files1)

exit 0
