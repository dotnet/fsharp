ImplFile
  (ParsedImplFileInput
     ("/root/Lambda/LambdaWithTwoParametersGivesCorrectBody.fs", false,
      QualifiedNameOfFile LambdaWithTwoParametersGivesCorrectBody, [], [],
      [SynModuleOrNamespace
         ([LambdaWithTwoParametersGivesCorrectBody], false, AnonModule,
          [Expr
             (Lambda
                (false, false,
                 SimplePats
                   ([Id (a, None, false, false, false, (2,4--2,5))], (2,4--2,5)),
                 Lambda
                   (false, true,
                    SimplePats
                      ([Id (b, None, false, false, false, (2,6--2,7))],
                       (2,6--2,7)), Ident x, None, (2,0--2,12),
                    { ArrowRange = Some (2,8--2,10) }),
                 Some
                   ([Named (SynIdent (a, None), false, None, (2,4--2,5));
                     Named (SynIdent (b, None), false, None, (2,6--2,7))],
                    Ident x), (2,0--2,12), { ArrowRange = Some (2,8--2,10) }),
              (2,0--2,12))], PreXmlDocEmpty, [], None, (2,0--3,0),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
