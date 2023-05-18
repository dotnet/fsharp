ImplFile
  (ParsedImplFileInput
     ("/root/Lambda/Param - Missing type 02.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Paren
                (FromParseError
                   (Lambda
                      (false, false,
                       SimplePats
                         ([Id (i, None, false, false, false, (3,5--3,6))], [],
                          (3,5--3,6)),
                       ArbitraryAfterError ("anonLambdaExpr4", (3,6--3,6)),
                       Some
                         ([Named (SynIdent (i, None), false, None, (3,5--3,6))],
                          ArbitraryAfterError ("anonLambdaExpr4", (3,6--3,6))),
                       (3,1--3,6), { ArrowRange = None }), (3,1--3,6)),
                 (3,0--3,1), Some (3,13--3,14), (3,0--3,14)), (3,0--3,14))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,14), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,6)-(3,7) parse error Unexpected symbol ':' in lambda expression. Expected '->' or other token.
