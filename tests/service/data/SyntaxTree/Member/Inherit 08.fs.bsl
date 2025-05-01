ImplFile
  (ParsedImplFileInput
     ("/root/Member/Inherit 08.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  ObjectModel
                    (Unspecified,
                     [Inherit
                        (None, None, (4,4--4,11),
                         { InheritKeyword = (4,4--4,11) });
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((4,12), FSharp.Compiler.Xml.XmlDocCollector),
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
                                 ([this; P], [(6,15--6,16)], [None; None]), None,
                               None, Pats [], None, (6,11--6,17)), None,
                            Const (Int32 1, (6,20--6,21)), (6,11--6,17),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (6,4--6,10)
                              InlineKeyword = None
                              EqualsRange = Some (6,18--6,19) }), (4,12--6,21))],
                     (4,4--6,21)), [], None, (3,5--6,21),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--6,21))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,21), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,12)-(4,14) parse error Unexpected keyword 'as' in type definition
