ImplFile
  (ParsedImplFileInput
     ("/root/Member/Member 01.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  ObjectModel
                    (Unspecified,
                     [Member
                        (SynBinding
                           (None, Normal, false, false,
                            [{ Attributes =
                                [{ TypeName = SynLongIdent ([A], [], [None])
                                   ArgExpr = Const (Unit, (4,6--4,7))
                                   Target = None
                                   AppliesToGetterAndSetter = false
                                   Range = (4,6--4,7) }]
                               Range = (4,4--4,9) }],
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo
                                 ([[SynArgInfo ([], false, None)]; []],
                                  SynArgInfo ([], false, None)), None),
                            LongIdent
                              (SynLongIdent
                                 ([this; P1], [(4,21--4,22)], [None; None]),
                               None, None, Pats [], None, (4,17--4,24)), None,
                            Const (Int32 1, (4,27--4,28)), (4,4--4,24),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (4,10--4,16)
                              InlineKeyword = None
                              EqualsRange = Some (4,25--4,26) }), (4,4--4,28));
                      Member
                        (SynBinding
                           (None, Normal, false, false,
                            [{ Attributes =
                                [{ TypeName = SynLongIdent ([B], [], [None])
                                   ArgExpr = Const (Unit, (5,6--5,7))
                                   Target = None
                                   AppliesToGetterAndSetter = false
                                   Range = (5,6--5,7) }]
                               Range = (5,4--5,9) }],
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo
                                 ([[SynArgInfo ([], false, None)]; []],
                                  SynArgInfo ([], false, None)), None),
                            FromParseError (Wild (5,16--5,16), (5,16--5,16)),
                            None,
                            ArbitraryAfterError
                              ("classDefnMember1", (5,16--5,16)), (5,4--5,16),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (5,10--5,16)
                              InlineKeyword = None
                              EqualsRange = None }), (5,4--5,16));
                      Member
                        (SynBinding
                           (None, Normal, false, false,
                            [{ Attributes =
                                [{ TypeName = SynLongIdent ([C], [], [None])
                                   ArgExpr = Const (Unit, (6,6--6,7))
                                   Target = None
                                   AppliesToGetterAndSetter = false
                                   Range = (6,6--6,7) }]
                               Range = (6,4--6,9) }],
                            PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo
                                 ([[SynArgInfo ([], false, None)]; []],
                                  SynArgInfo ([], false, None)), None),
                            LongIdent
                              (SynLongIdent
                                 ([this; P3], [(6,21--6,22)], [None; None]),
                               None, None, Pats [], None, (6,17--6,24)), None,
                            Const (Int32 3, (6,27--6,28)), (6,4--6,24),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (6,10--6,16)
                              InlineKeyword = None
                              EqualsRange = Some (6,25--6,26) }), (6,4--6,28))],
                     (4,4--6,28)), [], None, (3,5--6,28),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--6,28))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,28), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(5,17)-(6,4) parse error Incomplete structured construct at or before this point in member definition
