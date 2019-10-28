using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CORRECTION_MASTER.FORMS;

namespace CORRECTION_MASTER
{
    // upgrading the unit to handle the various forms and components found in the NACHA parser.
    class File_Instance
    {
        // here is what we'll work through - build in an instance of each 
        public string FileName = "";

        public Landing_Page home_page = new Landing_Page(); 
        public ABOUT_NACHA about_page = new ABOUT_NACHA();

        // tweak the access record method to handle an empty filename.
        public ACCESS_RECORD file_page = new ACCESS_RECORD();


        //
        public DELETE_FORM delete_page = new DELETE_FORM();

        //
        public ENTRY_FORM entry_page = new ENTRY_FORM();

        //
        public FILE_VALIDATOR validator_page = new FILE_VALIDATOR();

        //
        public NACHA_TO_XL export_page = new NACHA_TO_XL();


        public File_Instance()
        {

        }

        ~File_Instance()
        {

        }
    }
}
