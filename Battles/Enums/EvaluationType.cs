using System;

namespace Battles.Enums
{
    public static class EvaluationType
    {
        public static EvaluationT GetType(string r)
        {
            if (!String.IsNullOrEmpty(r))
            {
                r = r.ToLower().Replace(" ", "");
                if (r == "complete")
                    return EvaluationT.Complete;
                else if (r == "flag")
                    return EvaluationT.Flag;
            }
            return EvaluationT.Complete;
        }

        public static string GetType(EvaluationT e)
        {
            if (e == EvaluationT.Complete)
                return "Complete";
            else if (e == EvaluationT.Flag)
                return "Flag";
            return "Complete";
        }
    }

    public enum EvaluationT
    {
        Complete,
        Flag
    }
}
