using System;

namespace Lucky {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class IsNotDataSchemaAttribute : Attribute {
        public IsNotDataSchemaAttribute() { }
    }
}
