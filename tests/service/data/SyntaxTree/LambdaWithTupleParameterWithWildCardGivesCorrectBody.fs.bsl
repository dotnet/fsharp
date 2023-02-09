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
                        /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (2,4--2,5))],
                    /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (2,4--2,5)),
                 Lambda
                   (false, true,
                    SimplePats
                      ([Id
                          (b, None, false, false, false,
                           /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (2,7--2,8));
                        Id
                          (_arg1, None, true, false, false,
                           /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (2,10--2,11))],
                       /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (2,6--2,12)),
                    Lambda
                      (false, true,
                       SimplePats
                         ([Id
                             (c, None, false, false, false,
                              /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (2,13--2,14))],
                          /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (2,13--2,14)),
                       Ident x, None,
                       /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (2,0--2,19),
                       { ArrowRange =
                          Some
                            /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (2,15--2,17) }),
                    None,
                    /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (2,0--2,19),
                    { ArrowRange =
                       Some
                         /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (2,15--2,17) }),
                 Some
                   ([Named
                       (SynIdent (a, None), false, None,
                        /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (2,4--2,5));
                     Paren
                       (Tuple
                          (false,
                           [Named
                              (SynIdent (b, None), false, None,
                               /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (2,7--2,8));
                            Wild
                              /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (2,10--2,11)],
                           /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (2,7--2,11)),
                        /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (2,6--2,12));
                     Named
                       (SynIdent (c, None), false, None,
                        /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (2,13--2,14))],
                    Ident x),
                 /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (2,0--2,19),
                 { ArrowRange =
                    Some
                      /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (2,15--2,17) }),
              /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (2,0--2,19))],
          PreXmlDocEmpty, [], None,
          /root/LambdaWithTupleParameterWithWildCardGivesCorrectBody.fs (2,0--3,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))