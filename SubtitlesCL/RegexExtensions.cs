namespace System.Text.RegularExpressions
{
    public static partial class RegexExtensions
    {
        public static string ReplaceGroup(this Regex regex, string input, string groupName, string replacement)
        {
            return regex.Replace(
                input,
                match =>
                {
                    Group group = match.Groups[groupName];
                    if (group.Success)
                    {
                        StringBuilder sb = new StringBuilder();

                        int previousCaptureEnd = 0;
                        foreach (Capture capture in group.Captures)
                        {
                            int currentCaptureEnd = capture.Index + capture.Length - match.Index;
                            int currentCaptureLength = capture.Index - match.Index - previousCaptureEnd;
                            sb.Append(match.Value.Substring(previousCaptureEnd, currentCaptureLength));
                            sb.Append(replacement);
                            previousCaptureEnd = currentCaptureEnd;
                        }

                        sb.Append(match.Value.Substring(previousCaptureEnd));

                        return sb.ToString();
                    }
                    else
                    {
                        return match.Value;
                    }
                }
            );
        }

        public static string ReplaceGroup(this Regex regex, string input, int groupNum, string replacement)
        {
            return regex.Replace(
                input,
                match =>
                {
                    Group group = match.Groups[groupNum];
                    if (group.Success)
                    {
                        StringBuilder sb = new StringBuilder();

                        int previousCaptureEnd = 0;
                        foreach (Capture capture in group.Captures)
                        {
                            int currentCaptureEnd = capture.Index + capture.Length - match.Index;
                            int currentCaptureLength = capture.Index - match.Index - previousCaptureEnd;
                            sb.Append(match.Value.Substring(previousCaptureEnd, currentCaptureLength));
                            sb.Append(replacement);
                            previousCaptureEnd = currentCaptureEnd;
                        }

                        sb.Append(match.Value.Substring(previousCaptureEnd));

                        return sb.ToString();
                    }
                    else
                    {
                        return match.Value;
                    }
                }
            );
        }
    }
}
