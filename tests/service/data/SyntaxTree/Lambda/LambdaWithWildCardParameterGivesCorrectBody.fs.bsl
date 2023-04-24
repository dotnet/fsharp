ImplFile
  (ParsedImplFileInput
     ("/root/Lambda/LambdaWithWildCardParameterGivesCorrectBody.fs", false,
      QualifiedNameOfFile LambdaWithWildCardParameterGivesCorrectBody, [], [],
      [SynModuleOrNamespace
         ([LambdaWithWildCardParameterGivesCorrectBody], false, AnonModule,
          [Expr
             (Lambda
                (false, false,
                 SimplePats
                   ([Id (a, None, false, false, false, (2,4--2,5))], (2,4--2,5)),
                 Lambda
                   (false, true,
                    SimplePats
                      ([Id (_arg1, None, true, false, false, (2,6--2,7))],
                       (2,6--2,7)),
                    Lambda
                      (false, true,
                       SimplePats
                         ([Id (b, None, false, false, false, (2,8--2,9))],
                          (2,8--2,9)), Ident x, None, (2,0--2,14),
                       { ArrowRange = Some (2,10--2,12) }), None, (2,0--2,14),
                    { ArrowRange = Some (2,10--2,12) }),
                 Some
                   ([Named (SynIdent (a, None), false, None, (2,4--2,5));
                     Wild (2,6--2,7);
                     Named (SynIdent (b, None), false, None, (2,8--2,9))],
                    Ident x), (2,0--2,14), { ArrowRange = Some (2,10--2,12) }),
              (2,0--2,14))], PreXmlDocEmpty, [], None, (2,0--3,0),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
