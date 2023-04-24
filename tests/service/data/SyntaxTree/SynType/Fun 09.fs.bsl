ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Fun 09.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Wild (3,4--3,5),
                  Some
                    (SynBindingReturnInfo
                       (Fun
                          (LongIdent (SynLongIdent ([a], [], [None])),
                           Fun
                             (LongIdent (SynLongIdent ([b], [], [None])),
                              FromParseError (3,16--3,16), (3,12--3,18),
                              { ArrowRange = (3,14--3,16) }), (3,7--3,18),
                           { ArrowRange = (3,9--3,11) }), (3,7--3,18), [],
                        { ColonRange = Some (3,5--3,6) })),
                  Typed
                    (Const (Unit, (3,19--3,21)),
                     Fun
                       (LongIdent (SynLongIdent ([a], [], [None])),
                        Fun
                          (LongIdent (SynLongIdent ([b], [], [None])),
                           FromParseError (3,16--3,16), (3,12--3,18),
                           { ArrowRange = (3,14--3,16) }), (3,7--3,18),
                        { ArrowRange = (3,9--3,11) }), (3,19--3,21)), (3,4--3,5),
                  Yes (3,0--3,21), { LeadingKeyword = Let (3,0--3,3)
                                     InlineKeyword = None
                                     EqualsRange = Some (3,17--3,18) })],
              (3,0--3,21))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,21), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,17)-(3,18) parse error Unexpected symbol '=' in binding
