ImplFile
  (ParsedImplFileInput
     ("/root/ConditionalDirective/IfElifElseEndif.fs", false,
      QualifiedNameOfFile A, [],
      [SynModuleOrNamespace
         ([A], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((8,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (x, None), false, None, (8,4--8,5)), None,
                  Const (Int32 3, (8,8--8,9)), (8,4--8,5), Yes (8,0--8,9),
                  { LeadingKeyword = Let (8,0--8,3)
                    InlineKeyword = None
                    EqualsRange = Some (8,6--8,7) })], (8,0--8,9),
              { InKeyword = None })],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--8,9), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives =
         [If (Ident "DEBUG", (3,0--3,9)); Elif (Ident "RELEASE", (5,0--5,13));
          Else (7,0--7,5); EndIf (9,0--9,6)]
        WarnDirectives = []
        CodeComments = [] }, set []))
