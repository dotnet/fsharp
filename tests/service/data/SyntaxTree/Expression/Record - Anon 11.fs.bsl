ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Record - Anon 11.fs", false, QualifiedNameOfFile Module,
      [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (AnonRecd
                (false, None,
                 [(SynLongIdent ([F1], [], [None]), Some (3,6--3,7),
                   App
                     (NonAtomic, false,
                      App
                        (NonAtomic, true,
                         LongIdent
                           (false,
                            SynLongIdent
                              ([op_Equality], [], [Some (OriginalNotation "=")]),
                            None, (4,6--4,7)), Ident F2, (4,3--4,7)),
                      Const (Int32 2, (4,8--4,9)), (4,3--4,9)))], (3,0--4,12),
                 { OpeningBraceRange = (3,0--3,2) }), (3,0--4,12))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,12), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
