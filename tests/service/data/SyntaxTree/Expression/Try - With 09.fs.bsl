ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Try - With 09.fs", false, QualifiedNameOfFile Module, [],
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
                  TryWith
                    (Const (Int32 1, (4,8--4,9)), [], (4,4--5,8), Yes (4,4--4,7),
                     Yes (5,4--5,8), { TryKeyword = (4,4--4,7)
                                       TryToWithRange = (4,4--5,8)
                                       WithKeyword = (5,4--5,8)
                                       WithToEndRange = (5,4--5,8) }),
                  (3,4--3,5), NoneAtLet, { LeadingKeyword = Let (3,0--3,3)
                                           InlineKeyword = None
                                           EqualsRange = Some (3,6--3,7) })],
              (3,0--5,8)); Expr (Const (Int32 3, (7,0--7,1)), (7,0--7,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(7,0)-(7,1) parse error Incomplete structured construct at or before this point in expression
