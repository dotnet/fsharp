ImplFile
  (ParsedImplFileInput
     ("/root/Member/MemberMispelledToMeme.fs", false,
      QualifiedNameOfFile MemberMispelledToMeme, [],
      [SynModuleOrNamespace
         ([MemberMispelledToMeme], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Seq],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (1,5--1,8)),
                  ObjectModel
                    (Unspecified,
                     [Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((2,3), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = false
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo
                                 ([[SynArgInfo ([], false, Some average)];
                                   [SynArgInfo ([], false, Some x)]],
                                  SynArgInfo ([], false, None)), None),
                            LongIdent
                              (SynLongIdent ([meme], [], [None]), None, None,
                               Pats
                                 [Named
                                    (SynIdent (average, None), false, None,
                                     (2,15--2,22));
                                  Paren
                                    (Typed
                                       (Named
                                          (SynIdent (x, None), false, None,
                                           (2,24--2,25)),
                                        App
                                          (LongIdent
                                             (SynLongIdent ([seq], [], [None])),
                                           None,
                                           [LongIdent
                                              (SynLongIdent ([int], [], [None]))],
                                           [], None, true, (2,27--2,34)),
                                        (2,24--2,34)), (2,23--2,35))], None,
                               (2,10--2,35)), None, Ident x, (2,10--2,35),
                            NoneAtInvisible,
                            { LeadingKeyword = Static (2,3--2,9)
                              InlineKeyword = None
                              EqualsRange = Some (2,36--2,37) }), (2,3--2,39))],
                     (2,3--2,39)), [], None, (1,5--2,39),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = Some (1,9--1,10)
                    WithKeyword = None })], (1,0--2,39))], PreXmlDocEmpty, [],
          None, (1,0--2,39), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(2,3)-(2,9) parse error Incomplete declaration of a static construct. Use 'static let','static do','static member' or 'static val' for declaration.
