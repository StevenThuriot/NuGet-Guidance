using NuGetGuidance.Interfaces;

namespace NuGetGuidance.Domain
{
    internal class PromptResult : IPromptResult
    {
        public PromptResult(string question, string input)
            :this(question)
        {
            Input = input;
        }

        public PromptResult(string question)
        {
            Question = question;
            IsOpen = true;
        }

        public bool IsOpen { get; set; }

        private bool _Result;
        public bool Result
        {
            get { return _Result; }
            set
            {
                _Result = value;
                IsOpen = false;
            }
        }

        public string Question { get; set; }
        public string Input { get; set; }

        public static readonly PromptResult Empty = new PromptResult(string.Empty)
            {
                IsOpen = false,
                Result = false,
                Input = string.Empty
            };
    }
}
