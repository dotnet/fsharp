ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Record - Anon 08.fs", false, QualifiedNameOfFile Module,
      [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (AnonRecd
                (false, None,
                 [(SynLongIdent ([F1], [], [None]), Some (3,6--3,7),
                   Const (Int32 1, (3,8--3,9)));
                  (SynLongIdent ([F2], [], [None]), None,
                   ArbitraryAfterError ("anonField", (4,3--4,5)))], (3,0--4,8),
                 { OpeningBraceRange = (3,0--3,2) }), (3,0--4,8))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,8), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,3)-(4,5) parse error Field bindings must have the form 'id = expr;'
