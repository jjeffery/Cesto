#region License

// Copyright 2004-2013 John Jeffery <john@jeffery.id.au>
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Text.RegularExpressions;

namespace Cesto.Config
{
    /// <summary>
    /// Extensions for reading/writing <see cref="TimeSpan"/> values to readable strings.
    /// Seems useful enough, but not quite ready to make public API yet.
    /// </summary>
    internal static class TimeSpanExtensions
    {
        public static string ToReadableString(this TimeSpan @this)
        {
            // For zero timespan, choose seconds as the unit
            if (@this == TimeSpan.Zero)
            {
                return "0s";
            }

            if (@this.Milliseconds != 0)
            {
                return @this.TotalMilliseconds + "ms";
            }

            if (@this.Seconds != 0)
            {
                return @this.TotalSeconds + "s";
            }

            if (@this.Minutes != 0)
            {
                return @this.TotalMinutes + "m";
            }

            if (@this.Hours != 0)
            {
                return @this.TotalHours + "h";
            }

            return @this.TotalDays + "d";
        }

        public static bool TryParse(string s, out TimeSpan result)
        {
            var regex = new Regex(@"^(\s*(\d+)\s*([a-z]+)\s*,?\s*)+$", RegexOptions.IgnoreCase);
            var match = regex.Match(s);
            if (!match.Success)
            {
                result = TimeSpan.Zero;
                return false;
            }

            TimeSpan total = TimeSpan.Zero;

            // Group 0 = entire expression
            // Group 1 = outer parenthesis expression (repeated digits+units)
            // Group 2 = digits
            // Group 3 = units
            var digitsGroup = match.Groups[2];
            var unitsGroup = match.Groups[3];
            var captureCount = digitsGroup.Captures.Count;

            for (int captureNum = 0; captureNum < captureCount; ++captureNum)
            {
                var value = int.Parse(digitsGroup.Captures[captureNum].Value);
                var units = unitsGroup.Captures[captureNum].Value;

                switch (units)
                {
                    case "d":
                    case "day":
                    case "days":
                        total += TimeSpan.FromDays(value);
                        break;
                    case "h":
                    case "hour":
                    case "hours":
                    case "hr":
                    case "hrs":
                        total += TimeSpan.FromHours(value);
                        break;
                    case "m":
                    case "min":
                    case "mins":
                    case "minute":
                    case "minutes":
                        total += TimeSpan.FromMinutes(value);
                        break;
                    case "s":
                    case "sec":
                    case "secs":
                    case "second":
                    case "seconds":
                        total += TimeSpan.FromSeconds(value);
                        break;
                    case "ms":
                    case "msec":
                    case "msecs":
                    case "millisecond":
                    case "milliseconds":
                        total += TimeSpan.FromMilliseconds(value);
                        break;

                    default:
                        result = default(TimeSpan);
                        return false;
                }
            }
            result = total;
            return true;
        }
    }
}