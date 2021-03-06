﻿using System;
using System.Diagnostics;
using System.Linq;
//using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
//using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.ComponentModel;
using System.Runtime.CompilerServices; 

namespace SharpUtils.Logger
{
    /// <summary>
    /// TODO GG: Consider https://github.com/nlog/NLog/ integration
    /// C# 3.5 Non ha i ThreadLocal. E' possibile emularli attraverso
    /// una annotazione speciale
    /// Vedi due link
    /// http://msdn.microsoft.com/en-us/library/system.threadstaticattribute(v=VS.90).aspx
    /// http://msdn.microsoft.com/en-us/library/6sby1byh.aspx
    /// NON INIZIALIZZARLO NELLA DICHIARAZIONE perché non funzionerà per tutti i thread...
    /// </summary>
    public class LoggerNDC
    {
        [ThreadStatic]
        static String currentNDC;

        public static void Push(string elem)
        {
            ensureInitialized();
            currentNDC+=elem;
        }

        public static string GetNDC()
        {
            ensureInitialized();
            return currentNDC;            
        }

        private static void ensureInitialized()
        {
            if (currentNDC == null)
            {
                Clear();
            }
        }

        public static void Clear(){
            currentNDC = "";
        }

    }

    public enum AuditAction
    {
        CREATE,UPDATE,DELETE,QUERY,
        INCIDENT

    }
    /// <summary>
    /// This class also support minimal auditing capability
    /// </summary>
    public class Logger
    {
        //private LogWriter myLogWriter;
        private string category { get; set; }
       
        

        /// <summary>
        /// Vedi http://stackoverflow.com/questions/40730/how-do-you-give-a-c-auto-property-a-default-value
        /// </summary>
        [Description("Parametro globale per abilitare dinamicamente debug log. Può essere impostato in qualsiasi momento"),
                System.ComponentModel.DefaultValue(true)]
        public static Boolean DefaultDebugMode  { get; set; }

        [Description("Diagnostic audit path")]
        public String AuditFilePath { get; set; }

        private static Boolean EnableConsoleOutputFlag = true;

        private Logger(string id)
        {
            this.category = id;
            if (string.IsNullOrEmpty(AuditFilePath))
            {
                AuditFilePath= System.Environment.GetEnvironmentVariable("TEMP");
            }
            //myLogWriter= EnterpriseLibraryContainer.Current.GetInstance<LogWriter>();           
        }

        public static Logger getLogger(string str)
        {
            return new Logger(str);      
        }

        public static Logger getLogger(object o)
        {
            string id=o.GetType().ToString();
            id = buildShortID(id);
            return new Logger(id);        
        }

        private static string buildShortID(string id)
        {
            if (id.Contains("."))
            {
                /// Cerca di ridurne la dimensione
                var parts = id.Split('.');
                id = parts.Last();
            }
            return id;
        }


        public static Logger getLogger(object o, string subCategory)
        {
            string id = o.GetType().ToString();
            return new Logger(buildShortID(id) + "." + subCategory);
        }



        public void info(String msg)
        {
            logMsg(msg, TraceEventType.Information);            
        }


        // WORKS ONLY ON .NET 4.5::::: [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void debug(String msg)
        {
            if (DefaultDebugMode)
            {
                logMsg(msg, TraceEventType.Verbose);
            }
        }

        public void warn(String msg)
        {
            logMsg(msg, TraceEventType.Warning);
        }

        public void warn(String errorMsg, Exception ex )
        {
            logMsg(errorMsg + "\n" + ex.Message + "\n\t" + ex.ToString(), TraceEventType.Warning);
        }


        public void error(String msg)
        {
            logMsg(msg, TraceEventType.Error);
        }


        public void audit(AuditAction action,Object obj){
            String f = AuditFilePath + "\\" + "audit.log";
            String msg = "AUDIT::"+action.ToString() + " " + obj;
            if (action == AuditAction.INCIDENT)
            {
                error(msg);
            }
            else
            {
                debug(msg);
            }
            
            System.IO.File.AppendAllText(f,msg+"\n\r");
        }


        private void logMsg(string msg, TraceEventType severity)
        {
            //LogEntry logEntry = new LogEntry();
            ////logEntry.EventId = 100;
            ////logEntry.Priority = 2;
            //// SE E SOLO SE cìè un thread local...aggiungilo
            //if (LoggerNDC.GetNDC().Equals(""))
            //{
            //    logEntry.Message = "@[] "+msg;
            //}
            //else
            //{
            //    logEntry.Message = "@[ " +LoggerNDC.GetNDC() +" ] "+msg;
            //}
            //logEntry.Categories.Add(this.category);
            //logEntry.Severity = severity;
            
            //myLogWriter.Write(logEntry);
            if (EnableConsoleOutputFlag)
            {
                String outmsg;

                if (severity == TraceEventType.Verbose)
                {
                    outmsg = "<D>";
                }
                else
                {
                    outmsg = "<" + severity.ToString().Substring(0, 1) + "> ";
                }


                outmsg += "["+this.category+"] ";
                if (LoggerNDC.GetNDC().Equals(""))
                {
                    outmsg += "@[] " + msg;
                }
                else
                {
                    outmsg += "@[ " + LoggerNDC.GetNDC() + " ] " + msg;
                }

                // TODO: Convert severity
                Console.WriteLine(outmsg);
            }
        }

        

        public void error(string errorMsg, Exception ex)
        {
            logMsg(errorMsg + "\n" + ex.Message + "\n\t" + ex.ToString(), TraceEventType.Error);
        }

        
        public  static void EnableConsoleOutput(bool val)
        {
            EnableConsoleOutputFlag = val;
        }
    }
}
