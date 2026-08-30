using Scholar.Enums;

namespace Scholar.Common.Papers
{
    public static class SectionInstructions
    {
        public static string For(QuestionType type) => type switch
        {
            QuestionType.Mcq => "Choose the correct option.",
            QuestionType.Short => "Answer the following short questions.",
            QuestionType.Long => "Answer the following long questions.",
            _ => string.Empty
        };
    }
}
