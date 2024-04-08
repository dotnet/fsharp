ImplFile
  (ParsedImplFileInput
     ("/root/Expression/If 03.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (IfThenElse
                (Const (Bool true, (3,3--3,7)),
                 ArbitraryAfterError ("if1", (3,7--3,7)), None, Yes (3,0--3,7),
                 true, (3,0--3,7), { IfKeyword = (3,0--3,2)
                                     IsElif = false
                                     ThenKeyword = (3,7--3,7)
                                     ElseKeyword = None
                                     IfToThenRange = (3,0--3,7) }), (3,0--3,7));
           Expr (Const (Unit, (5,0--5,2)), (5,0--5,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,8)-(5,0) parse error Incomplete structured construct at or before this point in expression
(3,0)-(3,2) parse error Incomplete conditional. Expected 'if <expr> then <expr>' or 'if <expr> then <expr> else <expr>'.
