ImplFile
  (ParsedImplFileInput
     ("/root/SynTypeDefnWithMemberWithSetget.fs", false,
      QualifiedNameOfFile SynTypeDefnWithMemberWithSetget, [], [],
      [SynModuleOrNamespace
         ([SynTypeDefnWithMemberWithSetget], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [A],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/SynTypeDefnWithMemberWithSetget.fs (1,5--1,6)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [],
                         SimplePats
                           ([],
                            /root/SynTypeDefnWithMemberWithSetget.fs (1,6--1,8)),
                         None,
                         PreXmlDoc ((1,6), FSharp.Compiler.Xml.XmlDocCollector),
                         /root/SynTypeDefnWithMemberWithSetget.fs (1,5--1,6),
                         { AsKeyword = None });
                      GetSetMember
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
                                    ([this; Z],
                                     [/root/SynTypeDefnWithMemberWithSetget.fs (2,15--2,16)],
                                     [None; None]), Some get, None,
                                  Pats
                                    [Paren
                                       (Const
                                          (Unit,
                                           /root/SynTypeDefnWithMemberWithSetget.fs (2,52--2,54)),
                                        /root/SynTypeDefnWithMemberWithSetget.fs (2,52--2,54))],
                                  None,
                                  /root/SynTypeDefnWithMemberWithSetget.fs (2,49--2,54)),
                               Some
                                 (SynBindingReturnInfo
                                    (LongIdent
                                       (SynLongIdent ([int], [], [None])),
                                     /root/SynTypeDefnWithMemberWithSetget.fs (2,55--2,58),
                                     [],
                                     { ColonRange =
                                        Some
                                          /root/SynTypeDefnWithMemberWithSetget.fs (2,54--2,55) })),
                               Typed
                                 (Const
                                    (Int32 1,
                                     /root/SynTypeDefnWithMemberWithSetget.fs (2,61--2,62)),
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  /root/SynTypeDefnWithMemberWithSetget.fs (2,61--2,62)),
                               /root/SynTypeDefnWithMemberWithSetget.fs (2,49--2,54),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  Member
                                    /root/SynTypeDefnWithMemberWithSetget.fs (2,4--2,10)
                                 InlineKeyword = None
                                 EqualsRange =
                                  Some
                                    /root/SynTypeDefnWithMemberWithSetget.fs (2,59--2,60) })),
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
                                    ([this; Z],
                                     [/root/SynTypeDefnWithMemberWithSetget.fs (2,15--2,16)],
                                     [None; None]), Some set, None,
                                  Pats
                                    [Paren
                                       (Typed
                                          (Wild
                                             /root/SynTypeDefnWithMemberWithSetget.fs (2,28--2,29),
                                           LongIdent
                                             (SynLongIdent ([int], [], [None])),
                                           /root/SynTypeDefnWithMemberWithSetget.fs (2,28--2,33)),
                                        /root/SynTypeDefnWithMemberWithSetget.fs (2,27--2,34))],
                                  None,
                                  /root/SynTypeDefnWithMemberWithSetget.fs (2,23--2,34)),
                               Some
                                 (SynBindingReturnInfo
                                    (LongIdent
                                       (SynLongIdent ([unit], [], [None])),
                                     /root/SynTypeDefnWithMemberWithSetget.fs (2,35--2,39),
                                     [],
                                     { ColonRange =
                                        Some
                                          /root/SynTypeDefnWithMemberWithSetget.fs (2,34--2,35) })),
                               Typed
                                 (Const
                                    (Unit,
                                     /root/SynTypeDefnWithMemberWithSetget.fs (2,42--2,44)),
                                  LongIdent (SynLongIdent ([unit], [], [None])),
                                  /root/SynTypeDefnWithMemberWithSetget.fs (2,42--2,44)),
                               /root/SynTypeDefnWithMemberWithSetget.fs (2,23--2,34),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  Member
                                    /root/SynTypeDefnWithMemberWithSetget.fs (2,4--2,10)
                                 InlineKeyword = None
                                 EqualsRange =
                                  Some
                                    /root/SynTypeDefnWithMemberWithSetget.fs (2,40--2,41) })),
                         /root/SynTypeDefnWithMemberWithSetget.fs (2,4--2,62),
                         { InlineKeyword = None
                           WithKeyword =
                            /root/SynTypeDefnWithMemberWithSetget.fs (2,18--2,22)
                           GetKeyword =
                            Some
                              /root/SynTypeDefnWithMemberWithSetget.fs (2,49--2,52)
                           AndKeyword =
                            Some
                              /root/SynTypeDefnWithMemberWithSetget.fs (2,45--2,48)
                           SetKeyword =
                            Some
                              /root/SynTypeDefnWithMemberWithSetget.fs (2,23--2,26) })],
                     /root/SynTypeDefnWithMemberWithSetget.fs (2,4--2,62)), [],
                  Some
                    (ImplicitCtor
                       (None, [],
                        SimplePats
                          ([],
                           /root/SynTypeDefnWithMemberWithSetget.fs (1,6--1,8)),
                        None,
                        PreXmlDoc ((1,6), FSharp.Compiler.Xml.XmlDocCollector),
                        /root/SynTypeDefnWithMemberWithSetget.fs (1,5--1,6),
                        { AsKeyword = None })),
                  /root/SynTypeDefnWithMemberWithSetget.fs (1,5--2,62),
                  { LeadingKeyword =
                     Type /root/SynTypeDefnWithMemberWithSetget.fs (1,0--1,4)
                    EqualsRange =
                     Some /root/SynTypeDefnWithMemberWithSetget.fs (1,9--1,10)
                    WithKeyword = None })],
              /root/SynTypeDefnWithMemberWithSetget.fs (1,0--2,62))],
          PreXmlDocEmpty, [], None,
          /root/SynTypeDefnWithMemberWithSetget.fs (1,0--2,62),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))