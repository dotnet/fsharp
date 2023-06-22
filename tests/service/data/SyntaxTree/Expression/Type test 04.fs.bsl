ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Type test 04.fs", false, QualifiedNameOfFile Module, [],
      [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (IfThenElse
                (TypeTest (Ident i, FromParseError (3,7--3,7), (3,3--3,7)),
                 Const (Unit, (3,13--3,15)), None, Yes (3,0--3,12), false,
                 (3,0--3,15), { IfKeyword = (3,0--3,2)
                                IsElif = false
                                ThenKeyword = (3,8--3,12)
                                ElseKeyword = None
                                IfToThenRange = (3,0--3,12) }), (3,0--3,15))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,15), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,8)-(3,12) parse error Unexpected keyword 'then' in expression
