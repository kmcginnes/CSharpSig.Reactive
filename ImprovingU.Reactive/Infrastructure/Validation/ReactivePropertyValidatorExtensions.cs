using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ImprovingU.Reactive.Infrastructure.Validation
{
    public static class ReactivePropertyValidatorExtensions
    {
        public static ReactivePropertyValidator<string> IfMatch(this ReactivePropertyValidator<string> This, string pattern, string errorMessage)
        {
            var regex = new Regex(pattern);

            return This.IfTrue(regex.IsMatch, errorMessage);
        }

        public static ReactivePropertyValidator<string> IfNotMatch(this ReactivePropertyValidator<string> This, string pattern, string errorMessage)
        {
            var regex = new Regex(pattern);

            return This.IfFalse(regex.IsMatch, errorMessage);
        }

        public static ReactivePropertyValidator<string> IfNullOrEmpty(this ReactivePropertyValidator<string> This, string errorMessage)
        {
            return This.IfTrue(String.IsNullOrEmpty, errorMessage);
        }

        public static ReactivePropertyValidator<string> IfNotUri(this ReactivePropertyValidator<string> This, string errorMessage)
        {
            return This.IfFalse(s =>
            {
                Uri uri;
                return Uri.TryCreate(s, UriKind.Absolute, out uri);
            }, errorMessage);
        }
        
        public static ReactivePropertyValidator<string> IfContainsInvalidPathChars(this ReactivePropertyValidator<string> This, string errorMessage)
        {
            return This.IfTrue(str =>
            {
                // easiest check to make
                if (StringExtensions.ContainsAny(str, Path.GetInvalidPathChars()))
                {
                    return true;
                }

                string driveLetter;

                try
                {
                    // if for whatever reason you don't have an absolute path
                    // hopefully you've remembered to use `IfPathNotRooted`
                    // in your validator
                    driveLetter = Path.GetPathRoot(str);
                }
                catch (PathTooLongException)
                {
                    // when you pass in a path that is too long
                    // you're gonna have a bad time:
                    // - 260 characters for full path
                    // - 248 characters for directory name
                    return false;
                }
                catch (ArgumentException)
                {
                    // Path.GetPathRoot does some fun things
                    // around legal combinations of characters that we miss 
                    // by simply checking against an array of legal characters
                    return true;
                }

                if (driveLetter == null)
                {
                    return false;
                }

                // lastly, check each directory name doesn't contain
                // any invalid filename characters
                var foldersInPath = str.Substring(driveLetter.Length);
                return Enumerable.Any<string>(foldersInPath.Split(new[] { '\\', '/' }, StringSplitOptions.None), x => StringExtensions.ContainsAny(x, Path.GetInvalidFileNameChars()));
            }, errorMessage);
        }

        public static ReactivePropertyValidator<string> IfPathNotRooted(this ReactivePropertyValidator<string> This, string errorMessage)
        {
            return This.IfFalse(Path.IsPathRooted, errorMessage);
        }

        public static ReactivePropertyValidator<string> DirectoryExists(this ReactivePropertyValidator<string> This, string errorMessage)
        {
            return This.IfFalse(Directory.Exists, errorMessage);
        }

        public static ReactivePropertyValidator<string> IfUncPath(this ReactivePropertyValidator<string> This, string errorMessage)
        {
            return This.IfTrue(str => str.StartsWith(@"\\", StringComparison.Ordinal), errorMessage);
        }
    }
}