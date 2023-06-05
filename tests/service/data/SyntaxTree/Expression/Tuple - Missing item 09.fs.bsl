ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Tuple - Missing item 09.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Paren
                (Tuple
                   (false,
                    [Const (Int32 1, (3,1--3,2));
                     ArbitraryAfterError ("tupleExpr6", (3,3--3,3));
                     ArbitraryAfterError ("tupleExpr7", (3,4--3,4));
                     Const (Int32 2, (3,5--3,6))],
                    [(3,2--3,3); (3,3--3,4); (3,4--3,5)], (3,1--3,6)),
                 (3,0--3,1), Some (3,6--3,7), (3,0--3,7)), (3,0--3,7))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,7), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,4)-(3,5) parse error Unexpected symbol ',' in expression
(3,3)-(3,4) parse error Expecting expression
