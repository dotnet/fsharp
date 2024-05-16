ImplFile
  (ParsedImplFileInput
     ("/root/OperatorName/ActivePatternExceptionAnd 05.fs", false,
      QualifiedNameOfFile ActivePatternExceptionAnd 05, [], [],
      [SynModuleOrNamespace
         ([ActivePatternExceptionAnd 05], false, AnonModule,
          [Exception
             (SynExceptionDefn
                (SynExceptionDefnRepr
                   ([],
                    SynUnionCase
                      ([], SynIdent (MyExn, None), Fields [], PreXmlDocEmpty,
                       None, (1,10--1,15), { BarRange = None }), None,
                    PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), None,
                    (1,0--1,15)), None, [], (1,0--1,15)), (1,0--1,15));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Paren
                    (Or
                       (Ands
                          ([Typed
                              (Wild (3,5--3,6),
                               LongIdent (SynLongIdent ([exn], [], [None])),
                               (3,5--3,12));
                            LongIdent
                              (SynLongIdent ([MyExn], [], [None]), None, None,
                               Pats [], None, (3,15--3,20))], (3,5--3,20)),
                        Wild (3,23--3,24), (3,5--3,24),
                        { BarRange = (3,21--3,22) }), (3,4--3,25)), None,
                  App
                    (NonAtomic, false, Ident exn, Const (Unit, (3,32--3,34)),
                     (3,28--3,34)), (3,4--3,25), Yes (3,0--3,34),
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,26--3,27) })], (3,0--3,34))],
          PreXmlDocEmpty, [], None, (1,0--3,34), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))

(1,0)-(2,0) parse warning The declarations in this file will be placed in an implicit module 'ActivePatternExceptionAnd 05' based on the file name 'ActivePatternExceptionAnd 05.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.
