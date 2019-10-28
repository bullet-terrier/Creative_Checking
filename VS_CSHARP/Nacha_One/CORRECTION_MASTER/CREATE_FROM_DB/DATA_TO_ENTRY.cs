using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORRECTION_MASTER.CREATE_FROM_DB
{
    class DATA_TO_ENTRY
    {

        Dictionary<string, string> entry_record = new Dictionary<string, string>(); // we'll handle this.

        public List<string> get_keys() {
            return new List<string>
        {
            "E. Rec Type"
            ,"Trans. Code"
            ,"Routing Num"
            ,"Check Digit"
            ,"Account Num"
            ,"Amount     "
            ,"ID Number  "
            ,"Rec. Name  "
            ,"Disc. Data "
            ,"Addenda Ind"
            ,"Trace Num. "
            ,"A. Rec Type"
            ,"Adden. Type"
            ,"Paymnt Info"
            ,"Add Seq Num"
            ,"Ent Seq Num"

        };
        }

        void add_field(string fieldname, string fieldvalue) { }
        void add_field(string fieldname, int fieldvalue) { }
        void add_field(string fieldname, float fieldvalue) { }
        void add_field(string fieldname, object fieldvalue) { }
        void add_field(Tuple<string, string> fieldnamvalue) { }
        void add_fields(Dictionary<string, string> keyValues)
        {
        } // I'm not sure what we necessarily need this one to do.

    }
}
