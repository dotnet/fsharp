ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Object - Class 14.fs", false, QualifiedNameOfFile Module,
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
                        PreXmlDoc ((4,5), FSharp.Compiler.Xml.XmlDocCollector),
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
                        Named (SynIdent (this, None), false, None, (4,12--4,16)),
                        None, ArbitraryAfterError ("memberCore1", (4,16--4,16)),
                        (4,12--4,16), NoneAtInvisible,
                        { LeadingKeyword = Member (4,5--4,11)
                          InlineKeyword = None
                          EqualsRange = None }), (4,5--4,16));
                  Member
                    (SynBinding
                       (None, Normal, false, false, [],
                        PreXmlDoc ((5,5), FSharp.Compiler.Xml.XmlDocCollector),
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
                          (SynLongIdent
                             ([this; P2], [(5,16--5,17)], [None; None]), None,
                           None, Pats [], None, (5,12--5,19)), None,
                        Const (Int32 2, (5,22--5,23)), (5,12--5,19),
                        NoneAtInvisible, { LeadingKeyword = Member (5,5--5,11)
                                           InlineKeyword = None
                                           EqualsRange = Some (5,20--5,21) }),
                     (5,5--5,23))], [], (3,2--3,9), (3,0--5,25)), (3,0--5,25));
           Expr (Const (Unit, (7,0--7,2)), (7,0--7,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(4,17)-(5,5) parse error Expecting member body
