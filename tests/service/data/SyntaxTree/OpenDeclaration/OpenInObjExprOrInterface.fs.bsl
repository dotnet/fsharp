ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/OpenInObjExprOrInterface.fs", false,
      QualifiedNameOfFile OpenInObjExprOrInterface, [],
      [SynModuleOrNamespace
         ([OpenInObjExprOrInterface], false, AnonModule,
          [Expr
             (ObjExpr
                (LongIdent
                   (SynLongIdent
                      ([System; IDisposable], [(1,12--1,13)], [None; None])),
                 None, Some (1,25--1,29), [],
                 [Member
                    (SynBinding
                       (None, Normal, false, false, [],
                        PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                        SynValData
                          (Some { IsInstance = true
                                  IsDispatchSlot = false
                                  IsOverrideOrExplicitImpl = true
                                  IsFinal = false
                                  GetterOrSetterIsCompilerGenerated = false
                                  MemberKind = Member },
                           SynValInfo
                             ([[SynArgInfo ([], false, None)];
                               [SynArgInfo ([], false, None)]],
                              SynArgInfo ([], false, None)), None),
                        LongIdent
                          (SynLongIdent ([_; F], [(3,12--3,13)], [None; None]),
                           None, None, Pats [Wild (3,15--3,16)], None,
                           (3,11--3,16)), None, Const (Int32 3, (3,19--3,20)),
                        (3,11--3,16), NoneAtInvisible,
                        { LeadingKeyword = Member (3,4--3,10)
                          InlineKeyword = None
                          EqualsRange = Some (3,17--3,18) }), (3,4--3,20))], [],
                 (1,2--1,24), (1,0--4,1)), (1,0--4,1));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [A],
                     PreXmlDoc ((6,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (6,5--6,6)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (6,6--6,8)), None,
                         PreXmlDoc ((6,6), FSharp.Compiler.Xml.XmlDocCollector),
                         (6,5--6,6), { AsKeyword = None });
                      Interface
                        (LongIdent
                           (SynLongIdent
                              ([System; IDisposable], [(7,20--7,21)],
                               [None; None])), Some (7,33--7,37),
                         Some
                           [Member
                              (SynBinding
                                 (None, Normal, false, false, [],
                                  PreXmlDoc ((9,8), FSharp.Compiler.Xml.XmlDocCollector),
                                  SynValData
                                    (Some
                                       { IsInstance = true
                                         IsDispatchSlot = false
                                         IsOverrideOrExplicitImpl = true
                                         IsFinal = false
                                         GetterOrSetterIsCompilerGenerated =
                                          false
                                         MemberKind = Member },
                                     SynValInfo
                                       ([[SynArgInfo ([], false, None)];
                                         [SynArgInfo ([], false, None)]],
                                        SynArgInfo ([], false, None)), None),
                                  LongIdent
                                    (SynLongIdent
                                       ([_; F], [(9,16--9,17)], [None; None]),
                                     None, None, Pats [Wild (9,19--9,20)], None,
                                     (9,15--9,20)), None,
                                  Const (Int32 3, (9,23--9,24)), (9,15--9,20),
                                  NoneAtInvisible,
                                  { LeadingKeyword = Member (9,8--9,14)
                                    InlineKeyword = None
                                    EqualsRange = Some (9,21--9,22) }),
                               (9,8--9,24))], (7,4--9,24))], (7,4--9,24)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (6,6--6,8)), None,
                        PreXmlDoc ((6,6), FSharp.Compiler.Xml.XmlDocCollector),
                        (6,5--6,6), { AsKeyword = None })), (6,5--9,24),
                  { LeadingKeyword = Type (6,0--6,4)
                    EqualsRange = Some (6,9--6,10)
                    WithKeyword = None })], (6,0--9,24))], PreXmlDocEmpty, [],
          None, (1,0--10,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(2,4)-(2,8) parse error Unexpected keyword 'open' in object expression. Expected 'member', 'override', 'static' or other token.
(2,16)-(3,4) parse error Expecting member body
(8,8)-(8,12) parse error Unexpected keyword 'open' in member definition. Expected 'member', 'override', 'static' or other token.
(8,20)-(9,8) parse error Expecting member body
