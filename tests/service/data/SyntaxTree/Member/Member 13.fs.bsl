ImplFile
  (ParsedImplFileInput
     ("/root/Member/Member 13.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [A],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  ObjectModel
                    (Unspecified,
                     [Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = false
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo ([[]], SynArgInfo ([], false, None)),
                               None),
                            LongIdent
                              (SynLongIdent ([P], [], [None]), None, None,
                               Pats [], None, (4,18--4,19)),
                            Some
                              (SynBindingReturnInfo
                                 (FromParseError (4,20--4,20), (4,20--4,20), [],
                                  { ColonRange = Some (4,19--4,20) })),
                            Typed
                              (ArbitraryAfterError ("memberCore2", (4,20--4,20)),
                               FromParseError (4,20--4,20), (4,20--4,20)),
                            (4,18--4,19), NoneAtInvisible,
                            { LeadingKeyword =
                               StaticMember ((4,4--4,10), (4,11--4,17))
                              InlineKeyword = None
                              EqualsRange = None }), (4,4--4,20))], (4,4--4,20)),
                  [], None, (3,5--4,20), { LeadingKeyword = Type (3,0--3,4)
                                           EqualsRange = Some (3,7--3,8)
                                           WithKeyword = None })], (3,0--4,20));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [B],
                     PreXmlDoc ((6,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (6,5--6,6)),
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
                                  PreXmlDoc ((7,11), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (7,11--7,14), { LeadingKeyword = None
                                                        MutableKeyword = None })],
                            PreXmlDoc ((7,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (7,6--7,14), { BarRange = Some (7,4--7,5) })],
                        (7,4--7,14)), (7,4--7,14)), [], None, (6,5--7,14),
                  { LeadingKeyword = Type (6,0--6,4)
                    EqualsRange = Some (6,7--6,8)
                    WithKeyword = None })], (6,0--7,14));
           Expr (Const (Unit, (9,0--9,2)), (9,0--9,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--9,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(6,0)-(6,4) parse error Incomplete structured construct at or before this point in member definition
(6,0)-(6,4) parse error Expecting member body
