ImplFile
  (ParsedImplFileInput
     ("/root/Lambda/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs",
      false,
      QualifiedNameOfFile LambdaWithTupleParameterWithWildCardGivesCorrectBody,
      [], [],
      [SynModuleOrNamespace
         ([LambdaWithTupleParameterWithWildCardGivesCorrectBody], false,
          AnonModule,
          [Expr
             (Lambda
                (false, false,
                 SimplePats
                   ([Id (a, None, false, false, false, (2,4--2,5))], (2,4--2,5)),
                 Lambda
                   (false, true,
                    SimplePats
                      ([Id (b, None, false, false, false, (2,7--2,8));
                        Id (_arg1, None, true, false, false, (2,10--2,11))],
                       (2,6--2,12)),
                    Lambda
                      (false, true,
                       SimplePats
                         ([Id (c, None, false, false, false, (2,13--2,14))],
                          (2,13--2,14)), Ident x, None, (2,0--2,19),
                       { ArrowRange = Some (2,15--2,17) }), None, (2,0--2,19),
                    { ArrowRange = Some (2,15--2,17) }),
                 Some
                   ([Named (SynIdent (a, None), false, None, (2,4--2,5));
                     Paren
                       (Tuple
                          (false,
                           [Named (SynIdent (b, None), false, None, (2,7--2,8));
                            Wild (2,10--2,11)], (2,7--2,11)), (2,6--2,12));
                     Named (SynIdent (c, None), false, None, (2,13--2,14))],
                    Ident x), (2,0--2,19), { ArrowRange = Some (2,15--2,17) }),
              (2,0--2,19))], PreXmlDocEmpty, [], None, (2,0--3,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
