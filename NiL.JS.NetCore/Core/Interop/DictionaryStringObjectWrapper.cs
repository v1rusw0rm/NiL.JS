﻿using System.Collections.Generic;
using System.Linq;

namespace NiL.JS.Core.Interop
{
    internal sealed class DictionaryStringObjectWrapper : JSObject
    {
        private readonly Dictionary<string, object> _target;

        private sealed class ValueWrapper : JSValue
        {
            private readonly string _key;
            private readonly DictionaryStringObjectWrapper _owner;

            public ValueWrapper(DictionaryStringObjectWrapper owner, string key)
            {
                _owner = owner;
                _key = key;
                _attributes |= JSValueAttributesInternal.Reassign;

                object value = null;
                if (owner._target.TryGetValue(key, out value))
                    base.Assign(Marshal(value));
            }

            public override void Assign(JSValue value)
            {
                _owner.SetProperty(_key, value, false);

                base.Assign(value);
            }
        }

        public DictionaryStringObjectWrapper(Dictionary<string, object> target)
        {
            _valueType = JSValueType.Object;
            _oValue = this;
            _target = target;
        }

        protected internal override JSValue GetProperty(JSValue key, bool forWrite, PropertyScope propertyScope)
        {
            if (key.ValueType == JSValueType.Symbol || propertyScope >= PropertyScope.Super)
                return base.GetProperty(key, forWrite, propertyScope);

            var keyString = key.ToString();

            if (!forWrite)
            {
                if (!_target.ContainsKey(keyString))
                    return undefined;

                return Marshal(_target[keyString]);
            }

            return new ValueWrapper(this, keyString);
        }

        protected internal override void SetProperty(JSValue key, JSValue value, PropertyScope propertyScope, bool throwOnError)
        {
            if (key.ValueType == JSValueType.Symbol || propertyScope >= PropertyScope.Super)
                base.SetProperty(key, value, propertyScope, throwOnError);

            _target[key.ToString()] = value.Value;
        }

        protected internal override bool DeleteProperty(JSValue key)
        {
            if (key.ValueType == JSValueType.Symbol)
                return base.DeleteProperty(key);

            return _target.Remove(key.ToString());
        }

        protected internal override IEnumerator<KeyValuePair<string, JSValue>> GetEnumerator(bool hideNonEnum, EnumerationMode enumeratorMode)
        {
            if (enumeratorMode == EnumerationMode.KeysOnly)
                return _target.Keys.Select(x => new KeyValuePair<string, JSValue>(x, null)).GetEnumerator();

            if (enumeratorMode == EnumerationMode.RequireValues)
                return _target.Select(x => new KeyValuePair<string, JSValue>(x.Key, Marshal(x.Value))).GetEnumerator();

            return _target.Select(x => new KeyValuePair<string, JSValue>(x.Key, new ValueWrapper(this, x.Key))).GetEnumerator();
        }
    }
}
