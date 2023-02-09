ImplFile
  (ParsedImplFileInput
     ("/root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs", false,
      QualifiedNameOfFile ColonBeforeReturnTypeIsPartOfTriviaInProperties, [],
      [],
      [SynModuleOrNamespace
         ([ColonBeforeReturnTypeIsPartOfTriviaInProperties], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (1,5--1,6)),
                  ObjectModel
                    (Unspecified,
                     [GetSetMember
                        (Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
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
                                     [/root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,15--2,16)],
                                     [None; None]), Some get, None,
                                  Pats
                                    [Paren
                                       (Const
                                          (Unit,
                                           /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,26--2,28)),
                                        /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,26--2,28))],
                                  None,
                                  /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,23--2,28)),
                               Some
                                 (SynBindingReturnInfo
                                    (LongIdent
                                       (SynLongIdent ([int], [], [None])),
                                     /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,29--2,32),
                                     [],
                                     { ColonRange =
                                        Some
                                          /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,28--2,29) })),
                               Typed
                                 (Const
                                    (Int32 1,
                                     /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,35--2,36)),
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,35--2,36)),
                               /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,23--2,28),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  Member
                                    /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,4--2,10)
                                 InlineKeyword = None
                                 EqualsRange =
                                  Some
                                    /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,33--2,34) })),
                         Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
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
                                     [/root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,15--2,16)],
                                     [None; None]), Some set, None,
                                  Pats
                                    [Paren
                                       (Typed
                                          (Wild
                                             /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,46--2,47),
                                           LongIdent
                                             (SynLongIdent ([int], [], [None])),
                                           /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,46--2,51)),
                                        /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,45--2,52))],
                                  None,
                                  /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,41--2,52)),
                               Some
                                 (SynBindingReturnInfo
                                    (LongIdent
                                       (SynLongIdent ([unit], [], [None])),
                                     /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,53--2,57),
                                     [],
                                     { ColonRange =
                                        Some
                                          /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,52--2,53) })),
                               Typed
                                 (Const
                                    (Unit,
                                     /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,60--2,62)),
                                  LongIdent (SynLongIdent ([unit], [], [None])),
                                  /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,60--2,62)),
                               /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,41--2,52),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  Member
                                    /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,4--2,10)
                                 InlineKeyword = None
                                 EqualsRange =
                                  Some
                                    /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,58--2,59) })),
                         /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,4--2,62),
                         { InlineKeyword = None
                           WithKeyword =
                            /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,18--2,22)
                           GetKeyword =
                            Some
                              /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,23--2,26)
                           AndKeyword =
                            Some
                              /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,37--2,40)
                           SetKeyword =
                            Some
                              /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,41--2,44) })],
                     /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (2,4--2,62)),
                  [], None,
                  /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (1,5--2,62),
                  { LeadingKeyword =
                     Type
                       /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (1,0--1,4)
                    EqualsRange =
                     Some
                       /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (1,7--1,8)
                    WithKeyword = None })],
              /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (1,0--2,62))],
          PreXmlDocEmpty, [], None,
          /root/ColonBeforeReturnTypeIsPartOfTriviaInProperties.fs (1,0--2,62),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))