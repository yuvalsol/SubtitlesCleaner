using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace System
{
    public static partial class ExceptionExtensions
    {
        public static string GetFormattedStackTrace(this Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            bool isFoundNamespaceNotSystem = false;

            StackTrace st = new StackTrace(ex, true);
            for (int i = 0; i < st.FrameCount; i++)
            {
                StackFrame sf = st.GetFrame(i);
                if (sf != null)
                {
                    MethodBase method = sf.GetMethod();
                    if (method != null)
                    {
                        Type reflectedType = method.ReflectedType;
                        if (reflectedType != null)
                        {
                            if (isFoundNamespaceNotSystem == false || reflectedType.Namespace.StartsWith("System") == false)
                            {
                                isFoundNamespaceNotSystem = reflectedType.Namespace.StartsWith("System") == false;

                                MethodInfo mi = method as MethodInfo;
                                if (mi != null)
                                {
                                    sb.Append(mi.GetSignature());

                                    int lineNumber = sf.GetFileLineNumber();
                                    if (lineNumber > 0)
                                    {
                                        sb.Append(" at line ");
                                        sb.Append(lineNumber);
                                    }

                                    sb.Append("\n");
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return sb.ToString();
        }
    }
}
