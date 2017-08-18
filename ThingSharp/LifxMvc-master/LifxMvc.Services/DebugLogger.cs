using System;
using System.Diagnostics;
using System.Reflection;

namespace Eric.Morrison.Logging
{
	public class DebugLogger
    {
        /// <summary>
        /// Logs the entrance of the method referenced by the specified MethodBase.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mb"></param>
        public void EnterMethod(MethodBase mb)
        {
            this.LogMethod(true, mb.ToStringEx(), null, null);
        }

        public void EnterMethod(MethodBase mb, string format, params object[] args)
        {
            this.LogMethod(true, mb.ToStringEx(), format, args);
        }

        public void EnterMethod(string methodText)
        {
            this.LogMethod(true, methodText, null, null);
        }

        public void EnterMethod(string methodText, string format, params object[] args)
        {
            this.LogMethod(true, methodText, format, args);
        }

        /// <summary>
        /// Logs the exit of the method referenced by the specified MethodBase.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mb"></param>
        public void ExitMethod(MethodBase mb)
        {
            this.LogMethod(false, mb.ToStringEx(), null, null);
        }

        public void ExitMethod(MethodBase mb, string format, params object[] args)
        {
            this.LogMethod(false, mb.ToStringEx(), format, args);
        }

        public void ExitMethod(string methodText)
        {
            this.LogMethod(false, methodText, null, null);
        }

        public void ExitMethod(string methodText, string format, params object[] args)
        {
            this.LogMethod(false, methodText, format, args);
        }

        public void LogMethod(bool entering, string methodText, string callerSuppliedFormat, params object[] callerSuppliedArgs)
        {
            try
            {
                var actionSymbol = "+";
                var actionText = "Enter";
                if (!entering)
                {
                    actionSymbol = "-";
                    actionText = "Exit";
                }

                string msg = string.Format("{0}{1} - {2}", actionSymbol, methodText, actionText);

                if (!string.IsNullOrEmpty(callerSuppliedFormat) && null != callerSuppliedArgs)
                {
                    var callerSupplied = string.Format(callerSuppliedFormat, callerSuppliedArgs);
                    msg = string.Format("{0}, {1}", msg, callerSupplied);
                }

                this.Log(msg);
            }
            catch (ArgumentNullException ex)
            {
                //Caller supplied format or args is null. 
                Debug.Assert(false, ex.Message);
            }
            catch (FormatException ex)
            {
                //Caller supplied format is invalid.
                //-or- 
                //The index of a format item is less than zero, or greater than or equal to the length of the args array. 
                Debug.Assert(false, ex.Message);
            }
        }

        private void Log(string msg)
        {
            Debug.WriteLine(msg);
        }

    }//class

    static class MethodBaseExtensions
    {
        static public string ToStringEx(this MethodBase mb)
        {
            var result = string.Format("{0}:{1}", mb.DeclaringType.Name, mb.Name);
            return result;
        }
    }
}//ns
