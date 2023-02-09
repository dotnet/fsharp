ImplFile
  (ParsedImplFileInput
     ("/root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs", false,
      QualifiedNameOfFile LambdaWithTupleParameterWithWildCardGivesCorrectBody,
      [], [],
      [SynModuleOrNamespace
         ([LambdaWithTupleParameterWithWildCardGivesCorrectBody], false,
          AnonModule,
          [Expr
             (Lambda
                (false, false,
                 SimplePats
                   ([Id
                       (a, None, false, false, false,
                        /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (1,4--1,5))],
                    /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (1,4--1,5)),
                 Lambda
                   (false, true,
                    SimplePats
                      ([Id
                          (b, None, false, false, false,
                           /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (1,7--1,8));
                        Id
                          (_arg1, None, true, false, false,
                           /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (1,10--1,11))],
                       /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (1,6--1,12)),
                    Lambda
                      (false, true,
                       SimplePats
                         ([Id
                             (c, None, false, false, false,
                              /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (1,13--1,14))],
                          /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (1,13--1,14)),
                       Ident x, None,
                       /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (1,0--1,19),
                       { ArrowRange =
                          Some
                            /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (1,15--1,17) }),
                    None,
                    /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (1,0--1,19),
                    { ArrowRange =
                       Some
                         /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (1,15--1,17) }),
                 Some
                   ([Named
                       (SynIdent (a, None), false, None,
                        /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (1,4--1,5));
                     Paren
                       (Tuple
                          (false,
                           [Named
                              (SynIdent (b, None), false, None,
                               /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (1,7--1,8));
                            Wild
                              /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (1,10--1,11)],
                           /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (1,7--1,11)),
                        /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (1,6--1,12));
                     Named
                       (SynIdent (c, None), false, None,
                        /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (1,13--1,14))],
                    Ident x),
                 /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (1,0--1,19),
                 { ArrowRange =
                    Some
                      /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (1,15--1,17) }),
              /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (1,0--1,19))],
          PreXmlDocEmpty, [], None,
          /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (1,0--1,19),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))