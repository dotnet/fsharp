ImplFile
  (ParsedImplFileInput
     ("/root/OperatorName/ActivePatternExceptionAnd 07.fs", false,
      QualifiedNameOfFile ActivePatternExceptionAnd 07, [], [],
      [SynModuleOrNamespace
         ([ActivePatternExceptionAnd 07], false, AnonModule,
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
                            Paren
                              (LongIdent
                                 (SynLongIdent ([MyExn], [], [None]), None, None,
                                  Pats [], None, (3,16--3,21)), (3,15--3,22))],
                           (3,5--3,22)), Wild (3,25--3,26), (3,5--3,26),
                        { BarRange = (3,23--3,24) }), (3,4--3,27)), None,
                  App
                    (NonAtomic, false, Ident exn, Const (Unit, (3,34--3,36)),
                     (3,30--3,36)), (3,4--3,27), Yes (3,0--3,36),
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,28--3,29) })], (3,0--3,36))],
          PreXmlDocEmpty, [], None, (1,0--3,36), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))

(1,0)-(2,0) parse warning The declarations in this file will be placed in an implicit module 'ActivePatternExceptionAnd 07' based on the file name 'ActivePatternExceptionAnd 07.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.
