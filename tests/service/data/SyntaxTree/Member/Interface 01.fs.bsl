ImplFile
  (ParsedImplFileInput
     ("/root/Member/Interface 01.fs", false, QualifiedNameOfFile Module, [], [],
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
                     [Interface
                        (LongIdent (SynLongIdent ([I], [], [None])),
                         Some (4,16--4,20),
                         Some
                           [Member
                              (SynBinding
                                 (None, Normal, false, false, [],
                                  PreXmlDoc ((5,8), FSharp.Compiler.Xml.XmlDocCollector),
                                  SynValData
                                    (Some
                                       { IsInstance = true
                                         IsDispatchSlot = false
                                         IsOverrideOrExplicitImpl = true
                                         IsFinal = false
                                         GetterOrSetterIsCompilerGenerated =
                                          false
                                         MemberKind = Member },
                                     SynValInfo
                                       ([[SynArgInfo ([], false, None)]; []],
                                        SynArgInfo ([], false, None)), None),
                                  LongIdent
                                    (SynLongIdent
                                       ([this; P1], [(5,21--5,22)], [None; None]),
                                     None, None, Pats [], None, (5,17--5,24)),
                                  None, Const (Int32 1, (5,27--5,28)),
                                  (5,17--5,24), NoneAtInvisible,
                                  { LeadingKeyword = Override (5,8--5,16)
                                    InlineKeyword = None
                                    EqualsRange = Some (5,25--5,26) }),
                               (5,8--5,28))], (4,4--5,28));
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((7,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                                 ([this; P2], [(7,15--7,16)], [None; None]),
                               None, None, Pats [], None, (7,11--7,18)), None,
                            Const (Int32 1, (7,21--7,22)), (7,11--7,18),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (7,4--7,10)
                              InlineKeyword = None
                              EqualsRange = Some (7,19--7,20) }), (7,4--7,22))],
                     (4,4--7,22)), [], None, (3,5--7,22),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--7,22))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,22), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
