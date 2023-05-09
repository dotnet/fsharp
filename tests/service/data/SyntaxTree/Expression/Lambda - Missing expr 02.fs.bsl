ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Lambda - Missing expr 02.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Lambda
                (false, false,
                 SimplePats
                   ([Id (_arg1, None, true, false, false, (3,4--3,5))],
                    (3,4--3,5)),
                 ArbitraryAfterError ("anonLambdaExpr2", (3,8--3,8)),
                 Some
                   ([Wild (3,4--3,5)],
                    ArbitraryAfterError ("anonLambdaExpr2", (3,8--3,8))),
                 (3,0--3,8), { ArrowRange = Some (3,6--3,8) }), (3,0--3,8))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,8), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,0)-(4,0) parse warning Possible incorrect indentation: this token is offside of context started at position (1:1). Try indenting this token further or using standard formatting conventions.
(3,0)-(3,8) parse error Missing function body
