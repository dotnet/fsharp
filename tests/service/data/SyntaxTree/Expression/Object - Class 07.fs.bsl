ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Object - Class 07.fs", false, QualifiedNameOfFile Module,
      [], [],
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
                        None, ArbitraryAfterError ("memberCore2", (4,16--4,16)),
                        (4,12--4,16), NoneAtInvisible,
                        { LeadingKeyword = Member (4,5--4,11)
                          InlineKeyword = None
                          EqualsRange = None }), (4,5--4,16))], [], (3,2--3,9),
                 (3,0--4,18)), (3,0--4,18));
           Expr (Const (Unit, (6,0--6,2)), (6,0--6,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,17)-(4,18) parse error Expecting member body
