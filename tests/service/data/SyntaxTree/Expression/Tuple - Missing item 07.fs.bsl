ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Tuple - Missing item 07.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Paren
                (Tuple
                   (false,
                    [Ident a; ArbitraryAfterError ("tupleExpr8", (3,3--3,3));
                     Ident c; ArbitraryAfterError ("tupleExpr2", (3,6--3,6));
                     Ident e; ArbitraryAfterError ("tupleExpr1", (3,9--3,9))],
                    [(3,2--3,3); (3,3--3,4); (3,5--3,6); (3,6--3,7); (3,8--3,9)],
                    (3,1--3,9)), (3,0--3,1), Some (3,9--3,10), (3,0--3,10)),
              (3,0--3,10))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,10), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,3)-(3,4) parse error Expecting expression
(3,6)-(3,7) parse error Expecting expression
(3,8)-(3,9) parse error Expected an expression after this point
