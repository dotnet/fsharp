ImplFile
  (ParsedImplFileInput
     ("/root/Expression/InheritRecord - Field 2.fs", false,
      QualifiedNameOfFile Foo, [],
      [SynModuleOrNamespace
         ([Foo], false, NamedModule,
          [Expr
             (Record
                (Some
                   (LongIdent (SynLongIdent ([Exception], [], [None])),
                    Paren
                      (Const
                         (String ("test", Regular, (4,22--4,28)), (4,22--4,28)),
                       (4,21--4,22), Some (4,28--4,29), (4,21--4,29)),
                    (4,12--4,29), Some ((4,30--5,4), None), (4,4--4,11)), None,
                 [SynExprRecordField
                    ((SynLongIdent ([Field1], [], [None]), true),
                     Some (5,11--5,12), Some (Const (Int32 1, (5,13--5,14))),
                     (5,4--5,14), Some ((5,15--6,4), None));
                  SynExprRecordField
                    ((SynLongIdent ([Field2], [], [None]), true),
                     Some (6,11--6,12),
                     Some
                       (Const
                          (String ("two", Regular, (6,13--6,18)), (6,13--6,18))),
                     (6,4--6,18), Some ((6,19--7,4), None));
                  SynExprRecordField
                    ((SynLongIdent ([Field3], [], [None]), true),
                     Some (7,11--7,12), Some (Const (Double 3.0, (7,13--7,16))),
                     (7,4--7,16), None)], (3,0--8,1)), (3,0--8,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--8,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
