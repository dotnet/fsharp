ImplFile
  (ParsedImplFileInput
     ("/root/Member/Interface - Missing type 04.fs", false,
      QualifiedNameOfFile Module, [], [],
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
                           (None, Normal, false, false, [],
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
                                 ([this; P1], [(4,15--4,16)], [None; None]),
                               None, None, Pats [], None, (4,11--4,18)), None,
                            Const (Int32 1, (4,21--4,22)), (4,11--4,18),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (4,4--4,10)
                              InlineKeyword = None
                              EqualsRange = Some (4,19--4,20) }), (4,4--4,22));
                      Interface
                        (FromParseError (5,13--5,13), None, None, (5,4--5,13));
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
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
                                 ([this; P2], [(6,15--6,16)], [None; None]),
                               None, None, Pats [], None, (6,11--6,18)), None,
                            Const (Int32 2, (6,21--6,22)), (6,11--6,18),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (6,4--6,10)
                              InlineKeyword = None
                              EqualsRange = Some (6,19--6,20) }), (6,4--6,22))],
                     (4,4--6,22)), [], None, (3,5--6,22),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--6,22));
           Expr (Const (Int32 3, (8,0--8,1)), (8,0--8,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--8,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(5,14)-(6,4) parse error Incomplete structured construct at or before this point in member definition
