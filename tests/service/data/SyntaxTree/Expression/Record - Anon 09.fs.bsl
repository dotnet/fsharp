ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Record - Anon 09.fs", false, QualifiedNameOfFile Module,
      [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (AnonRecd
                (false, None,
                 [(SynLongIdent ([F1], [], [None]), Some (3,6--3,7),
                   Const (Int32 1, (3,8--3,9)));
                  (SynLongIdent ([F2], [], [None]), Some (4,6--4,7),
                   App
                     (NonAtomic, false,
                      App
                        (NonAtomic, true,
                         LongIdent
                           (false,
                            SynLongIdent
                              ([op_Equality], [], [Some (OriginalNotation "=")]),
                            None, (5,6--5,7)), Ident F3, (5,3--5,7)),
                      Const (Int32 3, (5,8--5,9)), (5,3--5,9)))], (3,0--5,12),
                 { OpeningBraceRange = (3,0--3,2) }), (3,0--5,12))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,12), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
