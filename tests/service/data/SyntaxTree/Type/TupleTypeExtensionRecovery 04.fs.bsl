ImplFile
  (ParsedImplFileInput
     ("/root/Type/TupleTypeExtensionRecovery 04.fs", false,
      QualifiedNameOfFile TupleTypeExtensionRecovery 04, [],
      [SynModuleOrNamespace
         ([TupleTypeExtensionRecovery 04], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (1,6--1,9),
                     Some (Var (SynTypar (T1, None, false), (1,6--1,9)))),
                  ObjectModel (Augmentation (1,11--1,15), [], (1,5--2,23)),
                  [Member
                     (SynBinding
                        (None, Normal, false, false, [],
                         PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
                         SynValData
                           (Some { IsInstance = false
                                   IsDispatchSlot = false
                                   IsOverrideOrExplicitImpl = false
                                   IsFinal = false
                                   GetterOrSetterIsCompilerGenerated = false
                                   MemberKind = Member },
                            SynValInfo ([[]], SynArgInfo ([], false, None)),
                            None),
                         LongIdent
                           (SynLongIdent ([X], [], [None]), None, None, Pats [],
                            None, (2,18--2,19)), None,
                         Const (Int32 1, (2,22--2,23)), (2,18--2,19),
                         NoneAtInvisible,
                         { LeadingKeyword =
                            StaticMember ((2,4--2,10), (2,11--2,17))
                           InlineKeyword = None
                           EqualsRange = Some (2,20--2,21) }), (2,4--2,23))],
                  None, (1,5--2,23), { LeadingKeyword = Type (1,0--1,4)
                                       EqualsRange = None
                                       WithKeyword = None })], (1,0--2,23))],
          PreXmlDocEmpty, [], None, (1,0--3,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      WarnDirectives = []
                      CodeComments = [] }, set []))

(1,0)-(2,0) parse warning The declarations in this file will be placed in an implicit module 'TupleTypeExtensionRecovery 04' based on the file name 'TupleTypeExtensionRecovery 04.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.
