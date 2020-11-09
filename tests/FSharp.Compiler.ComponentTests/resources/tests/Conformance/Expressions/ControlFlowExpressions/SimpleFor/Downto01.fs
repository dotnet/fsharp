// #Conformance #ControlFlow 
#light

// Sanity check the 'downto' keyword

let mutable counter = 0

for i = 10 downto 0 do
    counter <- counter + 1

if counter <> 11 then exit 1

exit 0
