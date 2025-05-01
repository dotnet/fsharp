ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 02.fs", false, QualifiedNameOfFile Module, [],
      [],
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
                            (b, Some (4,13--4,14),
                             FromParseError (Wild (4,14--4,14), (4,14--4,14)))],
                           (4,4--4,16), { ParenRange = (4,3--4,16) }), None,
                        (4,2--4,16)), None, Const (Int32 2, (4,20--4,21)),
                     (4,2--4,21), Yes, { ArrowRange = Some (4,17--4,19)
                                         BarRange = Some (4,0--4,1) })],
                 (3,0--4,21), { MatchKeyword = (3,0--3,5)
                                WithKeyword = (3,8--3,12) }), (3,0--4,21))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,21), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,15)-(4,16) parse error Unexpected symbol ')' in pattern
