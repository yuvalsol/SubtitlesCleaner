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
            bool isFoundNamespaceNotSystemWindowsForms = false;

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
                            if (reflectedType.Namespace != "System.Windows.Forms")
                            {
                                isFoundNamespaceNotSystemWindowsForms = true;

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
                            else if (isFoundNamespaceNotSystemWindowsForms)
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
