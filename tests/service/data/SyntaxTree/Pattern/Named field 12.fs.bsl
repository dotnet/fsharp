ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 12.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,16), Ident shape,
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([Rectangle], [], [None]), None, None,
                        NamePatPairs
                          ([(width, Some (4,18--4,19),
                             Named
                               (SynIdent (w, None), false, None, (4,20--4,21)));
                            (length, Some (4,30--4,31),
                             Named
                               (SynIdent (l, None), false, None, (4,32--4,33)))],
                           (4,12--4,34), { ParenRange = (4,11--4,34) }), None,
                        (4,2--4,34)), None,
                     App
                       (NonAtomic, false,
                        App
                          (NonAtomic, true,
                           LongIdent
                             (false,
                              SynLongIdent
                                ([op_Multiply], [],
                                 [Some (OriginalNotation "*")]), None,
                              (4,40--4,41)), Ident w, (4,38--4,41)), Ident l,
                        (4,38--4,43)), (4,2--4,43), Yes,
                     { ArrowRange = Some (4,35--4,37)
                       BarRange = Some (4,0--4,1) });
                  SynMatchClause
                    (LongIdent
                       (SynLongIdent ([Circle], [], [None]), None, None,
                        NamePatPairs
                          ([(radius, Some (5,16--5,17),
                             Named
                               (SynIdent (r, None), false, None, (5,18--5,19)))],
                           (5,9--5,20), { ParenRange = (5,8--5,20) }), None,
                        (5,2--5,20)), None,
                     App
                       (NonAtomic, false,
                        App
                          (NonAtomic, true,
                           LongIdent
                             (false,
                              SynLongIdent
                                ([op_Multiply], [],
                                 [Some (OriginalNotation "*")]), None,
                              (5,39--5,40)),
                           LongIdent
                             (false,
                              SynLongIdent
                                ([System; Math; PI],
                                 [(5,30--5,31); (5,35--5,36)],
                                 [None; None; None]), None, (5,24--5,38)),
                           (5,24--5,40)),
                        App
                          (NonAtomic, false,
                           App
                             (NonAtomic, true,
                              LongIdent
                                (false,
                                 SynLongIdent
                                   ([op_Exponentiation], [],
                                    [Some (OriginalNotation "**")]), None,
                                 (5,43--5,45)), Ident r, (5,41--5,45)),
                           Const (Double 2.0, (5,46--5,48)), (5,41--5,48)),
                        (5,24--5,48)), (5,2--5,48), Yes,
                     { ArrowRange = Some (5,21--5,23)
                       BarRange = Some (5,0--5,1) });
                  SynMatchClause
                    (LongIdent
                       (SynLongIdent ([Prism], [], [None]), None, None,
                        NamePatPairs
                          ([(width, Some (6,14--6,15),
                             Named
                               (SynIdent (w, None), false, None, (6,16--6,17)));
                            (length, Some (6,26--6,27),
                             Named
                               (SynIdent (l, None), false, None, (6,28--6,29)));
                            (height, Some (6,38--6,39),
                             Named
                               (SynIdent (h, None), false, None, (6,40--6,41)))],
                           (6,8--6,42), { ParenRange = (6,7--6,42) }), None,
                        (6,2--6,42)), None,
                     App
                       (NonAtomic, false,
                        App
                          (NonAtomic, true,
                           LongIdent
                             (false,
                              SynLongIdent
                                ([op_Multiply], [],
                                 [Some (OriginalNotation "*")]), None,
                              (6,52--6,53)),
                           App
                             (NonAtomic, false,
                              App
                                (NonAtomic, true,
                                 LongIdent
                                   (false,
                                    SynLongIdent
                                      ([op_Multiply], [],
                                       [Some (OriginalNotation "*")]), None,
                                    (6,48--6,49)), Ident w, (6,46--6,49)),
                              Ident l, (6,46--6,51)), (6,46--6,53)), Ident h,
                        (6,46--6,55)), (6,2--6,55), Yes,
                     { ArrowRange = Some (6,43--6,45)
                       BarRange = Some (6,0--6,1) })], (3,0--6,55),
                 { MatchKeyword = (3,0--3,5)
                   WithKeyword = (3,12--3,16) }), (3,0--6,55))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,55), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
