using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronPython;
using IronPython.Runtime;
using IronPython.Hosting;
using IronPython.Compiler;
using IronPython.Modules;
using System.Runtime.Hosting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Runtime;


// we're going to double down on some embedded python
namespace CORRECTION_MASTER
{
    // I need to rework the inheritance model I'm using in this application...
    public class PyEngine
    {

        public static ScriptEngine se;

    }
}
