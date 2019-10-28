/*
 * Benjamin Tiernan
 * Trying to make a function that will pull in a sql query as a string,
 * parse it and then generate a list of columns that would match with the output.
 * 
 * Final goal will be to implement this along with a datagrid viewer to generate
 * more responsive output to speed up the generation of these files...
 * 
 */

using System.Windows;
using System.Collections.Generic;
using System.Data.SqlClient;// Why CAN'T I GET TO REGULAREXPRESSIONS...
using System.Text.RegularExpressions;
using System.Windows.Forms;

// we'll simplify this a tad more.
namespace CORRECTION_MASTER
{


    class SqlToGrid
    {
        protected static int maximumRecursion = 255; // just to be a tad arbitrary.
        // well - since regex isn't going too well, I guess I could build out an explicit loop to pull the columns.

        // we're going to need forms -  we will use a forms.datagridview
        // as well as generating a few other components.
        private string plaintextquery;
        private string[] columns;
        private Regex sqlcol = new Regex(""); // I'm having a tough time to handle the sql...
        private DataGridView dg_view = new DataGridView(); // we'll be using this to handle the human interface.
        //private 

        // we will focus on just parsing the column headers here.

        // we'll need a placeholder object to refer to the columns and rows.
        public SqlToGrid(string plaintext)
        {
            this.plaintextquery = plaintext;
        }

        public SqlToGrid(System.IO.FileInfo sourceFile)
        {
            string m = System.IO.File.ReadAllText(sourceFile.FullName);
            this.plaintextquery = m; // done.
        }

        public SqlToGrid(System.IO.FileStream sourceStream)
        {
            string m = "";
            while (sourceStream.CanRead)
            {
                m += sourceStream.ReadByte();
            }
        }


        // we'll need to set this up with a plaintext query.
        // add an attribute to make this harder to access.
        public string[] queryColumns(string[] plaintextquerycolumns)
        {
            return new string[] {"" }; // return an empty string
        }


        public static Dictionary<string,string> BoundaryCharacters = new Dictionary<string, string>
            {
            { "[","]" },
            { "(",")" },
            { "{","}" },
            { "/*","*/" },
            { "--","\n" }
            // use this by checking on the keys - read until new key or until matching delimiter
            /*,
            { "]","[" },
            { ")","(" },
            { "}","{" },
            { "/","/" },
            */

            };

        // this module really isn't necessary - just something I wanted to set up to make dynamic reporting a tad easier.
        // okay - we'll use this to read through, and pull things out based on what delimiter it's using...
        // this is much more elegant than the queryColumns preliminary structure.
        private void/*Dictionary<string,List<string>>*/ readToDelimiter(string blockToread, out Dictionary<string, List<string>> valueVar, string delimiter = "")
        {
            valueVar = new Dictionary<string, List<string>>(); // get the list - we'll be assigning and returning valuevar...

            Dictionary<string, List<string>> accuVar = new Dictionary<string, List<string>>(); // use this to accumulate anything being handled recursively.
            // get matching delimiter...     
            if(BoundaryCharacters.ContainsKey(delimiter))
            {
                // we have something to add - so we'll work through...
                // we'll want to keep reading through to determine what happens at each stage.
                for (int i = 0; i < blockToread.Length; i++)
                {
                    string x = blockToread[i].ToString();
                    switch (x)
                    {
                        // building in the lookahead.
                        case "/":
                            goto case "open";
                        case "-":
                            goto case "open";
                        case "[":
                            goto case "open";
                        case "{":
                            goto case "open";
                        case "open":
                            int offset = 0;
                            if(BoundaryCharacters.ContainsKey(x+blockToread[i+1]))
                            {
                                x += blockToread[i + 1];
                                offset = 1;
                            }
                            if (BoundaryCharacters.ContainsKey(x))
                            {
                                if (!valueVar.ContainsKey(x))
                                {
                                    valueVar.Add(x, new List<string>());
                                }
                                readToDelimiter(blockToread.Substring(i + offset, blockToread.Length - i + offset),
                                    out accuVar,
                                    x + blockToread[i + offset]
                                    );
                            }
                            break;
                        case "close":
                            //if(Boundary)
                            
                            if(BoundaryCharacters.ContainsValue(x+blockToread[i+1]))
                            {
                                x = x + blockToread[i + 1]; //
                            }
                            // 
                            if (BoundaryCharacters.ContainsValue(x))
                            {
                                // close the boundary object.
                            }
                            break;
                        default:
                            if (BoundaryCharacters.ContainsKey(x))
                            {

                                if (!valueVar.ContainsKey(x + blockToread[i + 1]))
                                {
                                    valueVar.Add(x + blockToread[i + 1], new List<string>());
                                }
                                readToDelimiter(blockToread.Substring(i + 1, blockToread.Length - i + 1),
                                    out accuVar,
                                    x + blockToread[i + 1]
                                    );
                            }
                            break;
                    }
                }
            }
            else
            {
                for(int i = 0; i<blockToread.Length;i++)
                {
                    string x = blockToread[i].ToString();
                    switch(x)
                    {
                        // building in the lookahead.
                        case "/":
                            if(BoundaryCharacters.ContainsKey(x+blockToread[i+1]))
                            {
                                delimiter = x + blockToread[i + 1];
                            }
                            break;
                        case "-":
                            if(BoundaryCharacters.ContainsKey(x+blockToread[i+1]))
                            {
                                delimiter = x + blockToread[i + 1];
                            }                          
                            break;
                        default:
                            if(BoundaryCharacters.ContainsKey(x))
                            {
                                delimiter = x;
                            }
                            break;
                    }
                }
                
                // if there isn't anythign in a delimiter - we don't have anything to add to the output variable.
            }
            return;
        }

        //
    }
}