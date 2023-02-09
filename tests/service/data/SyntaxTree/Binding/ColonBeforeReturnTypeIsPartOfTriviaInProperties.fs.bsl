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
                     false, None,
                     /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,5--2,6)),
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
                                    ([this; Y],
                                     [/root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,15--3,16)],
                                     [None; None]), Some get, None,
                                  Pats
                                    [Paren
                                       (Const
                                          (Unit,
                                           /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,26--3,28)),
                                        /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,26--3,28))],
                                  None,
                                  /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,23--3,28)),
                               Some
                                 (SynBindingReturnInfo
                                    (LongIdent
                                       (SynLongIdent ([int], [], [None])),
                                     /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,29--3,32),
                                     [],
                                     { ColonRange =
                                        Some
                                          /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,28--3,29) })),
                               Typed
                                 (Const
                                    (Int32 1,
                                     /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,35--3,36)),
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,35--3,36)),
                               /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,23--3,28),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  Member
                                    /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,4--3,10)
                                 InlineKeyword = None
                                 EqualsRange =
                                  Some
                                    /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,33--3,34) })),
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
                                    ([this; Y],
                                     [/root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,15--3,16)],
                                     [None; None]), Some set, None,
                                  Pats
                                    [Paren
                                       (Typed
                                          (Wild
                                             /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,46--3,47),
                                           LongIdent
                                             (SynLongIdent ([int], [], [None])),
                                           /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,46--3,51)),
                                        /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,45--3,52))],
                                  None,
                                  /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,41--3,52)),
                               Some
                                 (SynBindingReturnInfo
                                    (LongIdent
                                       (SynLongIdent ([unit], [], [None])),
                                     /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,53--3,57),
                                     [],
                                     { ColonRange =
                                        Some
                                          /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,52--3,53) })),
                               Typed
                                 (Const
                                    (Unit,
                                     /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,60--3,62)),
                                  LongIdent (SynLongIdent ([unit], [], [None])),
                                  /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,60--3,62)),
                               /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,41--3,52),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  Member
                                    /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,4--3,10)
                                 InlineKeyword = None
                                 EqualsRange =
                                  Some
                                    /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,58--3,59) })),
                         /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,4--3,62),
                         { InlineKeyword = None
                           WithKeyword =
                            /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,18--3,22)
                           GetKeyword =
                            Some
                              /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,23--3,26)
                           AndKeyword =
                            Some
                              /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,37--3,40)
                           SetKeyword =
                            Some
                              /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,41--3,44) })],
                     /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (3,4--3,62)),
                  [], None,
                  /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,5--3,62),
                  { LeadingKeyword =
                     Type
                       /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,0--2,4)
                    EqualsRange =
                     Some
                       /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,7--2,8)
                    WithKeyword = None })],
              /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,0--3,62))],
          PreXmlDocEmpty, [], None,
          /root/Binding/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,0--4,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))