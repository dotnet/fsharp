namespace basic
    public class BasicClass
       dim v as integer
       public property Prop as integer
          private get
              return v
          end get
          set(value as integer)
               v = value
          end set
       end property
    end class
end namespace