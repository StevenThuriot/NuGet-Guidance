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

using System;
using System.Globalization;

namespace NuGetGuidance.Domain
{
    internal class ExceptionLogMessage : LogMessage
    {
        public ExceptionLogMessage(string message, string exception)
            : base(message, LogLevel.Error)
        {
            Exception = exception;
        }

        public string Exception { get; private set; }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{0},{1} • Exception: [ {2} ]", base.ToString(), Environment.NewLine, Exception);
        }
    }
}