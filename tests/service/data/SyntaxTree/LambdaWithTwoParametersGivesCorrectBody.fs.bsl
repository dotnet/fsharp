ImplFile
  (ParsedImplFileInput
     ("/root/LambdaWithTwoParametersGivesCorrectBody.fs", false,
      QualifiedNameOfFile LambdaWithTwoParametersGivesCorrectBody, [], [],
      [SynModuleOrNamespace
         ([LambdaWithTwoParametersGivesCorrectBody], false, AnonModule,
          [Expr
             (Lambda
                (false, false,
                 SimplePats
                   ([Id
                       (a, None, false, false, false,
                        /root/LambdaWithTwoParametersGivesCorrectBody.fs (1,4--1,5))],
                    /root/LambdaWithTwoParametersGivesCorrectBody.fs (1,4--1,5)),
                 Lambda
                   (false, true,
                    SimplePats
                      ([Id
                          (b, None, false, false, false,
                           /root/LambdaWithTwoParametersGivesCorrectBody.fs (1,6--1,7))],
                       /root/LambdaWithTwoParametersGivesCorrectBody.fs (1,6--1,7)),
                    Ident x, None,
                    /root/LambdaWithTwoParametersGivesCorrectBody.fs (1,0--1,12),
                    { ArrowRange =
                       Some
                         /root/LambdaWithTwoParametersGivesCorrectBody.fs (1,8--1,10) }),
                 Some
                   ([Named
                       (SynIdent (a, None), false, None,
                        /root/LambdaWithTwoParametersGivesCorrectBody.fs (1,4--1,5));
                     Named
                       (SynIdent (b, None), false, None,
                        /root/LambdaWithTwoParametersGivesCorrectBody.fs (1,6--1,7))],
                    Ident x),
                 /root/LambdaWithTwoParametersGivesCorrectBody.fs (1,0--1,12),
                 { ArrowRange =
                    Some
                      /root/LambdaWithTwoParametersGivesCorrectBody.fs (1,8--1,10) }),
              /root/LambdaWithTwoParametersGivesCorrectBody.fs (1,0--1,12))],
          PreXmlDocEmpty, [], None,
          /root/LambdaWithTwoParametersGivesCorrectBody.fs (1,0--1,12),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))