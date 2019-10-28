// #define DEBUG
// #define UPDATE_20190723
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Drawing;
using System.Configuration;
using System.Diagnostics;


// okay - so we're using the ironpython lib for runtime now.
#if UPDATE_201910
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
#endif


// changes:
//   2019-07-25: added in new field for "AUTOMATICALLY_OPEN_SEARCHED_ENTRY" which impacts behavior on the search option for the "ACCESS_RECORD","ENTRY_SEARCH",and "ENTRY_FORM" forms.
//   2019-10-16: adding embeddable IronPython in order to more rapidly extend these functions.
//
//
// todo:
//   * add readonly behavior to the configuration options.
//   * add a seaparate read/write behavior set to the configuration tool, to simplify configuration in future releases.
//
//
namespace CORRECTION_MASTER
{

#if UPDATE_201910
    // we may inherit this for some derivative classes
    // I'm aiming to include some extensible scripting to make this handle a wider array of objects.
    // we'll want this to represent the most basic components of the scripting language.
    public class Engine
    {
        protected ScriptEngine engine;
        protected ScriptScope scope;

        public Engine()
        {
            //
            engine = Python.CreateEngine();
            scope = engine.CreateScope();
        }

#endif
        // so we'll want to try printing something out - I'll use this in debug/
#if DEBUG
        /// <summary>
        /// Testrun - we'll just handle a super super basic function here,
        /// but we'll be able to expand on so much of this pretty quickly.
        /// I'm going to try adding something to inherit a python model for other fields.
        /// </summary>
        public static void testRun()
        {


            // ITS ALIVE!
            ScriptEngine engine = Python.CreateEngine(); // 
            ScriptEngine engine2 = Python.CreateEngine(); // might try using a createruntime call instead.
            ScriptScope scope = engine.CreateScope();
            ScriptScope scope2 = engine.CreateScope();
            ScriptScope scope3 = engine.CreateScope(); // trying to load multiple scripts - let's see...
            // initializing these here in an attempt to capture more verbose output at the end.
            ScriptSource source = engine.CreateScriptSourceFromString("");
            ScriptSource sourc2 = engine.CreateScriptSourceFromString("");
            // might need to adjust the means the engine is using to render the output - once It's been used, it may require some differences.
            ScriptSource source3 = engine.CreateScriptSourceFromString("");
            try
            {

                // ITS ALIVE!
                /*
                ScriptEngine engine = Python.CreateEngine(); // 
                ScriptScope scope = engine.CreateScope();
                ScriptScope scope2 = engine.CreateScope();
                ScriptScope scope3 = engine.CreateScope(); // trying to load multiple scripts - let's see...
                */
                // I guess we should include it as an explicitly defined thing.
                // okay - these need to be a single line if possible.
                source = engine.CreateScriptSourceFromString(
                    "for a in range(0,1000): print(a);", SourceCodeKind.AutoDetect);
                object result = source.Execute(scope);
                
                Console.WriteLine($"Executed embedded python: {result.ToString()}");
                // if this works
                source3 = engine2.CreateScriptSourceFromString("print('wanted to launch this from embedded context - so far so good')"); 
                source3.Execute(scope3);
                //sourc2 = engine.CreateScriptSourceFromString(System.IO.File.ReadAllText($"{Environment.CurrentDirectory}/Scripts/NewAch.py"));
                //object result2 = sourc2.Execute(scope2);
                //System.IO.File.WriteAllText("PythonExecuted.txt",$"Executed embedded python: {System.IO.File.ReadAllText("Scripts/NewAch.py")}"+"\nThis worked pretty well all things considered");
                /**/
                // blocking these out in order to diagnose if it is creating an error each time.
            }
            catch(Exception e)
            {
                try
                {
                    System.IO.File.AppendAllLines("PythonembeddedError.txt", new string[] {
                        e.Data.ToString(),
                    e.StackTrace,
                e.Source,
                e.Message,
                e.ToString(),
                e.TargetSite.ToString()

                }
                    );
                    System.IO.File.AppendAllLines("PythonembeddedError.txt", new string[] { source.GetCode(),sourc2.GetCode(),source3.GetCode()});
                }
                catch (Exception ec)
                { System.IO.File.WriteAllText("./out.txt", ec.ToString()); }

                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Source);
            }            

        }

#endif
    }



// I'm not 100% sure, but this might be a good place to integrate the spooler and xmlutils.
// we'll generate a secondary utility for handling global errors.
class Program
{
#if UPDATE_20190807
    /// <summary>
    /// List of exceptions encountered throughout the application's execution paths.
    /// Wrapped in such a way to prevent use in a backdated model.
    /// </summary>
    static public List<Exception> encountered_Exceptions = new List<Exception>();
#endif

#if UPDATE_20190723
    // invocation: Program.set_font(c);
    /// <summary>
    /// We'll let this go to our heads.
    /// </summary>
    /// <param name="c">Set a controls font.</param>
    /// <param name="f">Font control</param>
    public static void set_font(Control c, Font f = null)
    {
        // we don't use the overload yet - but we will eventually add a series of configurations for this.
        // we'll try using this to set the font for all child controls recursively.
        try
        {
            // okay - so this works well - need to restructure some of it though.
#if UPDATE_20190725
            c.Font = new System.Drawing.Font(DEFAULT_APPLICATION_FONT, DEFAULT_APPLICATION_FONTSIZE);
#else
                c.Font = new System.Drawing.Font("Letter Gothic", 8);
#endif
        }
        catch
        {

        }
        foreach (Control e in c.Controls)
        {
            try
            {
                set_font(e);
            }
            catch // this may throw an error if that isn't available.
            {

            }

        }
    }

#if DEBUG
        /// <summary>
        /// attempting a generalized form of the dictionary key search.
        /// this will only be able to work on object references. you'll need to use the .Equals() method 
        /// for anything being checked by this.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static object DictionaryKeyFromValue(object value, Dictionary<object,object> dictionary)
        {
            object key = null;
            foreach(var a in dictionary.Keys)
            {
                if(dictionary[a].Equals(value))
                {
                    key = a; // this should work.
                    break;
                }
            }
            return key;
        }
#endif
#endif

