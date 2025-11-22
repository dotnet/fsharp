ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Object - Class 01.fs", false, QualifiedNameOfFile Module,
      [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (ObjExpr
                (LongIdent (SynLongIdent ([T], [], [None])),
                 Some (Const (Unit, (3,7--3,9)), None), Some (3,10--3,14), [],
                 [Member
                    (SynBinding
                       (None, Normal, false, false, [],
                        PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                        SynValData
                          (Some { IsInstance = true
                                  IsDispatchSlot = false
                                  IsOverrideOrExplicitImpl = true
                                  IsFinal = false
                                  GetterOrSetterIsCompilerGenerated = false
                                  MemberKind = Member },
                           SynValInfo
                             ([[SynArgInfo ([], false, None)]; []],
                              SynArgInfo ([], false, None)), None),
                        LongIdent
                          (SynLongIdent ([_; P1], [(4,14--4,15)], [None; None]),
                           None, None, Pats [], None, (4,13--4,17)), None,
                        Const (Int32 1, (4,20--4,21)), (4,13--4,17),
                        NoneAtInvisible, { LeadingKeyword = Override (4,4--4,12)
                                           InlineKeyword = None
                                           EqualsRange = Some (4,18--4,19) }),
                     (4,4--4,21))], [], (3,2--3,9), (3,0--4,23)), (3,0--4,23))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,23), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
