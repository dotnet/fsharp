ImplFile
  (ParsedImplFileInput
     ("/root/UnionCase/Missing keyword of.fs", false, QualifiedNameOfFile Module,
      [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (a, None), false, None, (3,4--3,5)), None,
                  Const (Unit, (3,8--3,10)), (3,4--3,5), Yes (3,0--3,10),
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,6--3,7) })], (3,0--3,10));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [U],
                     PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (5,5--5,6)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (Case1, None), Fields [],
                            PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (6,6--6,11), { BarRange = Some (6,4--6,5) });
                         SynUnionCase
                           ([], SynIdent (Case2, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  LongIdent
                                    (SynLongIdent ([string], [], [None])), false,
                                  PreXmlDoc ((7,12), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (7,12--7,18), { LeadingKeyword = None
                                                        MutableKeyword = None })],
                            PreXmlDoc ((7,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (7,6--7,18), { BarRange = Some (7,4--7,5) });
                         SynUnionCase
                           ([], SynIdent (Case3, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  false,
                                  PreXmlDoc ((8,15), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (8,15--8,18), { LeadingKeyword = None
                                                        MutableKeyword = None })],
                            PreXmlDoc ((8,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (8,6--8,18), { BarRange = Some (8,4--8,5) });
                         SynUnionCase
                           ([], SynIdent (Case4, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  StaticConstant (Int32 4, (9,12--9,13)), false,
                                  PreXmlDoc ((9,12), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (9,12--9,13), { LeadingKeyword = None
                                                        MutableKeyword = None })],
                            PreXmlDoc ((9,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (9,6--9,13), { BarRange = Some (9,4--9,5) })],
                        (6,4--9,13)), (6,4--9,13)), [], None, (5,5--9,13),
                  { LeadingKeyword = Type (5,0--5,4)
                    EqualsRange = Some (5,7--5,8)
                    WithKeyword = None })], (5,0--9,13));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((11,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (b, None), false, None, (11,4--11,5)), None,
                  Const (Unit, (11,8--11,10)), (11,4--11,5), Yes (11,0--11,10),
                  { LeadingKeyword = Let (11,0--11,3)
                    InlineKeyword = None
                    EqualsRange = Some (11,6--11,7) })], (11,0--11,10))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--11,10), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(7,6)-(7,18) parse error Missing keyword 'of'
(9,6)-(9,13) parse error Missing keyword 'of'
