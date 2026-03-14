ImplFile
  (ParsedImplFileInput
     ("/root/WarnScope/IndentedWarnDirective.fs", false,
      QualifiedNameOfFile IndentedWarnDirective, [],
      [SynModuleOrNamespace
         ([IndentedWarnDirective], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (x, None), false, None, (1,4--1,5)), None,
                  Const (Unit, (3,4--3,6)), (1,4--1,5), Yes (1,0--3,6),
                  { LeadingKeyword = Let (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange = Some (1,6--1,7) })], (1,0--3,6),
              { InKeyword = None })], PreXmlDocEmpty, [], None, (1,0--4,0),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = [Nowarn (2,4--2,14)]
        CodeComments = [] }, set []))
