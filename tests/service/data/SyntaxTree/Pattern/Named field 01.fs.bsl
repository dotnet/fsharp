ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 01.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,12), Const (Int32 1, (3,6--3,7)),
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([A], [], [None]), None, None,
                        NamePatPairs
                          ([(a, Some (4,6--4,7), Wild (4,8--4,9));
                            (b, Some (4,13--4,14), Wild (4,15--4,16))],
                           (4,4--4,17), { ParenRange = (4,3--4,17) }), None,
                        (4,2--4,17)), None, Const (Int32 2, (4,21--4,22)),
                     (4,2--4,22), Yes, { ArrowRange = Some (4,18--4,20)
                                         BarRange = Some (4,0--4,1) })],
                 (3,0--4,22), { MatchKeyword = (3,0--3,5)
                                WithKeyword = (3,8--3,12) }), (3,0--4,22))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,22), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
