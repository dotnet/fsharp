ImplFile
  (ParsedImplFileInput
     ("/root/Lambda/LambdaWithWildCardThatReturnsALambdaGivesCorrectBody.fs",
      false,
      QualifiedNameOfFile LambdaWithWildCardThatReturnsALambdaGivesCorrectBody,
      [], [],
      [SynModuleOrNamespace
         ([LambdaWithWildCardThatReturnsALambdaGivesCorrectBody], false,
          AnonModule,
          [Expr
             (Lambda
                (false, false,
                 SimplePats
                   ([Id (_arg2, None, true, false, false, (2,4--2,5))],
                    (2,4--2,5)),
                 Lambda
                   (false, false,
                    SimplePats
                      ([Id (_arg1, None, true, false, false, (2,13--2,14))],
                       (2,13--2,14)), Ident x,
                    Some ([Wild (2,13--2,14)], Ident x), (2,9--2,19),
                    { ArrowRange = Some (2,15--2,17) }),
                 Some
                   ([Wild (2,4--2,5)],
                    Lambda
                      (false, false,
                       SimplePats
                         ([Id (_arg1, None, true, false, false, (2,13--2,14))],
                          (2,13--2,14)), Ident x,
                       Some ([Wild (2,13--2,14)], Ident x), (2,9--2,19),
                       { ArrowRange = Some (2,15--2,17) })), (2,0--2,19),
                 { ArrowRange = Some (2,6--2,8) }), (2,0--2,19))],
          PreXmlDocEmpty, [], None, (2,0--3,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
