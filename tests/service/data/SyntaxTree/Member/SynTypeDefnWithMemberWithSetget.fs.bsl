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
                     false, None, (2,5--2,6)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], SimplePats ([], (2,6--2,8)), None,
                         PreXmlDoc ((2,6), FSharp.Compiler.Xml.XmlDocCollector),
                         (2,5--2,6), { AsKeyword = None });
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
                                    ([this; Z], [(3,15--3,16)], [None; None]),
                                  Some get, None,
                                  Pats
                                    [Paren
                                       (Const (Unit, (3,52--3,54)), (3,52--3,54))],
                                  None, (3,49--3,54)),
                               Some
                                 (SynBindingReturnInfo
                                    (LongIdent
                                       (SynLongIdent ([int], [], [None])),
                                     (3,55--3,58), [],
                                     { ColonRange = Some (3,54--3,55) })),
                               Typed
                                 (Const (Int32 1, (3,61--3,62)),
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  (3,61--3,62)), (3,49--3,54), NoneAtInvisible,
                               { LeadingKeyword = Member (3,4--3,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (3,59--3,60) })),
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
                                    ([this; Z], [(3,15--3,16)], [None; None]),
                                  Some set, None,
                                  Pats
                                    [Paren
                                       (Typed
                                          (Wild (3,28--3,29),
                                           LongIdent
                                             (SynLongIdent ([int], [], [None])),
                                           (3,28--3,33)), (3,27--3,34))], None,
                                  (3,23--3,34)),
                               Some
                                 (SynBindingReturnInfo
                                    (LongIdent
                                       (SynLongIdent ([unit], [], [None])),
                                     (3,35--3,39), [],
                                     { ColonRange = Some (3,34--3,35) })),
                               Typed
                                 (Const (Unit, (3,42--3,44)),
                                  LongIdent (SynLongIdent ([unit], [], [None])),
                                  (3,42--3,44)), (3,23--3,34), NoneAtInvisible,
                               { LeadingKeyword = Member (3,4--3,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (3,40--3,41) })),
                         (3,4--3,62), { InlineKeyword = None
                                        WithKeyword = (3,18--3,22)
                                        GetKeyword = Some (3,49--3,52)
                                        AndKeyword = Some (3,45--3,48)
                                        SetKeyword = Some (3,23--3,26) })],
                     (3,4--3,62)), [],
                  Some
                    (ImplicitCtor
                       (None, [], SimplePats ([], (2,6--2,8)), None,
                        PreXmlDoc ((2,6), FSharp.Compiler.Xml.XmlDocCollector),
                        (2,5--2,6), { AsKeyword = None })), (2,5--3,62),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,9--2,10)
                    WithKeyword = None })], (2,0--3,62))], PreXmlDocEmpty, [],
          None, (2,0--4,0), { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
