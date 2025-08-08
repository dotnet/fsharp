ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/InObjExpr.fs", false, QualifiedNameOfFile InObjExpr,
      [],
      [SynModuleOrNamespace
         ([InObjExpr], false, AnonModule,
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
                 (1,2--1,24), (1,0--4,1)), (1,0--4,1))], PreXmlDocEmpty, [],
          None, (1,0--4,1), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(2,4)-(2,8) parse error Unexpected keyword 'open' in object expression. Expected 'member', 'override', 'static' or other token.
(2,16)-(3,4) parse error Expecting member body
