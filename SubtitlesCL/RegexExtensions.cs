namespace System.Text.RegularExpressions
{
    public delegate bool IsMatchEvaluator(Match match);

    public static partial class RegexExtensions
    {
        public static string ReplaceGroup(this Regex regex, string input, string groupName, string replacement, IsMatchEvaluator evaluator = null)
        {
            return regex.Replace(
                input,
                match =>
                {
                    if (evaluator != null && evaluator(match) == false)
                        return match.Value;

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
    }
}
