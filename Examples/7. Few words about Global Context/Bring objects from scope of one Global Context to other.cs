﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExamplesFramework;
using NiL.JS.Core;

namespace Examples._7.Few_words_about_Global_Context
{
    public sealed class Bring_objects_from_scope_of_one_Global_Context_to_other : Example
    {
        /*
         * You can bring objects (functions, constructors, values, something else)
         * from scope of one Global Context to other.
         * If you call function in some Global Context (the First)
         * from another Global Context (the Second), at short time (while function is executing)
         * in callee thread will be active Global Context, which is parent for Context of called function (Second).
         * All global variables and type proxies will be stored in the Second Global Context.
         * Full isolation.
         */

        public override void Run()
        {
            var firstGlobalContext = new GlobalContext("First global context");
            var secondGlobalContext = new GlobalContext("Second global context");
            
            var firstContext = new Context(firstGlobalContext);
            var secondContext = new Context(secondGlobalContext);

            firstContext.Eval(@"
function DefineGlobalVariable(name, value) {
    (function() { return this })()[name] = value;
}
");

            firstContext.Eval(@"
DefineGlobalVariable('someName', 'someValue');
console.log(typeof someName); // Output: string
console.log(this.someName);   // Output: someValue
");

            secondContext.DefineVariable("DefineGlobalVariable")
                .Assign(firstContext.GetVariable("DefineGlobalVariable"));

            secondContext.Eval(@"
DefineGlobalVariable('someName', 'someValue');
console.log(typeof someName); // Output: undefined
console.log(this.someName);   // Output: undefined
");
        }
    }
}
