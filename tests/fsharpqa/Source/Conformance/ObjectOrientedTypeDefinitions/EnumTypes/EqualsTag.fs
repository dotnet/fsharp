// #Regression #Conformance #ObjectOrientedTypes #Enums 
#light

// FSB 1574, Name resolution problem: Enum Case "Equals" not handled correctly.

type foo =
    | Add = 1
    | Subtract = 2
    | Multiply = 3
    | Divide = 4
    | Equals = 0

exit (int foo.Equals)
