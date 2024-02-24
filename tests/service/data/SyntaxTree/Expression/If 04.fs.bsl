ImplFile
  (ParsedImplFileInput
     ("/root/Expression/If 04.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (IfThenElse
                (ArbitraryAfterError ("if2", (3,2--3,2)),
                 ArbitraryAfterError ("if3", (3,2--3,2)), None, Yes (3,2--3,2),
                 true, (3,0--3,2), { IfKeyword = (3,0--3,2)
                                     IsElif = false
                                     ThenKeyword = (3,0--3,2)
                                     ElseKeyword = None
                                     IfToThenRange = (3,0--3,2) }), (3,0--3,2));
           Expr (Const (Unit, (5,0--5,2)), (5,0--5,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,3)-(5,0) parse error Incomplete structured construct at or before this point in expression
(3,0)-(3,2) parse error Incomplete conditional. Expected 'if <expr> then <expr>' or 'if <expr> then <expr> else <expr>'.
