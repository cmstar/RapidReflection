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
    }
}