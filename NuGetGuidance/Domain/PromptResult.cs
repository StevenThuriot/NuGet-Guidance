#region License

// 
//  Copyright 2012 Steven Thuriot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 

#endregion

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
