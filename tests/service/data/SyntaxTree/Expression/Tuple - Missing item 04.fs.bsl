ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Tuple - Missing item 04.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Paren
                (Tuple
                   (false,
                    [Ident a; ArbitraryAfterError ("tupleExpr8", (3,3--3,3));
                     Ident c], [(3,2--3,3); (3,3--3,4)], (3,1--3,5)), (3,0--3,1),
                 Some (3,5--3,6), (3,0--3,6)), (3,0--3,6))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,6), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,3)-(3,4) parse error Expecting expression
