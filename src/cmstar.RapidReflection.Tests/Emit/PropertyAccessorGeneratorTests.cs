using System;
using NUnit.Framework;

namespace cmstar.RapidReflection.Emit
{
    [TestFixture]
    public class PropertyAccessorGeneratorTests
    {
        [Test]
        public void CreateGetterThrowsArgumentException()
        {
            var propWithNoGetter = typeof(IInternalInterface).GetProperty("PropWithNoGetter");
            Assert.Throws<ArgumentException>(
                () => PropertyAccessorGenerator.CreateGetter(propWithNoGetter, true));

            var propWithPrivateGetter = typeof(InternalClass).GetProperty("PropWithPrivateGetter");
            Assert.Throws<ArgumentException>(
                () => PropertyAccessorGenerator.CreateGetter(propWithPrivateGetter, false));
            Assert.Throws<ArgumentException>(
                () => PropertyAccessorGenerator.CreateGetter(propWithPrivateGetter, false));

            var prop = typeof(IInternalInterface).GetProperty("PublicProp");
            var getter = PropertyAccessorGenerator.CreateGetter(prop, false);
            Assert.Throws<InvalidCastException>(() => getter(new object()));
            Assert.Throws<NullReferenceException>(() => getter(null));
        }

        [Test]
        public void CreateGetterForClass()
        {
            var prop = typeof(IInternalInterface).GetProperty("PublicProp");
            var getter = PropertyAccessorGenerator.CreateGetter(prop, false);
            var c = new InternalClass { PublicProp = 321, StringProp = "some" };
            Assert.AreEqual(c.PublicProp, getter(c));

            var propWithPrivateGetter = typeof(InternalClass).GetProperty("PropWithPrivateGetter");
            var privateGetter = PropertyAccessorGenerator.CreateGetter(propWithPrivateGetter, true);
            Assert.AreEqual(propWithPrivateGetter.GetValue(c, null), privateGetter(c));

            var stringProp = typeof(InternalClass).GetProperty("StringProp");
            var stringGetter = PropertyAccessorGenerator.CreateGetter(stringProp, false);
            Assert.AreEqual(c.StringProp, stringGetter(c));

            c.StringProp = null;
            Assert.AreEqual(c.StringProp, stringGetter(c));
        }

        [Test]
        public void CreateGetterForStaticProperties()
        {
            var propOfClass = typeof(InternalClass).GetProperty("StaticProp");
            var getterForClass = PropertyAccessorGenerator.CreateGetter(propOfClass, false);
            InternalClass.StaticProp = 332;
            Assert.AreEqual(InternalClass.StaticProp, getterForClass(null));

            var propOfStruct = typeof(InternalStruct).GetProperty("StaticProp");
            var getterForStruct = PropertyAccessorGenerator.CreateGetter(propOfStruct, false);
            InternalStruct.StaticProp = 222;
            Assert.AreEqual(InternalStruct.StaticProp, getterForStruct(null));
        }

        [Test]
        public void CreateGetterForStruct()
        {
            var propFromInterface = typeof(IInternalInterface).GetProperty("PublicProp");
            var s = new InternalStruct { PublicProp = 321, StringProp = "some" };
            var getter = PropertyAccessorGenerator.CreateGetter(propFromInterface, false);
            Assert.AreEqual(s.PublicProp, getter(s));

            var propFromStruct = typeof(InternalStruct).GetProperty("PublicProp");
            var getter6 = PropertyAccessorGenerator.CreateGetter(propFromStruct, false);
            Assert.AreEqual(s.PublicProp, getter6(s));

            var stringProp = typeof(InternalStruct).GetProperty("StringProp");
            var stringGetter = PropertyAccessorGenerator.CreateGetter(stringProp, false);
            Assert.AreEqual(s.StringProp, stringGetter(s));

            s.StringProp = null;
            Assert.AreEqual(s.StringProp, stringGetter(s));
        }

