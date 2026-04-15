ImplFile
  (ParsedImplFileInput
     ("/root/Expression/LetIn 07.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (LetOrUse
                { IsRecursive = false
                  Bindings =
                   [SynBinding
                      (None, Normal, false, false, [],
                       PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                       SynValData
                         (None, SynValInfo ([], SynArgInfo ([], false, None)),
                          None),
                       Named (SynIdent (x, None), false, None, (3,4--3,5)), None,
                       Const (Int32 1, (3,8--3,9)), (3,4--3,5), Yes (3,0--3,9),
                       { LeadingKeyword = Let (3,0--3,3)
                         InlineKeyword = None
                         EqualsRange = Some (3,6--3,7) })]
                  Body = Const (Unit, (3,13--3,15))
                  Range = (3,0--3,15)
                  Trivia = { InKeyword = Some (3,10--3,12) }
                  IsFromSource = true }, (3,0--3,15))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,15), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
