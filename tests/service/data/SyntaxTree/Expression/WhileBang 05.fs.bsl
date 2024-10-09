ImplFile
  (ParsedImplFileInput
     ("/root/Expression/WhileBang 05.fs", false, QualifiedNameOfFile Module, [],
      [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Wild (3,4--3,5), None,
                  WhileBang
                    (Yes (4,4--4,32),
                     App
                       (NonAtomic, false, Ident async,
                        ComputationExpr
                          (false,
                           YieldOrReturn
                             ((false, true), Const (Bool true, (4,26--4,30)),
                              (4,19--4,30),
                              { YieldOrReturnKeyword = (4,19--4,25) }),
                           (4,17--4,32)), (4,11--4,32)),
                     Const (Int32 2, (5,8--5,9)), (4,4--5,9)), (3,4--3,5),
                  Yes (3,0--5,9), { LeadingKeyword = Let (3,0--3,3)
                                    InlineKeyword = None
                                    EqualsRange = Some (3,6--3,7) })],
              (3,0--5,9)); Expr (Const (Int32 3, (7,0--7,1)), (7,0--7,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