    public static List<Process> Global_Procs = new List<Process>();

    private static List<Exception> Exceptions = new List<Exception>(); // we might want to set a specific utility to periodically load and manipulate this list.

    // some global switches.
    /// <summary>
    /// 
    /// </summary>
    public static bool PROCESS_ON_OPEN = (ConfigurationManager.AppSettings["PROCESS_ON_OPEN"] ?? "") == "true";

    /// <summary>
    /// 
    /// </summary>
    public static bool SAVE_ON_CLOSE = (ConfigurationManager.AppSettings["SAVE_ON_CLOSE"] ?? "") == "true";

    /// <summary>
    /// 
    /// </summary>
    public static bool EXPERT_MODE = (ConfigurationManager.AppSettings["EXPERT_MODE"] ?? "") == "42"; // we'll make this a bit cheekier.

    /// <summary>
    /// 
    /// </summary>
    public static bool HANDLE_ADDENDA = (ConfigurationManager.AppSettings["HANLDE_ADDENDA"] ?? "") == "true";

    /// <summary>
    /// 
    /// </summary>
    public static bool ALLOW_OLD_EDITOR = (ConfigurationManager.AppSettings["ALLOW_OLD_EDITOR"] ?? "") == "true";

    /// <summary>
    /// 
    /// </summary>
    public static bool REMOVE_HEADERS_FOR_CSV = (ConfigurationManager.AppSettings["REMOVE_HEADERS_FOR_CSV"] ?? "") == "true"; // this will work.
#if UPDATE_20190723
    /// <summary>
    /// 
    /// </summary>
    public static readonly bool AUTOMATICALLY_OPEN_SEARCHED_ENTRY = (ConfigurationManager.AppSettings["AUTOMATICALLY_OPEN_SEARCHED_ENTRY"] ?? "") == "true";

    /// <summary>
    /// 
    /// </summary>
    public static readonly bool CASE_INSENSITIVE_ENTRY_SEARCH = (ConfigurationManager.AppSettings["CASE_INSENSITIVE_ENTRY_SEARCH"] ?? "") == "true";
#endif

#if UPDATE_20190725
    /// <summary>
    /// Configuration for the font will be farmed out to the configuration file.
    /// </summary>
    public static readonly string DEFAULT_APPLICATION_FONT = (ConfigurationManager.AppSettings["DEFAULT_APPLICATION_FONT"] ?? "Consolas");

    /// <summary>
    /// Configuration for the font size will be farmed out to the configuration file and held in memory during application lifespan.
    /// </summary>
    public static readonly int DEFAULT_APPLICATION_FONTSIZE = Int32.Parse((ConfigurationManager.AppSettings["DEFAULT_APPLICATION_FONTSIZE"] ?? "08"));

#endif

#if UPDATE_201909
    public static readonly string DEFAULT_APPLICATION_BEHAVIOR = (ConfigurationManager.AppSettings["DEFAULT_BEHAVIOR"] ?? "");
#endif

    // I don't think anyghint is used in this version --- slating it for removal
    // we'll set this up as an old static event handler. 
    [Obsolete("We won't support the use of the old editor tooling.")]
    public static void START_OLD_EDITOR(object sender, EventArgs e)
    {
        throw new NotImplementedException("This module is being slated for removal. Please remove all references to it in your code.");
    }

#if DEBU3G_UPDATE_201909
        [STAThread]
        static void Main(string[] args)
        {
            Application.Run(new ST.DC_EFT_ENTRIES()); // test the new constructed unit.
        }
#else

#if DEBUG
        [STAThread]
        static void Main(string[] args)
        {
#if UPDATE_201909
            // different behavior patterns depedning on how the application is configured.
            // cool - using this to route seems to be perfectly fine.
            /**
             * DO NOT BYPASS THE FORMS.Startup Dialog.
             * DO NOT CHANGE THE FORMS.Startup Dialog - unless you are adding to it!
             * DO NOT Allow the program to run without the startup dialog!
             * 
             * If you are looking to modify the about information to include your changes - 
             * Add a header to each of these files, and include it in the <About> form.
             */
            var m = new FORMS.Startup();
            m.ShowDialog();
            if (FORMS.Startup.allowContinue)
            {
                // ADD ANYTHING THAT MUST HAPPEN HERE.
            }
            else
            {
                Environment.Exit(1);
            }
            switch (DEFAULT_APPLICATION_BEHAVIOR.ToUpper())
            {
                case "UNLIMITED":
                    break;
                case "LIMITED":
                    //Application.Run(new ST.DC_EFT_ENTRIES());
#if DEBUG
                    //Engine.testRun();
#endif
                    Application.Run(new Landing_Page());
                    //
                    break;
                default:
                    Application.Run(new Landing_Page());
                    break;
            }

#else
            
            Application.Run(new Landing_Page());

            foreach(Process p in Program.Global_Procs)
            {
                if (Program.EXPERT_MODE) { break; } // we'll allow expert mode processes to live beyond this process.
                if(!p.HasExited)
                {
                    p.Kill(); // we'll roll through and kill all of the outstanding processes.
                }
            }
        }
#endif
        }
#else
    [STAThread]
    static void Main(string[] args)
    {
        Application.Run(new CORRECTION_MASTER.Landing_Page());

#endif
    }
#endif

}

//}
