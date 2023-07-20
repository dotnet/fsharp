ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Typed - Missing type 04.fs", false,
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
                       (Typed
                          (Named (SynIdent (i, None), false, None, (3,5--3,6)),
                           FromParseError (3,7--3,7), (3,5--3,7)),
                        Wild (3,11--3,12), (3,5--3,12)), (3,4--3,13)), None,
                  Const (Unit, (3,16--3,18)), (3,4--3,13), Yes (3,0--3,18),
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,14--3,15) })], (3,0--3,18))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,18), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,8)-(3,10) parse error Unexpected keyword 'as' in pattern
