ImplFile
  (ParsedImplFileInput
     ("/root/Binding/RangeOfEqualSignShouldBePresentInMemberBindingWithReturnType.fs",
      false,
      QualifiedNameOfFile
        RangeOfEqualSignShouldBePresentInMemberBindingWithReturnType, [], [],
      [SynModuleOrNamespace
         ([RangeOfEqualSignShouldBePresentInMemberBindingWithReturnType], false,
          AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
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
                                 ([this; Y], [(3,15--3,16)], [None; None]), None,
                               None,
                               Pats
                                 [Paren
                                    (Const (Unit, (3,18--3,20)), (3,18--3,20))],
                               None, (3,11--3,20)),
                            Some
                              (SynBindingReturnInfo
                                 (LongIdent
                                    (SynLongIdent ([string], [], [None])),
                                  (3,23--3,29), [],
                                  { ColonRange = Some (3,21--3,22) })),
                            Typed
                              (Ident z,
                               LongIdent (SynLongIdent ([string], [], [None])),
                               (3,32--3,33)), (3,11--3,20), NoneAtInvisible,
                            { LeadingKeyword = Member (3,4--3,10)
                              InlineKeyword = None
                              EqualsRange = Some (3,30--3,31) }), (3,4--3,33))],
                     (3,4--3,33)), [],
                  Some
                    (ImplicitCtor
                       (None, [], SimplePats ([], [], (2,6--2,8)), None,
                        PreXmlDoc ((2,6), FSharp.Compiler.Xml.XmlDocCollector),
                        (2,5--2,6), { AsKeyword = None })), (2,5--3,33),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,9--2,10)
                    WithKeyword = None })], (2,0--3,33))], PreXmlDocEmpty, [],
          None, (2,0--4,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
