ImplFile
  (ParsedImplFileInput
     ("/root/Expression/If 09.fs", false, QualifiedNameOfFile Module, [], [],
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
                  IfThenElse
                    (Const (Unit, (4,7--4,9)), Const (Unit, (4,15--4,17)),
                     Some (Const (Unit, (5,4--5,6))), Yes (4,4--4,14), false,
                     (4,4--5,6), { IfKeyword = (4,4--4,6)
                                   IsElif = false
                                   ThenKeyword = (4,10--4,14)
                                   ElseKeyword = Some (4,18--4,22)
                                   IfToThenRange = (4,4--4,14) }), (3,4--3,5),
                  NoneAtLet, { LeadingKeyword = Let (3,0--3,3)
                               InlineKeyword = None
                               EqualsRange = Some (3,6--3,7) })], (3,0--5,6));
           Expr (Const (Unit, (7,0--7,2)), (7,0--7,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