        [Test]
        public void CreateSetterThrowsException()
        {
            var propWithNoGetter = typeof(IInternalInterface).GetProperty("PropWithNoSetter");
            Assert.Throws<ArgumentException>(
                () => PropertyAccessorGenerator.CreateSetter(propWithNoGetter, true));

            var propWithPrivateGetter = typeof(InternalClass).GetProperty("PropWithPrivateSetter");
            Assert.Throws<ArgumentException>(
                () => PropertyAccessorGenerator.CreateSetter(propWithPrivateGetter, false));
            Assert.Throws<ArgumentException>(
                () => PropertyAccessorGenerator.CreateSetter(propWithPrivateGetter, false));

            var prop = typeof(IInternalInterface).GetProperty("PublicProp");
            var setter = PropertyAccessorGenerator.CreateSetter(prop, false);
            Assert.Throws<InvalidCastException>(() => setter(new object(), 1));
            Assert.Throws<InvalidCastException>(() => setter(1, 1));
            Assert.Throws<NullReferenceException>(() => setter(null, 1));
        }

        [Test]
        public void CreateSetterForStaticProperties()
        {
            var propOfClass = typeof(InternalClass).GetProperty("StaticProp");
            var getterForClass = PropertyAccessorGenerator.CreateSetter(propOfClass, false);
            getterForClass(null, 12345);
            Assert.AreEqual(12345, InternalClass.StaticProp);

            var propOfStruct = typeof(InternalStruct).GetProperty("StaticProp");
            var getterForStruct = PropertyAccessorGenerator.CreateSetter(propOfStruct, false);
            getterForStruct(null, 54321);
            Assert.AreEqual(54321, InternalStruct.StaticProp);
        }

        [Test]
        public void CreateSetterForClass()
        {
            var prop = typeof(IInternalInterface).GetProperty("PublicProp");
            var setter = PropertyAccessorGenerator.CreateSetter(prop, false);
            var c = new InternalClass();
            setter(c, 123);
            Assert.AreEqual(123, c.PublicProp);

            var propWithPrivateSetter = typeof(InternalClass).GetProperty("PropWithPrivateSetter");
            var privateSetter = PropertyAccessorGenerator.CreateSetter(propWithPrivateSetter, true);
            privateSetter(c, 234);
            Assert.AreEqual(234, c.PropWithPrivateSetter);

            var stringProp = typeof(InternalClass).GetProperty("StringProp");
            var stringSetter = PropertyAccessorGenerator.CreateSetter(stringProp, false);
            stringSetter(c, "some string");
            Assert.AreEqual("some string", c.StringProp);

            stringSetter(c, null);
            Assert.AreEqual(null, c.StringProp);
        }

        [Test]
        public void CreateSetterForStruct()
        {
            var prop = typeof(IInternalInterface).GetProperty("PublicProp");
            var setter = PropertyAccessorGenerator.CreateSetter(prop, false);
            object s = new InternalStruct(); //the value type must be boxed
            setter(s, 123);
            Assert.AreEqual(123, ((InternalStruct)s).PublicProp);

            var propWithPrivateSetter = typeof(InternalStruct).GetProperty("PropWithPrivateSetter");
            var privateSetter = PropertyAccessorGenerator.CreateSetter(propWithPrivateSetter, true);
            privateSetter(s, 234);
            Assert.AreEqual(234, ((InternalStruct)s).PropWithPrivateSetter);

            var stringProp = typeof(InternalStruct).GetProperty("StringProp");
            var stringSetter = PropertyAccessorGenerator.CreateSetter(stringProp, false);
            stringSetter(s, "some string");
            Assert.AreEqual("some string", ((InternalStruct)s).StringProp);

            var objectValueSetter = PropertyAccessorGenerator.CreateSetter(prop, false);
            objectValueSetter(s, 321);
            Assert.AreEqual(321, ((InternalStruct)s).PublicProp);
        }
    }
}