ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/InObjExpr2.fs", false,
      QualifiedNameOfFile InObjExpr2, [],
      [SynModuleOrNamespace
         ([InObjExpr2], false, AnonModule,
          [Expr
             (ObjExpr
                (LongIdent
                   (SynLongIdent
                      ([System; IDisposable], [(1,12--1,13)], [None; None])),
                 None, Some (1,25--1,29), [],
                 [Member
                    (SynBinding
                       (None, Normal, false, false, [],
                        PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                          (SynLongIdent ([_; F], [(2,12--2,13)], [None; None]),
                           None, None, Pats [Wild (2,15--2,16)], None,
                           (2,11--2,16)), None, Const (Int32 3, (2,19--2,20)),
                        (2,11--2,16), NoneAtInvisible,
                        { LeadingKeyword = Member (2,4--2,10)
                          InlineKeyword = None
                          EqualsRange = Some (2,17--2,18) }), (2,4--2,20))], [],
                 (1,2--1,24), (1,0--4,1)), (1,0--4,1))], PreXmlDocEmpty, [],
          None, (1,0--4,1), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(3,4)-(3,8) parse error Unexpected keyword 'open' in object expression. Expected 'member', 'override', 'static' or other token.
(4,0)-(4,1) parse error Expecting member body
