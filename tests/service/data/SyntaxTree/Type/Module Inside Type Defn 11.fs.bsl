ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module Inside Type Defn 11.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (x, None), false, None, (3,4--3,5)), None,
                  Const (Int32 1, (3,8--3,9)), (3,4--3,5), Yes (3,0--3,9),
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,6--3,7) })], (3,0--3,9));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [x_t],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,8)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([int], [], [None])),
                        (4,11--4,14)), (4,11--4,14)), [], None, (4,5--4,14),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,9--4,10)
                    WithKeyword = None })], (4,0--4,14));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [t],
                     PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (5,5--5,6)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (X, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  false,
                                  PreXmlDoc ((6,9), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (6,9--6,12), { LeadingKeyword = None
                                                       MutableKeyword = None })],
                            PreXmlDoc ((6,2), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (6,4--6,12), { BarRange = Some (6,2--6,3) })],
                        (6,2--6,12)), (6,2--6,12)), [], None, (5,5--6,12),
                  { LeadingKeyword = Type (5,0--5,4)
                    EqualsRange = Some (5,7--5,8)
                    WithKeyword = None })], (5,0--6,12));
           NestedModule
             (SynComponentInfo
                ([], None, [], [Nested],
                 PreXmlDoc ((7,2), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (7,2--7,15)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((8,4), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([], SynArgInfo ([], false, None)),
                         None),
                      Named (SynIdent (x, None), false, None, (8,8--8,9)), None,
                      Const (Int32 1, (8,12--8,13)), (8,8--8,9), Yes (8,4--8,13),
                      { LeadingKeyword = Let (8,4--8,7)
                        InlineKeyword = None
                        EqualsRange = Some (8,10--8,11) })], (8,4--8,13));
               Types
                 ([SynTypeDefn
                     (SynComponentInfo
                        ([], None, [], [x_t],
                         PreXmlDoc ((9,4), FSharp.Compiler.Xml.XmlDocCollector),
                         false, None, (9,9--9,12)),
                      Simple
                        (TypeAbbrev
                           (Ok, LongIdent (SynLongIdent ([int], [], [None])),
                            (9,15--9,18)), (9,15--9,18)), [], None, (9,9--9,18),
                      { LeadingKeyword = Type (9,4--9,8)
                        EqualsRange = Some (9,13--9,14)
                        WithKeyword = None })], (9,4--9,18));
               Types
                 ([SynTypeDefn
                     (SynComponentInfo
                        ([], None, [], [t],
                         PreXmlDoc ((10,4), FSharp.Compiler.Xml.XmlDocCollector),
                         false, None, (10,9--10,10)),
                      Simple
                        (Union
                           (None,
                            [SynUnionCase
                               ([], SynIdent (X, None),
                                Fields
                                  [SynField
                                     ([], false, None,
                                      LongIdent
                                        (SynLongIdent ([int], [], [None])),
                                      false,
                                      PreXmlDoc ((10,18), FSharp.Compiler.Xml.XmlDocCollector),
                                      None, (10,18--10,21),
                                      { LeadingKeyword = None
                                        MutableKeyword = None })],
                                PreXmlDoc ((10,13), FSharp.Compiler.Xml.XmlDocCollector),
                                None, (10,13--10,21), { BarRange = None })],
                            (10,13--10,21)), (10,13--10,21)), [], None,
                      (10,9--10,21), { LeadingKeyword = Type (10,4--10,8)
                                       EqualsRange = Some (10,11--10,12)
                                       WithKeyword = None })], (10,4--10,21))],
              false, (7,2--11,5), { ModuleKeyword = Some (7,2--7,8)
                                    EqualsRange = Some (7,16--7,17) })],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--11,5), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(7,2)-(7,8) parse warning Invalid declaration syntax
