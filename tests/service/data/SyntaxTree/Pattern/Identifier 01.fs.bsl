ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Identifier 01.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,12), Ident e,
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([E], [], [None]), None, None, Pats [],
                        None, (4,2--4,3)), None, Const (Unit, (4,7--4,9)),
                     (4,2--4,9), Yes, { ArrowRange = Some (4,4--4,6)
                                        BarRange = Some (4,0--4,1) })],
                 (3,0--4,9), { MatchKeyword = (3,0--3,5)
                               WithKeyword = (3,8--3,12) }), (3,0--4,9))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,9), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
