using Microsoft.VisualStudio.TestTools.UnitTesting;
using NiL.JS.Core;
using System.Collections.Generic;

namespace FunctionalTests
{
    [TestClass]
    public class DictionaryStringObjectTests
    {
        [TestMethod]
        public void ReadPropertiesOfDictionary()
        {
            var obj = new Dictionary<string, object>();
            obj["field"] = "value";
            var context = new Context();
            context.DefineVariable("obj").Assign(JSValue.Marshal(obj));

            var value = context.Eval("obj.field");

            Assert.AreEqual(JSValueType.String, value.ValueType);
            Assert.AreEqual("value", value.Value);
        }

        [TestMethod]
        public void WritePropertiesOfDictionary()
        {
            var obj = new Dictionary<string, object>();
            var context = new Context();
            context.DefineVariable("obj").Assign(JSValue.Marshal(obj));

            var value = context.Eval("obj.field = 'value'");

            Assert.IsInstanceOfType(obj["field"], typeof(string));
            Assert.AreEqual("value", obj["field"]);
        }

        [TestMethod]
        public void WritePropertiesOfDictionaryOverWith()
        {
            var obj = new Dictionary<string, object>();
            obj["field"] = null;
            var context = new Context();
            context.DefineVariable("obj").Assign(JSValue.Marshal(obj));

            var value = context.Eval("with(obj) field = 'value'");

            Assert.IsInstanceOfType(obj["field"], typeof(string));
            Assert.AreEqual("value", obj["field"]);
        }

        [TestMethod]
        public void WriteInsideWithoShouldNotCreateNewField()
        {
            var obj = new Dictionary<string, object>();
            var context = new Context();
            context.DefineVariable("obj").Assign(JSValue.Marshal(obj));

            var value = context.Eval("with(obj) field = 'value'");

            Assert.IsFalse(obj.ContainsKey("field"));
        }
    }
}
