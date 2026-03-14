ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module At Module Level 01.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [SimpleType],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,15)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (A, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  false,
                                  PreXmlDoc ((5,11), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (5,11--5,14), { LeadingKeyword = None
                                                        MutableKeyword = None })],
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (5,6--5,14), { BarRange = Some (5,4--5,5) });
                         SynUnionCase
                           ([], SynIdent (B, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  LongIdent
                                    (SynLongIdent ([string], [], [None])), false,
                                  PreXmlDoc ((6,11), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (6,11--6,17), { LeadingKeyword = None
                                                        MutableKeyword = None })],
                            PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (6,6--6,17), { BarRange = Some (6,4--6,5) })],
                        (5,4--6,17)), (5,4--6,17)), [], None, (4,5--6,17),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,16--4,17)
                    WithKeyword = None })], (4,0--6,17));
           NestedModule
             (SynComponentInfo
                ([], None, [], [ValidModule],
                 PreXmlDoc ((7,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (7,0--7,18)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((8,4), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([], SynArgInfo ([], false, None)),
                         None),
                      Named (SynIdent (x, None), false, None, (8,8--8,9)), None,
                      Const (Int32 42, (8,12--8,14)), (8,8--8,9),
                      Yes (8,4--8,14), { LeadingKeyword = Let (8,4--8,7)
                                         InlineKeyword = None
                                         EqualsRange = Some (8,10--8,11) })],
                  (8,4--8,14), { InKeyword = None })], false, (7,0--8,14),
              { ModuleKeyword = Some (7,0--7,6)
                EqualsRange = Some (7,19--7,20) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--8,14), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,61)] }, set []))
