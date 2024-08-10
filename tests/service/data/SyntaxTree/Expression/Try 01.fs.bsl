ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Try 01.fs", false, QualifiedNameOfFile Module, [], [],
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
                    (Const (Int32 1, (4,8--4,9)), [], (4,4--4,9), Yes (4,4--4,7),
                     Yes (4,9--4,9), { TryKeyword = (4,4--4,7)
                                       TryToWithRange = (4,4--4,9)
                                       WithKeyword = (4,9--4,9)
                                       WithToEndRange = (4,4--4,9) }),
                  (3,4--3,5), NoneAtLet, { LeadingKeyword = Let (3,0--3,3)
                                           InlineKeyword = None
                                           EqualsRange = Some (3,6--3,7) })],
              (3,0--4,9)); Expr (Const (Int32 2, (6,0--6,1)), (6,0--6,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(6,0)-(6,1) parse error Incomplete structured construct at or before this point in expression. Expected 'finally', 'with' or other token.
