module Test
// Some syntax tests
module MultiLineTypeParameterNegativeTests = 

    type C4err<'T
               'U>() =  // Expect syntax error if comma omitted

         ()
