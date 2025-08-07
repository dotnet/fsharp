ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/InType_Union.fs", false,
      QualifiedNameOfFile InType_Union, [],
      [SynModuleOrNamespace
         ([InType_Union], false, AnonModule,
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
                  [Open
                     (ModuleOrNamespace
                        (SynLongIdent ([System], [], [None]), (3,13--3,19)),
                      (3,8--3,19));
                   GetSetMember
                     (Some
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlMerge
  (PreXmlDoc ((4,8), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
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
                                 ([_; RandomNumber], [(4,16--4,17)],
                                  [None; None]), Some get, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (4,38--4,40)), (4,38--4,40))],
                               None, (4,35--4,40)), None,
                            App
                              (Atomic, false,
                               DotGet
                                 (App
                                    (Atomic, false, Ident Random,
                                     Const (Unit, (4,49--4,51)), (4,43--4,51)),
                                  (4,51--4,52),
                                  SynLongIdent ([Next], [], [None]),
                                  (4,43--4,56)), Const (Unit, (4,56--4,58)),
                               (4,43--4,58)), (4,35--4,40), NoneAtInvisible,
                            { LeadingKeyword = Member (4,8--4,14)
                              InlineKeyword = None
                              EqualsRange = Some (4,41--4,42) })), None,
                      (4,8--4,58), { InlineKeyword = None
                                     WithKeyword = (4,30--4,34)
                                     GetKeyword = Some (4,35--4,38)
                                     AndKeyword = None
                                     SetKeyword = None })], None, (1,5--4,58),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = Some (1,12--1,13)
                    WithKeyword = Some (2,4--2,8) })], (1,0--4,58))],
          PreXmlDocEmpty, [], None, (1,0--5,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      WarnDirectives = []
                      CodeComments = [] }, set []))
