// ReSharper disable ConvertToAutoProperty
// ReSharper disable UnusedMember.Local
namespace cmstar.RapidReflection.Emit
{
    internal interface IInternalInterface
    {
        int PropWithNoSetter { get; }
        int PropWithNoGetter { set; }
        int PublicProp { get; set; }
        string StringProp { get; set; }
    }

    internal interface IInternalInterface2 : IInternalInterface
    {
        int PublicProp2 { get; set; }
    }

    internal class InternalClass : IInternalInterface
    {
        private int _intField;
        public int PublicProp { get { return _intField; } set { _intField = value; } }

        private string _stringField;
        public string StringProp { get { return _stringField; } set { _stringField = value; } }

        public int PropWithNoSetter { get { return 1112; } }
        public int PropWithNoGetter { set { } }

        int _g;
        public int PropWithPrivateGetter { private get { return _g; } set { _g = value; } }

        int _s;
        public int PropWithPrivateSetter { get { return _s; } private set { _s = value; } }

        public static int StaticField;
        public static int StaticProp { get; set; }
    }

    internal class InternalClass2 : InternalClass, IInternalInterface2
    {
        public int PublicProp2 { get; set; }
    }

    internal struct InternalStruct : IInternalInterface2
    {
        private int _intField;
        public int PublicProp { get { return _intField; } set { _intField = value; } }

        private string _stringField;
        public string StringProp { get { return _stringField; } set { _stringField = value; } }

        public int PropWithNoSetter { get { return 112; } }
        public int PropWithNoGetter { set { } }
        public int PublicProp2 { get; set; }

        int _g;
        public int PropWithPrivateGetter { private get { return _g; } set { _g = value; } }

        int _s;
        public int PropWithPrivateSetter { get { return _s; } private set { _s = value; } }

        public static int StaticField;
        public static int StaticProp { get; set; }
    }
}
