using System;
using System.Collections.Generic;

namespace ThingSharp.Utils
{
    public class ArgParser
    {
        private List<string> argList = new List<string>();

        public ArgParser(string[] args)
        {
            parse(args);
        }

        public void parse(string[] args)
        {
            if (args.Length > 0)
            {
                argList.AddRange(args);

                // make lower case
                argList = argList.ConvertAll(d => d.ToLower());
            }
        }

        public bool VerifyFormat()
        {
            bool argFormatIsCorrect = true;

            if (argList.Count > 0)
            {
                string arg = argList.Find(a => (a.StartsWith("-") == false));

                if (!String.IsNullOrEmpty(arg))
                {
                    argFormatIsCorrect = false;
                }
            }

            return argFormatIsCorrect;
        }

        public bool exists(string argName)
        {
            bool foundArg = false;

            // find the argument pair
            string arg = argList.Find(a => a.Contains(argName.ToLower()));

            if (!String.IsNullOrEmpty(arg))
            {
                foundArg = true;
            }

            return foundArg;
        }

        public string getValue(string argName)
        {
            string argValue = String.Empty;

            // find the argument pair
            string arg = argList.Find(a => a.Contains(argName.ToLower()));

            if (!String.IsNullOrEmpty(arg))
            {
                // parse the values
                List<string> valuePair = new List<string>(arg.Split(':'));

                // If the count is not equal to 2, then there is an error with the arguments
                if (valuePair.Count == 2)
                {
                    argValue = valuePair[1];
                }
            }

            return argValue;
        }

        public string getArg(string argName)
        {
            // Returns the whole argument string, even the flag

            string argString = String.Empty;

            // find the argument pair
            string arg = argList.Find(a => a.Contains(argName.ToLower()));

            if (!String.IsNullOrEmpty(arg))
            {
                argString = arg;
            }

            return argString;
        }
    }
}