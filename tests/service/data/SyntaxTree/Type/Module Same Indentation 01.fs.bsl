ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module Same Indentation 01.fs", false,
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
                ([], None, [], [B],
                 PreXmlDoc ((8,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (8,0--8,8)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((9,4), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([], SynArgInfo ([], false, None)),
                         None),
                      Named (SynIdent (x, None), false, None, (9,8--9,9)), None,
                      Const (Int32 42, (9,12--9,14)), (9,8--9,9),
                      Yes (9,4--9,14), { LeadingKeyword = Let (9,4--9,7)
                                         InlineKeyword = None
                                         EqualsRange = Some (9,10--9,11) })],
                  (9,4--9,14), { InKeyword = None })], false, (8,0--9,14),
              { ModuleKeyword = Some (8,0--8,6)
                EqualsRange = Some (8,9--8,10) });
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [C],
                     PreXmlDoc ((11,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (11,5--11,6)),
                  Simple
                    (Record
                       (None,
                        [SynField
                           ([], false, Some Field,
                            LongIdent (SynLongIdent ([int], [], [None])), false,
                            PreXmlDoc ((12,6), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (12,6--12,16), { LeadingKeyword = None
                                                   MutableKeyword = None })],
                        (12,4--12,18)), (12,4--12,18)), [], None, (11,5--12,18),
                  { LeadingKeyword = Type (11,0--11,4)
                    EqualsRange = Some (11,7--11,8)
                    WithKeyword = None })], (11,0--12,18))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--12,18), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,43)] }, set []))
