ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module Less Indented 01.fs", false, QualifiedNameOfFile Module,
      [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [A],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,6)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (CaseA, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  false,
                                  PreXmlDoc ((5,15), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (5,15--5,18), { LeadingKeyword = None
                                                        MutableKeyword = None })],
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (5,6--5,18), { BarRange = Some (5,4--5,5) });
                         SynUnionCase
                           ([], SynIdent (CaseB, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  LongIdent
                                    (SynLongIdent ([string], [], [None])), false,
                                  PreXmlDoc ((6,15), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (6,15--6,21), { LeadingKeyword = None
                                                        MutableKeyword = None })],
                            PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (6,6--6,21), { BarRange = Some (6,4--6,5) })],
                        (5,4--6,21)), (5,4--6,21)), [], None, (4,5--6,21),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,7--4,8)
                    WithKeyword = None })], (4,0--6,21));
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
                      Const (Int32 1, (8,12--8,13)), (8,8--8,9), Yes (8,4--8,13),
                      { LeadingKeyword = Let (8,4--8,7)
                        InlineKeyword = None
                        EqualsRange = Some (8,10--8,11) })], (8,4--8,13),
                  { InKeyword = None })], false, (7,0--8,13),
              { ModuleKeyword = Some (7,0--7,6)
                EqualsRange = Some (7,19--7,20) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--8,13), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,50)] }, set []))
