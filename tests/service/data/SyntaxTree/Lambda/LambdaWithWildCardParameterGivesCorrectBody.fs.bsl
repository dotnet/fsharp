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
                   ([Id
                       (a, None, false, false, false,
                        /root/Lambda/LambdaWithWildCardParameterGivesCorrectBody.fs (2,4--2,5))],
                    /root/Lambda/LambdaWithWildCardParameterGivesCorrectBody.fs (2,4--2,5)),
                 Lambda
                   (false, true,
                    SimplePats
                      ([Id
                          (_arg1, None, true, false, false,
                           /root/Lambda/LambdaWithWildCardParameterGivesCorrectBody.fs (2,6--2,7))],
                       /root/Lambda/LambdaWithWildCardParameterGivesCorrectBody.fs (2,6--2,7)),
                    Lambda
                      (false, true,
                       SimplePats
                         ([Id
                             (b, None, false, false, false,
                              /root/Lambda/LambdaWithWildCardParameterGivesCorrectBody.fs (2,8--2,9))],
                          /root/Lambda/LambdaWithWildCardParameterGivesCorrectBody.fs (2,8--2,9)),
                       Ident x, None,
                       /root/Lambda/LambdaWithWildCardParameterGivesCorrectBody.fs (2,0--2,14),
                       { ArrowRange =
                          Some
                            /root/Lambda/LambdaWithWildCardParameterGivesCorrectBody.fs (2,10--2,12) }),
                    None,
                    /root/Lambda/LambdaWithWildCardParameterGivesCorrectBody.fs (2,0--2,14),
                    { ArrowRange =
                       Some
                         /root/Lambda/LambdaWithWildCardParameterGivesCorrectBody.fs (2,10--2,12) }),
                 Some
                   ([Named
                       (SynIdent (a, None), false, None,
                        /root/Lambda/LambdaWithWildCardParameterGivesCorrectBody.fs (2,4--2,5));
                     Wild
                       /root/Lambda/LambdaWithWildCardParameterGivesCorrectBody.fs (2,6--2,7);
                     Named
                       (SynIdent (b, None), false, None,
                        /root/Lambda/LambdaWithWildCardParameterGivesCorrectBody.fs (2,8--2,9))],
                    Ident x),
                 /root/Lambda/LambdaWithWildCardParameterGivesCorrectBody.fs (2,0--2,14),
                 { ArrowRange =
                    Some
                      /root/Lambda/LambdaWithWildCardParameterGivesCorrectBody.fs (2,10--2,12) }),
              /root/Lambda/LambdaWithWildCardParameterGivesCorrectBody.fs (2,0--2,14))],
          PreXmlDocEmpty, [], None,
          /root/Lambda/LambdaWithWildCardParameterGivesCorrectBody.fs (2,0--3,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
