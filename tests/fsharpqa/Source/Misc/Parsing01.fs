// #Misc 
#light

// Verify if...then...else parsed as a single unit
// The '|>' should NOT apply to the result of the if...then...else expr:
// just the last value.

if false then 1 else 0 |> exit

exit 1
