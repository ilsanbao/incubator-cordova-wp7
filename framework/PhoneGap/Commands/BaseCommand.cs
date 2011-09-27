﻿using System;
using System.Reflection;

namespace WP7GapClassLib.PhoneGap.Commands
{
    public abstract class BaseCommand
    {
        /*
         *  All commands + plugins must extend BaseCommand, because they are dealt with as BaseCommands in PGView.xaml.cs
         *  
         **/

        public event EventHandler<PluginResult> OnCommandResult;

        public event EventHandler<ScriptCallback> OnCustomScript;

        public BaseCommand()
        {
             
        }

        /*
         *  InvokeMethodNamed will call the named method of a BaseCommand subclass if it exists and pass the variable arguments list along.
         **/

        public object InvokeMethodNamed(string methodName, params object[] args)
        {
            MethodInfo mInfo = this.GetType().GetMethod(methodName);

            if (mInfo != null)
            {
                // every function handles DispatchCommandResult by itself
                return mInfo.Invoke(this, args);
            }

            // actually methodName could refer to a property
            if (args == null || args.Length == 0 ||
               (args.Length == 1 && "undefined".Equals(args[0])))
            {
                PropertyInfo pInfo = this.GetType().GetProperty(methodName);
                if (pInfo != null)
                {
                    
                    object res = pInfo.GetValue(this , null);

                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK, res));
                    
                    return res;
                }
            }

            throw new MissingMethodException(methodName);            

        }


        public void InvokeCustomScript(ScriptCallback script)
        {
            if (this.OnCustomScript != null)
            {
                this.OnCustomScript(this, script);               
            }
        }

        public void DispatchCommandResult()
        {
            this.DispatchCommandResult(new PluginResult(PluginResult.Status.NO_RESULT));
        }

        public void DispatchCommandResult(PluginResult result)
        {
            if (this.OnCommandResult != null)
            {
                this.OnCommandResult(this, result);

                if (!result.KeepCallback)
                {
                    this.OnCommandResult = null;
                }

            }
        }
    }
}
