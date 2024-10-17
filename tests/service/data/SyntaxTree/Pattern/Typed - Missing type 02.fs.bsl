ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Typed - Missing type 02.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (i, None), false, None, (3,4--3,5)),
                  Some
                    (SynBindingReturnInfo
                       (FromParseError (3,6--3,6), (3,6--3,6), [],
                        { ColonRange = Some (3,5--3,6) })),
                  Typed
                    (Const (Int32 1, (3,9--3,10)), FromParseError (3,6--3,6),
                     (3,9--3,10)), (3,4--3,5), Yes (3,0--3,10),
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,7--3,8) })], (3,0--3,10));
           Expr (Const (Unit, (5,0--5,2)), (5,0--5,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,7)-(3,8) parse error Unexpected symbol '=' in binding
