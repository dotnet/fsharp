ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Lambda - Missing expr 01.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Paren
                (Lambda
                   (false, false,
                    SimplePats
                      ([Id (_arg1, None, true, false, false, (3,5--3,6))],
                       (3,5--3,6)),
                    ArbitraryAfterError ("anonLambdaExpr2", (3,9--3,9)),
                    Some
                      ([Wild (3,5--3,6)],
                       ArbitraryAfterError ("anonLambdaExpr2", (3,9--3,9))),
                    (3,1--3,9), { ArrowRange = Some (3,7--3,9) }), (3,0--3,1),
                 Some (3,10--3,11), (3,0--3,11)), (3,0--3,11))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,11), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,1)-(3,9) parse error Missing function body
