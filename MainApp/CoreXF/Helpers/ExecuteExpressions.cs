
using DynamicExpresso;
using System;
using System.Diagnostics;

namespace CoreXF
{
    public static class ExecuteExpressions
    {
        public static void ExecuteAction(string action, object[] contexts)
        {

            try
            {
                var interpreter = new Interpreter();
                var identifiers = interpreter.DetectIdentifiers(action);

                foreach (var context in contexts)
                {
                    var type = context?.GetType();
                    if (type == null)
                        continue;

                    foreach (var elm in identifiers.UnknownIdentifiers)
                    {
                        if (string.IsNullOrEmpty(elm))
                            continue;

                        var prop = type.GetProperty(elm);
                        if (prop != null)
                        {
                            interpreter.SetVariable(elm, prop.GetValue(context));
                            continue;
                        }
                    }
                }

                var f = interpreter.ParseAsDelegate<Action>(action);
                f.Invoke();

            }
            catch
            {
                Debug.Write($"ExecuteAction: action can't be executed {action}");
            }
        }

    }
}
