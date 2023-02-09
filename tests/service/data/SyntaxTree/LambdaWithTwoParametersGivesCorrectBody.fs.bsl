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
                        /root/LambdaWithTwoParametersGivesCorrectBody.fs (2,4--2,5))],
                    /root/LambdaWithTwoParametersGivesCorrectBody.fs (2,4--2,5)),
                 Lambda
                   (false, true,
                    SimplePats
                      ([Id
                          (b, None, false, false, false,
                           /root/LambdaWithTwoParametersGivesCorrectBody.fs (2,6--2,7))],
                       /root/LambdaWithTwoParametersGivesCorrectBody.fs (2,6--2,7)),
                    Ident x, None,
                    /root/LambdaWithTwoParametersGivesCorrectBody.fs (2,0--2,12),
                    { ArrowRange =
                       Some
                         /root/LambdaWithTwoParametersGivesCorrectBody.fs (2,8--2,10) }),
                 Some
                   ([Named
                       (SynIdent (a, None), false, None,
                        /root/LambdaWithTwoParametersGivesCorrectBody.fs (2,4--2,5));
                     Named
                       (SynIdent (b, None), false, None,
                        /root/LambdaWithTwoParametersGivesCorrectBody.fs (2,6--2,7))],
                    Ident x),
                 /root/LambdaWithTwoParametersGivesCorrectBody.fs (2,0--2,12),
                 { ArrowRange =
                    Some
                      /root/LambdaWithTwoParametersGivesCorrectBody.fs (2,8--2,10) }),
              /root/LambdaWithTwoParametersGivesCorrectBody.fs (2,0--2,12))],
          PreXmlDocEmpty, [], None,
          /root/LambdaWithTwoParametersGivesCorrectBody.fs (2,0--3,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))