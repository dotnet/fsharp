ImplFile
  (ParsedImplFileInput
     ("/root/Binding/TupleReturnTypeOfBindingShouldContainStars.fs", false,
      QualifiedNameOfFile TupleReturnTypeOfBindingShouldContainStars, [], [],
      [SynModuleOrNamespace
         ([TupleReturnTypeOfBindingShouldContainStars], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (a, None), false, None, (2,4--2,5)),
                  Some
                    (SynBindingReturnInfo
                       (Tuple
                          (false,
                           [Type (LongIdent (SynLongIdent ([int], [], [None])));
                            Star (2,12--2,13);
                            Type
                              (LongIdent (SynLongIdent ([string], [], [None])))],
                           (2,8--2,20)), (2,8--2,20), [],
                        { ColonRange = Some (2,6--2,7) })),
                  Typed
                    (App
                       (NonAtomic, false, Ident failwith,
                        Const
                          (String ("todo", Regular, (2,32--2,38)), (2,32--2,38)),
                        (2,23--2,38)),
                     Tuple
                       (false,
                        [Type (LongIdent (SynLongIdent ([int], [], [None])));
                         Star (2,12--2,13);
                         Type (LongIdent (SynLongIdent ([string], [], [None])))],
                        (2,8--2,20)), (2,23--2,38)), (2,4--2,5), Yes (2,0--2,38),
                  { LeadingKeyword = Let (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange = Some (2,21--2,22) })], (2,0--2,38));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (b, None), false, None, (3,4--3,5)),
                  Some
                    (SynBindingReturnInfo
                       (Tuple
                          (false,
                           [Type (LongIdent (SynLongIdent ([int], [], [None])));
                            Star (3,12--3,13);
                            Type
                              (LongIdent (SynLongIdent ([string], [], [None])));
                            Star (3,21--3,22);
                            Type (LongIdent (SynLongIdent ([bool], [], [None])))],
                           (3,8--3,27)), (3,8--3,27), [],
                        { ColonRange = Some (3,6--3,7) })),
                  Typed
                    (Tuple
                       (false,
                        [Const (Int32 1, (3,30--3,31));
                         Const
                           (String ("", Regular, (3,33--3,35)), (3,33--3,35));
                         Const (Bool false, (3,37--3,42))],
                        [(3,31--3,32); (3,35--3,36)], (3,30--3,42)),
                     Tuple
                       (false,
                        [Type (LongIdent (SynLongIdent ([int], [], [None])));
                         Star (3,12--3,13);
                         Type (LongIdent (SynLongIdent ([string], [], [None])));
                         Star (3,21--3,22);
                         Type (LongIdent (SynLongIdent ([bool], [], [None])))],
                        (3,8--3,27)), (3,30--3,42)), (3,4--3,5), Yes (3,0--3,42),
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,28--3,29) })], (3,0--3,42))],
          PreXmlDocEmpty, [], None, (2,0--4,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
