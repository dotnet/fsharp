ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Tuple - Missing item 01.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Paren
                (Tuple
                   (false,
                    [Ident a; ArbitraryAfterError ("tupleExpr5", (3,3--3,3))],
                    [(3,2--3,3)], (3,1--3,3)), (3,0--3,1), Some (3,3--3,4),
                 (3,0--3,4)), (3,0--3,4))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,4), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,2)-(3,3) parse error Expected an expression after this point
