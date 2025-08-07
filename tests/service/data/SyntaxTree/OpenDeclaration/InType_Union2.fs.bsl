ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/InType_Union2.fs", false,
      QualifiedNameOfFile InType_Union2, [],
      [SynModuleOrNamespace
         ([InType_Union2], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [AUnion],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (1,5--1,11)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (A, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  false,
                                  PreXmlDoc ((1,21), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (1,21--1,24), { LeadingKeyword = None
                                                        MutableKeyword = None })],
                            PreXmlDoc ((1,14), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (1,16--1,24), { BarRange = Some (1,14--1,15) })],
                        (1,14--1,24)), (1,14--1,24)),
                  [GetSetMember
                     (Some
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlMerge
  (PreXmlDoc ((3,8), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
                            SynValData
                              (Some { IsInstance = true
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
                                 ([_; RandomNumber], [(3,16--3,17)],
                                  [None; None]), Some get, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (3,38--3,40)), (3,38--3,40))],
                               None, (3,35--3,40)), None,
                            App
                              (Atomic, false,
                               DotGet
                                 (App
                                    (Atomic, false, Ident Random,
                                     Const (Unit, (3,49--3,51)), (3,43--3,51)),
                                  (3,51--3,52),
                                  SynLongIdent ([Next], [], [None]),
                                  (3,43--3,56)), Const (Unit, (3,56--3,58)),
                               (3,43--3,58)), (3,35--3,40), NoneAtInvisible,
                            { LeadingKeyword = Member (3,8--3,14)
                              InlineKeyword = None
                              EqualsRange = Some (3,41--3,42) })), None,
                      (3,8--3,58), { InlineKeyword = None
                                     WithKeyword = (3,30--3,34)
                                     GetKeyword = Some (3,35--3,38)
                                     AndKeyword = None
                                     SetKeyword = None });
                   Open
                     (ModuleOrNamespace
                        (SynLongIdent ([System], [], [None]), (4,13--4,19)),
                      (4,8--4,19))], None, (1,5--4,19),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = Some (1,12--1,13)
                    WithKeyword = Some (2,4--2,8) })], (1,0--4,19))],
          PreXmlDocEmpty, [], None, (1,0--5,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      WarnDirectives = []
                      CodeComments = [] }, set []))
