ImplFile
  (ParsedImplFileInput
     ("/root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs", false,
      QualifiedNameOfFile ColonBeforeReturnTypeIsPartOfTriviaInProperties, [],
      [],
      [SynModuleOrNamespace
         ([ColonBeforeReturnTypeIsPartOfTriviaInProperties], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,6)),
                  ObjectModel
                    (Unspecified,
                     [GetSetMember
                        (Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
                               SynValData
                                 (Some
                                    { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = PropertyGet },
                                  SynValInfo
                                    ([[SynArgInfo ([], false, None)]; []],
                                     SynArgInfo ([], false, None)), None),
                               LongIdent
                                 (SynLongIdent
                                    ([this; Y], [(3,15--3,16)], [None; None]),
                                  Some get, None,
                                  Pats
                                    [Paren
                                       (Const (Unit, (3,26--3,28)), (3,26--3,28))],
                                  None, (3,23--3,28)),
                               Some
                                 (SynBindingReturnInfo
                                    (LongIdent
                                       (SynLongIdent ([int], [], [None])),
                                     (3,29--3,32), [],
                                     { ColonRange = Some (3,28--3,29) })),
                               Typed
                                 (Const (Int32 1, (3,35--3,36)),
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  (3,35--3,36)), (3,23--3,28), NoneAtInvisible,
                               { LeadingKeyword = Member (3,4--3,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (3,33--3,34) })),
                         Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
                               SynValData
                                 (Some
                                    { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = PropertySet },
                                  SynValInfo
                                    ([[SynArgInfo ([], false, None)];
                                      [SynArgInfo ([], false, None)]],
                                     SynArgInfo ([], false, None)), None),
                               LongIdent
                                 (SynLongIdent
                                    ([this; Y], [(3,15--3,16)], [None; None]),
                                  Some set, None,
                                  Pats
                                    [Paren
                                       (Typed
                                          (Wild (3,46--3,47),
                                           LongIdent
                                             (SynLongIdent ([int], [], [None])),
                                           (3,46--3,51)), (3,45--3,52))], None,
                                  (3,41--3,52)),
                               Some
                                 (SynBindingReturnInfo
                                    (LongIdent
                                       (SynLongIdent ([unit], [], [None])),
                                     (3,53--3,57), [],
                                     { ColonRange = Some (3,52--3,53) })),
                               Typed
                                 (Const (Unit, (3,60--3,62)),
                                  LongIdent (SynLongIdent ([unit], [], [None])),
                                  (3,60--3,62)), (3,41--3,52), NoneAtInvisible,
                               { LeadingKeyword = Member (3,4--3,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (3,58--3,59) })),
                         (3,4--3,62), { InlineKeyword = None
                                        WithKeyword = (3,18--3,22)
                                        GetKeyword = Some (3,23--3,26)
                                        AndKeyword = Some (3,37--3,40)
                                        SetKeyword = Some (3,41--3,44) })],
                     (3,4--3,62)), [], None, (2,5--3,62),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,7--2,8)
                    WithKeyword = None })], (2,0--3,62))], PreXmlDocEmpty, [],
          None, (2,0--4,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
