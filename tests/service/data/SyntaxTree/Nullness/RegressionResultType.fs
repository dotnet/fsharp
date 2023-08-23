    type MyResult<'T,'TError> = 

      /// Represents an OK or a Successful result. The code succeeded with a value of 'T.
      | Ok of ResultValue:'T 

      /// Represents an Error or a Failure. The code failed with a value of 'TError representing what went wrong.
      | Error of ErrorValue:'TError