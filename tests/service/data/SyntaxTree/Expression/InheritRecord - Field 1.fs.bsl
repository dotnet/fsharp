ImplFile
  (ParsedImplFileInput
     ("/root/Expression/InheritRecord - Field 1.fs", false,
      QualifiedNameOfFile Foo, [],
      [SynModuleOrNamespace
         ([Foo], false, NamedModule,
          [Expr
             (Record
                (Some
                   (LongIdent (SynLongIdent ([Exception], [], [None])),
                    Paren
                      (App
                         (NonAtomic, false,
                          App
                            (NonAtomic, true,
                             LongIdent
                               (false,
                                SynLongIdent
                                  ([op_Addition], [],
                                   [Some (OriginalNotation "+")]), None,
                                (5,17--5,18)),
                             App
                               (NonAtomic, false,
                                App
                                  (NonAtomic, true,
                                   LongIdent
                                     (false,
                                      SynLongIdent
                                        ([op_Addition], [],
                                         [Some (OriginalNotation "+")]), None,
                                      (4,17--4,18)),
                                   Const
                                     (String
                                        ("This is a ", Regular, (4,4--4,16)),
                                      (4,4--4,16)), (4,4--4,18)),
                                Const
                                  (String ("multiline ", Regular, (5,4--5,16)),
                                   (5,4--5,16)), (4,4--5,16)), (4,4--5,18)),
                          Const
                            (String ("message", Regular, (6,4--6,13)),
                             (6,4--6,13)), (4,4--6,13)), (3,19--3,20),
                       Some (7,2--7,3), (3,19--7,3)), (3,10--7,3),
                    Some ((7,4--8,2), None), (3,2--3,9)), None,
                 [SynExprRecordField
                    ((SynLongIdent ([X], [], [None]), true), Some (8,4--8,5),
                     Some (Const (Int32 42, (8,6--8,8))), (8,2--8,8),
                     Some ((8,9--9,2), None));
                  SynExprRecordField
                    ((SynLongIdent ([Y], [], [None]), true), Some (9,4--9,5),
                     Some
                       (Const
                          (String ("test", Regular, (9,6--9,12)), (9,6--9,12))),
                     (9,2--9,12), None)], (3,0--10,1)), (3,0--10,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--10,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
