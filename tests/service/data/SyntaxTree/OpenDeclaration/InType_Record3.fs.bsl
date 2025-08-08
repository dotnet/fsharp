ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/InType_Record3.fs", false,
      QualifiedNameOfFile InType_Record3, [],
      [SynModuleOrNamespace
         ([InType_Record3], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [ARecord],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (1,5--1,12)),
                  ObjectModel
                    (Unspecified,
                     [Open
                        (ModuleOrNamespace
                           (SynLongIdent ([System], [], [None]), (2,9--2,15)),
                         (2,4--2,15));
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
                                    ([_; RandomNumber], [(5,16--5,17)],
                                     [None; None]), Some get, None,
                                  Pats
                                    [Paren
                                       (Const (Unit, (5,38--5,40)), (5,38--5,40))],
                                  None, (5,35--5,40)), None,
                               App
                                 (Atomic, false,
                                  DotGet
                                    (App
                                       (Atomic, false, Ident Random,
                                        Const (Unit, (5,49--5,51)), (5,43--5,51)),
                                     (5,51--5,52),
                                     SynLongIdent ([Next], [], [None]),
                                     (5,43--5,56)), Const (Unit, (5,56--5,58)),
                                  (5,43--5,58)), (5,35--5,40), NoneAtInvisible,
                               { LeadingKeyword = Member (5,8--5,14)
                                 InlineKeyword = None
                                 EqualsRange = Some (5,41--5,42) })), None,
                         (3,4--5,58), { InlineKeyword = None
                                        WithKeyword = (5,30--5,34)
                                        GetKeyword = Some (5,35--5,38)
                                        AndKeyword = None
                                        SetKeyword = None })], (2,4--5,58)), [],
                  None, (1,5--5,58), { LeadingKeyword = Type (1,0--1,4)
                                       EqualsRange = Some (1,13--1,14)
                                       WithKeyword = None })], (1,0--5,58))],
          PreXmlDocEmpty, [], None, (1,0--6,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      WarnDirectives = []
                      CodeComments = [] }, set []))

(3,4)-(3,5) parse error Unexpected symbol '{' in member definition
(6,0)-(6,0) parse error Incomplete structured construct at or before this point in implementation file
