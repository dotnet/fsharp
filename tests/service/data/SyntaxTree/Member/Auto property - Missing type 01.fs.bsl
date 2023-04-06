ImplFile
  (ParsedImplFileInput
     ("/root/Member/Auto property - Missing type 01.fs", false,
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
                     [ImplicitCtor
                        (None, [], SimplePats ([], [], (3,6--3,8)), None,
                         PreXmlDoc ((3,6), FSharp.Compiler.Xml.XmlDocCollector),
                         (3,5--3,6), { AsKeyword = None });
                      AutoProperty
                        ([], false, P1, Some (FromParseError (4,18--4,18)),
                         Member, { IsInstance = true
                                   IsDispatchSlot = false
                                   IsOverrideOrExplicitImpl = false
                                   IsFinal = false
                                   GetterOrSetterIsCompilerGenerated = false
                                   MemberKind = Member },
                         { IsInstance = true
                           IsDispatchSlot = false
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertySet },
                         PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                         None, ArbitraryAfterError ("autoProp2", (4,18--4,18)),
                         (4,4--4,18),
                         { LeadingKeyword =
                            MemberVal ((4,4--4,10), (4,11--4,14))
                           WithKeyword = None
                           EqualsRange = None
                           GetSetKeywords = None });
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
                            Const (Int32 1, (6,21--6,22)), (6,11--6,18),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (6,4--6,10)
                              InlineKeyword = None
                              EqualsRange = Some (6,19--6,20) }), (6,4--6,22))],
                     (4,4--6,22)), [],
                  Some
                    (ImplicitCtor
                       (None, [], SimplePats ([], [], (3,6--3,8)), None,
                        PreXmlDoc ((3,6), FSharp.Compiler.Xml.XmlDocCollector),
                        (3,5--3,6), { AsKeyword = None })), (3,5--6,22),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,9--3,10)
                    WithKeyword = None })], (3,0--6,22))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,22), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,19)-(6,4) parse error Incomplete structured construct at or before this point in member definition
(4,19)-(6,4) parse error Expecting member body
