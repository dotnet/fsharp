ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 08.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,20),
                 Record
                   (None, None,
                    [SynExprRecordField
                       ((SynLongIdent ([A], [], [None]), true),
                        Some (3,10--3,11), Some (Const (Int32 1, (3,12--3,13))),
                        (3,8--3,13), None)], (3,6--3,15)),
                 [SynMatchClause
                    (Record
                       ([NamePatPairField
                           (SynLongIdent ([A], [], [None]), None, (4,4--4,5),
                            FromParseError (Wild (4,5--4,5), (4,5--4,5)),
                            Some ((4,6--4,7), Some (4,7)));
                         NamePatPairField
                           (SynLongIdent ([B], [], [None]), Some (4,10--4,11),
                            (4,8--4,13), Const (Int32 3, (4,12--4,13)), None)],
                        (4,2--4,15)), None, Ident a, (4,2--4,20), Yes,
                     { ArrowRange = Some (4,16--4,18)
                       BarRange = Some (4,0--4,1) })], (3,0--4,20),
                 { MatchKeyword = (3,0--3,5)
                   WithKeyword = (3,16--3,20) }), (3,0--4,20))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,20), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(4,6)-(4,7) parse error Unexpected symbol ';' in pattern. Expected '.', '=' or other token.
