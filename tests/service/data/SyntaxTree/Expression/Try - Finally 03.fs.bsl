ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Try - Finally 03.fs", false, QualifiedNameOfFile Module,
      [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None,
                     None), Wild (3,4--3,5), None,
                  TryFinally
                    (Const (Int32 1, (4,8--4,9)), Const (Int32 2, (6,4--6,5)),
                     (4,4--6,5), Yes (4,4--4,7), Yes (5,4--5,11),
                     { TryKeyword = (4,4--4,7)
                       FinallyKeyword = (5,4--5,11) }), (3,4--3,5), NoneAtLet,
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,6--3,7) })], (3,0--6,5));
           Expr (Const (Int32 3, (8,0--8,1)), (8,0--8,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--8,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
