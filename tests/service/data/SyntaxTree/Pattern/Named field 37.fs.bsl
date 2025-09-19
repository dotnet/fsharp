ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 37.fs", false, QualifiedNameOfFile Module, [],
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
                               (6,4--6,14),
                               Paren
                                 (Tuple
                                    (false,
                                     [Named
                                        (SynIdent (x, None), false, None,
                                         (6,9--6,10));
                                      Named
                                        (SynIdent (y, None), false, None,
                                         (6,12--6,13))], [(6,10--6,11)],
                                     (6,9--6,13)), (6,8--6,14)), None)],
                           (6,4--6,15), { ParenRange = (6,3--6,15) }), None,
                        (6,2--6,15)), None, Ident x, (6,2--6,20), Yes,
                     { ArrowRange = Some (6,16--6,18)
                       BarRange = Some (6,0--6,1) })], (3,0--6,20),
                 Yes (3,0--3,3), Yes (5,0--5,4),
                 { TryKeyword = (3,0--3,3)
                   TryToWithRange = (3,0--5,4)
                   WithKeyword = (5,0--5,4)
                   WithToEndRange = (5,0--6,20) }), (3,0--6,20))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,20), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
