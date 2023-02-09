ImplFile
  (ParsedImplFileInput
     ("/root/Member/SynTypeDefnWithMemberWithSetget.fs", false,
      QualifiedNameOfFile SynTypeDefnWithMemberWithSetget, [], [],
      [SynModuleOrNamespace
         ([SynTypeDefnWithMemberWithSetget], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [A],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/Member/SynTypeDefnWithMemberWithSetget.fs (2,5--2,6)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [],
                         SimplePats
                           ([],
                            /root/Member/SynTypeDefnWithMemberWithSetget.fs (2,6--2,8)),
                         None,
                         PreXmlDoc ((2,6), FSharp.Compiler.Xml.XmlDocCollector),
                         /root/Member/SynTypeDefnWithMemberWithSetget.fs (2,5--2,6),
                         { AsKeyword = None });
                      GetSetMember
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
                                    ([this; Z],
                                     [/root/Member/SynTypeDefnWithMemberWithSetget.fs (3,15--3,16)],
                                     [None; None]), Some get, None,
                                  Pats
                                    [Paren
                                       (Const
                                          (Unit,
                                           /root/Member/SynTypeDefnWithMemberWithSetget.fs (3,52--3,54)),
                                        /root/Member/SynTypeDefnWithMemberWithSetget.fs (3,52--3,54))],
                                  None,
                                  /root/Member/SynTypeDefnWithMemberWithSetget.fs (3,49--3,54)),
                               Some
                                 (SynBindingReturnInfo
                                    (LongIdent
                                       (SynLongIdent ([int], [], [None])),
                                     /root/Member/SynTypeDefnWithMemberWithSetget.fs (3,55--3,58),
                                     [],
                                     { ColonRange =
                                        Some
                                          /root/Member/SynTypeDefnWithMemberWithSetget.fs (3,54--3,55) })),
                               Typed
                                 (Const
                                    (Int32 1,
                                     /root/Member/SynTypeDefnWithMemberWithSetget.fs (3,61--3,62)),
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  /root/Member/SynTypeDefnWithMemberWithSetget.fs (3,61--3,62)),
                               /root/Member/SynTypeDefnWithMemberWithSetget.fs (3,49--3,54),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  Member
                                    /root/Member/SynTypeDefnWithMemberWithSetget.fs (3,4--3,10)
                                 InlineKeyword = None
                                 EqualsRange =
                                  Some
                                    /root/Member/SynTypeDefnWithMemberWithSetget.fs (3,59--3,60) })),
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
                                    ([this; Z],
                                     [/root/Member/SynTypeDefnWithMemberWithSetget.fs (3,15--3,16)],
                                     [None; None]), Some set, None,
                                  Pats
                                    [Paren
                                       (Typed
                                          (Wild
                                             /root/Member/SynTypeDefnWithMemberWithSetget.fs (3,28--3,29),
                                           LongIdent
                                             (SynLongIdent ([int], [], [None])),
                                           /root/Member/SynTypeDefnWithMemberWithSetget.fs (3,28--3,33)),
                                        /root/Member/SynTypeDefnWithMemberWithSetget.fs (3,27--3,34))],
                                  None,
                                  /root/Member/SynTypeDefnWithMemberWithSetget.fs (3,23--3,34)),
                               Some
                                 (SynBindingReturnInfo
                                    (LongIdent
                                       (SynLongIdent ([unit], [], [None])),
                                     /root/Member/SynTypeDefnWithMemberWithSetget.fs (3,35--3,39),
                                     [],
                                     { ColonRange =
                                        Some
                                          /root/Member/SynTypeDefnWithMemberWithSetget.fs (3,34--3,35) })),
                               Typed
                                 (Const
                                    (Unit,
                                     /root/Member/SynTypeDefnWithMemberWithSetget.fs (3,42--3,44)),
                                  LongIdent (SynLongIdent ([unit], [], [None])),
                                  /root/Member/SynTypeDefnWithMemberWithSetget.fs (3,42--3,44)),
                               /root/Member/SynTypeDefnWithMemberWithSetget.fs (3,23--3,34),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  Member
                                    /root/Member/SynTypeDefnWithMemberWithSetget.fs (3,4--3,10)
                                 InlineKeyword = None
                                 EqualsRange =
                                  Some
                                    /root/Member/SynTypeDefnWithMemberWithSetget.fs (3,40--3,41) })),
                         /root/Member/SynTypeDefnWithMemberWithSetget.fs (3,4--3,62),
                         { InlineKeyword = None
                           WithKeyword =
                            /root/Member/SynTypeDefnWithMemberWithSetget.fs (3,18--3,22)
                           GetKeyword =
                            Some
                              /root/Member/SynTypeDefnWithMemberWithSetget.fs (3,49--3,52)
                           AndKeyword =
                            Some
                              /root/Member/SynTypeDefnWithMemberWithSetget.fs (3,45--3,48)
                           SetKeyword =
                            Some
                              /root/Member/SynTypeDefnWithMemberWithSetget.fs (3,23--3,26) })],
                     /root/Member/SynTypeDefnWithMemberWithSetget.fs (3,4--3,62)),
                  [],
                  Some
                    (ImplicitCtor
                       (None, [],
                        SimplePats
                          ([],
                           /root/Member/SynTypeDefnWithMemberWithSetget.fs (2,6--2,8)),
                        None,
                        PreXmlDoc ((2,6), FSharp.Compiler.Xml.XmlDocCollector),
                        /root/Member/SynTypeDefnWithMemberWithSetget.fs (2,5--2,6),
                        { AsKeyword = None })),
                  /root/Member/SynTypeDefnWithMemberWithSetget.fs (2,5--3,62),
                  { LeadingKeyword =
                     Type
                       /root/Member/SynTypeDefnWithMemberWithSetget.fs (2,0--2,4)
                    EqualsRange =
                     Some
                       /root/Member/SynTypeDefnWithMemberWithSetget.fs (2,9--2,10)
                    WithKeyword = None })],
              /root/Member/SynTypeDefnWithMemberWithSetget.fs (2,0--3,62))],
          PreXmlDocEmpty, [], None,
          /root/Member/SynTypeDefnWithMemberWithSetget.fs (2,0--4,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))