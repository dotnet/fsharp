ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Typed - Missing type 03.fs", false,
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
                  Paren
                    (As
                       (Paren
                          (Typed
                             (Named
                                (SynIdent (i, None), false, None, (3,6--3,7)),
                              FromParseError (3,8--3,8), (3,6--3,8)), (3,5--3,9)),
                        Wild (3,13--3,14), (3,5--3,14)), (3,4--3,15)), None,
                  Const (Unit, (3,18--3,20)), (3,4--3,15), Yes (3,0--3,20),
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,16--3,17) })], (3,0--3,20))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,20), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,8)-(3,9) parse error Unexpected symbol ')' in pattern
