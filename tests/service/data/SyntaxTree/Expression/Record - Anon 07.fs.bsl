ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Record - Anon 07.fs", false, QualifiedNameOfFile Module,
      [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (AnonRecd
                (false, None,
                 [(SynLongIdent ([F1], [], [None]), Some (3,6--3,7),
                   Const (Int32 1, (3,8--3,9)));
                  (SynLongIdent ([F2], [], [None]), Some (4,6--4,7),
                   ArbitraryAfterError ("anonField", (4,3--4,5)))], (3,0--4,10),
                 { OpeningBraceRange = (3,0--3,2) }), (3,0--4,10))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,10), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,3)-(4,5) parse error Field bindings must have the form 'id = expr;'
