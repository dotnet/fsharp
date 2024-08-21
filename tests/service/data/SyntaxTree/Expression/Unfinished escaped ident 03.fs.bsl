ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Unfinished escaped ident 03.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,13), Const (Unit, (3,6--3,8)),
                 [SynMatchClause
                    (Named (SynIdent (, None), false, None, (4,2--4,3)), None,
                     Const (Unit, (4,7--4,9)), (4,2--4,9), Yes,
                     { ArrowRange = Some (4,4--4,6)
                       BarRange = Some (4,0--4,1) });
                  SynMatchClause
                    (Named (SynIdent (, None), false, None, (5,2--5,6)), None,
                     Const (Unit, (5,10--5,12)), (5,2--5,12), Yes,
                     { ArrowRange = Some (5,7--5,9)
                       BarRange = Some (5,0--5,1) })], (3,0--5,12),
                 { MatchKeyword = (3,0--3,5)
                   WithKeyword = (3,9--3,13) }), (3,0--5,12));
           Expr
             (Match
                (Yes (7,0--7,13), Const (Unit, (7,6--7,8)),
                 [SynMatchClause
                    (Or
                       (Named
                          (SynIdent ( -> (), None), false, None, (8,2--8,10)),
                        Wild (9,2--9,3), (8,2--9,3), { BarRange = (9,0--9,1) }),
                     None, Const (Unit, (9,7--9,9)), (8,2--9,9), Yes,
                     { ArrowRange = Some (9,4--9,6)
                       BarRange = Some (8,0--8,1) })], (7,0--9,9),
                 { MatchKeyword = (7,0--7,5)
                   WithKeyword = (7,9--7,13) }), (7,0--9,9))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--9,9), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,2)-(4,3) parse error This is not a valid identifier
(5,2)-(5,6) parse error This is not a valid identifier
(8,2)-(8,10) parse error This is not a valid identifier
