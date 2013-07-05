using System;
using System.Reflection;
using NUnit.Framework;

namespace cmstar.RapidReflection.Emit
{
    [TestFixture]
    public class FieldAccessorGeneratorTests
    {
        [Test]
        public void CreateGetterForClass()
        {
            var intField = typeof(InternalClass).GetField("_intField", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(intField);

            var intGetter = FieldAccessorGenerator.CreateGetter(intField);
            var c = new InternalClass2();
            intField.SetValue(c, 123);
            Assert.AreEqual(intField.GetValue(c), intGetter(c));

            var stringField = typeof(InternalClass).GetField("_stringField", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(stringField);

            var stringGetter = FieldAccessorGenerator.CreateGetter(stringField);
            stringField.SetValue(c, "str");
            Assert.AreEqual(stringField.GetValue(c), stringGetter(c));

            stringField.SetValue(c, null);
            Assert.AreEqual(null, stringGetter(c));
        }

        [Test]
        public void CreateGetterForStruct()
        {
            var intField = typeof(InternalStruct).GetField("_intField", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(intField);

            var intGetter = FieldAccessorGenerator.CreateGetter(intField);
            var s = new InternalStruct();
            intField.SetValue(s, 1231);
            Assert.AreEqual(intField.GetValue(s), intGetter(s));

            var stringField = typeof(InternalStruct).GetField("_stringField", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(stringField);

            var stringGetter = FieldAccessorGenerator.CreateGetter(stringField);
            stringField.SetValue(s, "str1");
            Assert.AreEqual(stringField.GetValue(s), stringGetter(s));

            stringField.SetValue(s, null);
            Assert.AreEqual(null, stringGetter(s));
        }

        [Test]
        public void CreateGetterForStaticFields()
        {
            var fieldFromClass = typeof(InternalClass).GetField("StaticField", BindingFlags.Static | BindingFlags.Public);
            Assert.NotNull(fieldFromClass);
            var getterForClass = FieldAccessorGenerator.CreateGetter(fieldFromClass);
            InternalClass.StaticField = 1234;
            Assert.AreEqual(InternalClass.StaticField, getterForClass(null));

            var fieldFromStruct = typeof(InternalStruct).GetField("StaticField", BindingFlags.Static | BindingFlags.Public);
            Assert.NotNull(fieldFromStruct);
            var getterForStruct = FieldAccessorGenerator.CreateGetter(fieldFromStruct);
            InternalStruct.StaticField = 4321;
            Assert.AreEqual(InternalStruct.StaticField, getterForStruct(null));
        }

        [Test]
        public void CreateSetterForClass()
        {
            var intField = typeof(InternalClass).GetField("_intField", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(intField);

            var intSetter = FieldAccessorGenerator.CreateSetter(intField);
            var c = new InternalClass2();
            Assert.Throws<InvalidCastException>(() => intSetter(c, "123"));
            intSetter(c, 1234);
            Assert.AreEqual(1234, intField.GetValue(c));

            var stringField = typeof(InternalClass).GetField("_stringField", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(stringField);

            var stringSetter = FieldAccessorGenerator.CreateSetter(stringField);
            Assert.Throws<InvalidCastException>(() => stringSetter(c, 123));
            stringSetter(c, "string");
            Assert.AreEqual("string", stringField.GetValue(c));

            stringSetter(c, null);
            Assert.AreEqual(null, stringField.GetValue(c));
        }

        [Test]
        public void CreateSetterForStruct()
        {
            var intField = typeof(InternalStruct).GetField("_intField", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(intField);

            var intSetter = FieldAccessorGenerator.CreateSetter(intField);
            object s = new InternalStruct();
            Assert.Throws<InvalidCastException>(() => intSetter(s, "123"));
            intSetter(s, 876);
            Assert.AreEqual(876, intField.GetValue(s));

            var stringField = typeof(InternalStruct).GetField("_stringField", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(stringField);

            var stringSetter = FieldAccessorGenerator.CreateSetter(stringField);
            Assert.Throws<InvalidCastException>(() => stringSetter(s, 123));
            stringSetter(s, "str1");
            Assert.AreEqual("str1", stringField.GetValue(s));

            stringSetter(s, null);
            Assert.AreEqual(null, stringField.GetValue(s));
        }

        [Test]
        public void CreateGenericGetterThrowsException()
        {
            var fieldFromClass = typeof(InternalClass).GetField("_intField", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.Throws<ArgumentException>(() => FieldAccessorGenerator.CreateGetter<IInternalInterface, int>(fieldFromClass));
            Assert.Throws<ArgumentException>(() => FieldAccessorGenerator.CreateGetter<IInternalInterface, string>(fieldFromClass));
            Assert.Throws<ArgumentException>(() => FieldAccessorGenerator.CreateGetter<InternalStruct, int>(fieldFromClass));

            var fieldFromStruct = typeof(InternalStruct).GetField("_intField", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.Throws<ArgumentException>(() => FieldAccessorGenerator.CreateGetter<IInternalInterface, int>(fieldFromStruct));
            Assert.Throws<ArgumentException>(() => FieldAccessorGenerator.CreateGetter<InternalClass, int>(fieldFromStruct));
            Assert.Throws<ArgumentException>(() => FieldAccessorGenerator.CreateGetter<int, int>(fieldFromStruct));
        }

        [Test]
        public void CreateGenericGetterForClass()
        {
            var intField = typeof(InternalClass).GetField("_intField", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(intField);

            var intGetter = FieldAccessorGenerator.CreateGetter<InternalClass2, int>(intField);
            var c = new InternalClass2();
            intField.SetValue(c, 123);
            Assert.AreEqual(intField.GetValue(c), intGetter(c));

            var intGetter2 = FieldAccessorGenerator.CreateGetter<InternalClass, object>(intField);
            intField.SetValue(c, 1232);
            Assert.AreEqual(intField.GetValue(c), intGetter2(c));

            var intGetter3 = FieldAccessorGenerator.CreateGetter<object, int>(intField);
            intField.SetValue(c, 1233);
            Assert.AreEqual(intField.GetValue(c), intGetter3(c));

            var stringField = typeof(InternalClass).GetField("_stringField", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(stringField);

            var stringGetter = FieldAccessorGenerator.CreateGetter<InternalClass, string>(stringField);
            stringField.SetValue(c, "str");
            Assert.AreEqual(stringField.GetValue(c), stringGetter(c));

            stringField.SetValue(c, null);
            Assert.AreEqual(null, stringGetter(c));

            var stringGetter2 = FieldAccessorGenerator.CreateGetter<InternalClass2, object>(stringField);
            stringField.SetValue(c, "str2");
            Assert.AreEqual(stringField.GetValue(c), stringGetter2(c));

            var stringGetter3 = FieldAccessorGenerator.CreateGetter<object, string>(stringField);
            stringField.SetValue(c, "str3");
            Assert.AreEqual(stringField.GetValue(c), stringGetter3(c));
        }

        [Test]
        public void CreateGenericGetterForStruct()
        {
            var intField = typeof(InternalStruct).GetField("_intField", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(intField);

            var intGetter = FieldAccessorGenerator.CreateGetter<InternalStruct, int>(intField);
            var s = new InternalStruct();
            intField.SetValue(s, 1231);
            Assert.AreEqual(intField.GetValue(s), intGetter(s));

            var intGetter2 = FieldAccessorGenerator.CreateGetter<object, int>(intField);
            intField.SetValue(s, 1232);
            Assert.AreEqual(intField.GetValue(s), intGetter2(s));

            var intGetter3 = FieldAccessorGenerator.CreateGetter<InternalStruct, object>(intField);
            intField.SetValue(s, 1233);
            Assert.AreEqual(intField.GetValue(s), intGetter3(s));

            var stringField = typeof(InternalStruct).GetField("_stringField", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(stringField);

            var stringGetter = FieldAccessorGenerator.CreateGetter<InternalStruct, string>(stringField);
            stringField.SetValue(s, "str1");
            Assert.AreEqual(stringField.GetValue(s), stringGetter(s));

            stringField.SetValue(s, null);
            Assert.AreEqual(null, stringGetter(s));

            var stringGetter2 = FieldAccessorGenerator.CreateGetter<InternalStruct, object>(stringField);
            stringField.SetValue(s, "str2");
            Assert.AreEqual(stringField.GetValue(s), stringGetter2(s));

            var stringGetter3 = FieldAccessorGenerator.CreateGetter<object, string>(stringField);
            stringField.SetValue(s, "str3");
            Assert.AreEqual(stringField.GetValue(s), stringGetter3(s));
        }

        [Test]
        public void CreateGenericSetterThrowsException()
        {
            var fieldFromClass = typeof(InternalClass).GetField("_intField", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(fieldFromClass);

            Assert.Throws<ArgumentException>(() => FieldAccessorGenerator.CreateSetter<IInternalInterface, int>(fieldFromClass));
            Assert.Throws<ArgumentException>(() => FieldAccessorGenerator.CreateSetter<IInternalInterface, string>(fieldFromClass));
            Assert.Throws<ArgumentException>(() => FieldAccessorGenerator.CreateSetter<InternalStruct, int>(fieldFromClass));

            var fieldFromStruct = typeof(InternalStruct).GetField("_intField", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(fieldFromStruct);

            Assert.Throws<ArgumentException>(() => FieldAccessorGenerator.CreateSetter<IInternalInterface, int>(fieldFromStruct));
            Assert.Throws<ArgumentException>(() => FieldAccessorGenerator.CreateSetter<InternalClass, int>(fieldFromStruct));
            Assert.Throws<ArgumentException>(() => FieldAccessorGenerator.CreateSetter<int, int>(fieldFromStruct));
        }

        [Test]
        public void CreateGenericSetterForClass()
        {
            var intField = typeof(InternalClass).GetField("_intField", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(intField);

            var intSetter = FieldAccessorGenerator.CreateSetter<InternalClass2, int>(intField);
            var c = new InternalClass2();
            intSetter(c, 1234);
            Assert.AreEqual(1234, intField.GetValue(c));

            var intSetter2 = FieldAccessorGenerator.CreateSetter<InternalClass2, object>(intField);
            intSetter2(c, 12342);
            Assert.AreEqual(12342, intField.GetValue(c));

            var intSetter3 = FieldAccessorGenerator.CreateSetter<object, int>(intField);
            intSetter3(c, 123423);
            Assert.AreEqual(123423, intField.GetValue(c));

            var stringField = typeof(InternalClass).GetField("_stringField", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(stringField);

            var stringSetter = FieldAccessorGenerator.CreateSetter<InternalClass, string>(stringField);
            stringSetter(c, "string");
            Assert.AreEqual("string", stringField.GetValue(c));

            stringSetter(c, null);
            Assert.AreEqual(null, stringField.GetValue(c));
        }

        [Test]
        public void CreateGenericSetterForStruct()
        {
            var intField = typeof(InternalStruct).GetField("_intField", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(intField);

            var intSetter = FieldAccessorGenerator.CreateSetter<object, int>(intField);
            object s = new InternalStruct();
            intSetter(s, 876);
            Assert.AreEqual(876, intField.GetValue(s));

            var intSetter2 = FieldAccessorGenerator.CreateSetter<object, object>(intField);
            intSetter2(s, 8762);
            Assert.AreEqual(8762, intField.GetValue(s));

            var stringField = typeof(InternalStruct).GetField("_stringField", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(stringField);

            var stringSetter = FieldAccessorGenerator.CreateSetter<object, string>(stringField);
            stringSetter(s, "str1");
            Assert.AreEqual("str1", stringField.GetValue(s));

            stringSetter(s, null);
            Assert.AreEqual(null, stringField.GetValue(s));

            var stringSetter2 = FieldAccessorGenerator.CreateSetter<object, object>(stringField);
            stringSetter2(s, "str12");
            Assert.AreEqual("str12", stringField.GetValue(s));
        }
    }
}