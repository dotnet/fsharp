ImplFile
  (ParsedImplFileInput
     ("/root/OperatorName/ActivePatternAnd 01.fs", false,
      QualifiedNameOfFile ActivePatternAnd 01, [], [],
      [SynModuleOrNamespace
         ([ActivePatternAnd 01], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Paren
                    (Ands
                       ([Typed
                           (Wild (1,5--1,6),
                            LongIdent (SynLongIdent ([exn], [], [None])),
                            (1,5--1,12));
                         LongIdent
                           (SynLongIdent ([Failure], [], [None]), None, None,
                            Pats [Wild (1,23--1,24)], None, (1,15--1,24))],
                        (1,5--1,24)), (1,4--1,25)), None,
                  App
                    (NonAtomic, false, Ident exn, Const (Unit, (1,32--1,34)),
                     (1,28--1,34)), (1,4--1,25), Yes (1,0--1,34),
                  { LeadingKeyword = Let (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange = Some (1,26--1,27) })], (1,0--1,34))],
          PreXmlDocEmpty, [], None, (1,0--1,34), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))

(1,0)-(1,34) parse warning The declarations in this file will be placed in an implicit module 'ActivePatternAnd 01' based on the file name 'ActivePatternAnd 01.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.
