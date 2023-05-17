ImplFile
  (ParsedImplFileInput
     ("/root/OperatorName/ActivePatternIdentifierInPrivateMember.fs", false,
      QualifiedNameOfFile ActivePatternIdentifierInPrivateMember, [], [],
      [SynModuleOrNamespace
         ([ActivePatternIdentifierInPrivateMember], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [A],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,6)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], SimplePats ([], [], (2,6--2,8)), None,
                         PreXmlDoc ((2,6), FSharp.Compiler.Xml.XmlDocCollector),
                         (2,5--2,6), { AsKeyword = None });
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                                 ([_; |A'|], [(3,20--3,21)],
                                  [None;
                                   Some
                                     (HasParenthesis ((3,21--3,22), (5,5--5,6)))]),
                               None, None, Pats [], Some (Private (3,11--3,18)),
                               (3,11--5,6)), None,
                            LongIdent
                              (false,
                               SynLongIdent
                                 ([|Lazy|], [],
                                  [Some
                                     (HasParenthesis ((5,9--5,10), (7,5--7,6)))]),
                               None, (5,9--7,6)), (3,11--5,6), NoneAtInvisible,
                            { LeadingKeyword = Member (3,4--3,10)
                              InlineKeyword = None
                              EqualsRange = Some (5,7--5,8) }), (3,4--7,6))],
                     (3,4--7,6)), [],
                  Some
                    (ImplicitCtor
                       (None, [], SimplePats ([], [], (2,6--2,8)), None,
                        PreXmlDoc ((2,6), FSharp.Compiler.Xml.XmlDocCollector),
                        (2,5--2,6), { AsKeyword = None })), (2,5--7,6),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,9--2,10)
                    WithKeyword = None })], (2,0--7,6))], PreXmlDocEmpty, [],
          None, (2,0--8,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
