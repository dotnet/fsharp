ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module At Type Column 01.fs", false,
      QualifiedNameOfFile Module, [],
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
                            None, (5,6--5,14), { BarRange = Some (5,4--5,5) })],
                        (5,4--5,14)), (5,4--5,14)), [], None, (4,5--5,14),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,7--4,8)
                    WithKeyword = None })], (4,0--5,14));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [B],
                     PreXmlDoc ((6,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (6,5--6,6)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([B], [], [None])),
                        (6,9--6,10)), (6,9--6,10)), [], None, (6,5--6,10),
                  { LeadingKeyword = Type (6,0--6,4)
                    EqualsRange = Some (6,7--6,8)
                    WithKeyword = None })], (6,0--6,10));
           NestedModule
             (SynComponentInfo
                ([], None, [], [C],
                 PreXmlDoc ((7,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (7,0--7,8)), false,
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
                EqualsRange = Some (7,9--7,10) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--8,13), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,48)] }, set []))
