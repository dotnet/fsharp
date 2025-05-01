// #Regression #Conformance #DeclarationElements #Accessibility 
// On let







module M =
            private   let x1 = 42      // here
            public    let x2 = 42      // here
            internal  let x3 = 42      // here
