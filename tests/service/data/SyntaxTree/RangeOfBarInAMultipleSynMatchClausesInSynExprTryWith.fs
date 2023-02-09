
try
    foo ()
with
| IOException as ioex ->
    // some comment
    ()
| ex -> ()
