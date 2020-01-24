namespace basic
    public class BasicClass
       dim v as integer
       public writeonly property Prop as integer
          set(value as integer)
               v = value
          end set
       end property
    end class
end namespace