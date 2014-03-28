module Test
// Some syntax tests
module MultiLineTypeParameterNegativeTests = 

    type C<'T,
        'U>() =  // Expect "possible incorrect indentation"
         static let x =  1

    type C1err<'T,
      'U>() =  // Expect "possible incorrect indentation"
         static let x =  1

    type C2err<'T,
     'U>() =  // Expect "possible incorrect indentation"
         static let x =  1

    type C3err<'T,
    'U>() =  // Expect "possible incorrect indentation"
         static let x =  1

    type C4err<'T
               'U>() =  // Expect syntax error if comma omitted
 
         ()
