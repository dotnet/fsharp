ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 24.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (TryWith
                (Const (Unit, (4,2--4,4)),
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([A], [], [None]), None, None,
                        NamePatPairs
                          ([NamePatPairField
                              (SynLongIdent ([a], [], [None]), Some (6,6--6,7),
                               (6,4--6,9), Wild (6,8--6,9),
                               Some (Semicolon ((6,9--6,10), Some (6,10))));
                            NamePatPairField
                              (SynLongIdent ([b], [], [None]), Some (6,13--6,14),
                               (6,11--6,14),
                               FromParseError (Wild (6,14--6,14), (6,14--6,14)),
                               None)], (6,4--6,16), { ParenRange = (6,3--6,16) }),
                        None, (6,2--6,16)), None, Const (Int32 2, (6,20--6,21)),
                     (6,2--6,21), Yes, { ArrowRange = Some (6,17--6,19)
                                         BarRange = Some (6,0--6,1) })],
                 (3,0--6,21), Yes (3,0--3,3), Yes (5,0--5,4),
                 { TryKeyword = (3,0--3,3)
                   TryToWithRange = (3,0--5,4)
                   WithKeyword = (5,0--5,4)
                   WithToEndRange = (5,0--6,21) }), (3,0--6,21))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,21), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(6,13)-(6,14) parse error Expecting pattern
