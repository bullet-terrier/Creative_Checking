using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;


namespace CORRECTION_MASTER
{
    // might bypass this for some functions in the main loop.
    /// <summary>
    /// Dotnet object representation of a run of the mill NACHA file.
    /// </summary>
    public class ACH_FILE
    {
        #region PRE_20190807
        /*
        public Dictionary<string,List<string>> Entry_By_Names { get; set; }
        public Dictionary<string,List<string>> Entry_By_Amount { get; set; }
        public Dictionary<string,List<string>> Entry_By_Account { get; set; }
        */

        // the main thing we'll want to do is open a file when able - we can use a function to close a couple of pages at a time.

        /// <summary>
        /// placeholder for the file_header object
        /// </summary>
        public string file_header = ""; // we'll also try to wrap things up in a single batch.
        /// <summary>
        /// placeholder for the batch_header object;
        /// </summary>
        public string batch_header = "";
        /// <summary>
        /// place holder for the file_footer object;
        /// </summary>
        public string file_footer = "";
        /// <summary>
        /// placeholder for the batch_footer object.
        /// </summary>
        public string batch_footer = "";
        /// <summary>
        /// the raw ach file that will be manipulated.
        /// </summary>
        public string[] raw_file { get; set; }
        
        // padding will be handled after the fact, check globals.

        // we'll have this on each file. name/amount in this order.
        /// <summary>
        /// Public list of the available entry/addenda objects. listed as a tuple of [name,value]=[entry,addenda]
        /// </summary>
        public Dictionary<Tuple<string, string>, Tuple<string, string>> entryAddenda = new Dictionary<Tuple<string, string>, Tuple<string, string>>();
        //
        //public List<string> file { get; set; }
        /// <summary>
        /// public acccessor for the add_entry_addenda method of this module.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="indx"></param>
        public void add_entry(string[] file, int indx)
        {
            add_entry_addenda(file, indx);
        }

        /// <summary>
        /// add a new set of entry/addenda to the file.
        /// </summary>
        /// <param name="file">string array component listing the individual files.</param>
        /// <param name="indx">??</param>
        protected void add_entry_addenda(string[] file, int indx)
        {
            // name,amount = tuple 1
            // entry, addenda = tuple 2
#if DEBUG
            for(int i = indx; i<indx+1;i++)
            {
                Console.WriteLine($"LENGTH: {file[i].Length} VALUE: {file[i]}");
            }
#endif 
            string amt = "";
            string name = "";
            string entry = file[indx];
            string addenda = ""; // we'll populate this if entry has some conditions met.
#if UPDATE_20190807
#else
            bool addenda_indicator = false; // this does get used - I think that intellisense is just getting mangled.
#endif

            var tbase = FORMS.ENTRY_FORM.field_map["Addenda Ind"];
            var nbase = FORMS.ENTRY_FORM.field_map["Rec. Name  "];
            var abase = FORMS.ENTRY_FORM.field_map["Amount     "];

            // if there is an addenda indicator.
            try
            {
                if (!(entry.Substring(tbase.Item1, tbase.Item2).Contains(' ') || entry.Substring(tbase.Item1, tbase.Item2).Contains('0')))
                {
#if UPDATE_20190807
#else
                    addenda_indicator = true;
#endif
                    addenda = file[indx + 1]; // get the next line, since it will contain the index.
                }
            }
            catch (ArgumentOutOfRangeException AOORE)
            {
#if UPDATE_20190807
                Program.encountered_Exceptions.Add(AOORE);
#endif
#if DEBUG

                for (int z = 0; z < entry.Length; z++) { Console.WriteLine($"{ entry[z]}\t{z}"); } // trying to diagnose the problem.
#endif
            }
            catch (IndexOutOfRangeException IOORE)
            {
#if UPDATE_20190807
                Program.encountered_Exceptions.Add(IOORE);
#endif
#if DEBUG
                for (int z = 0; z < entry.Length; z++) { Console.WriteLine($"{ entry[z]}\t{z}"); } // trying to diagnose the problem.
#endif
            }
            // no need for an else as of now.
            try
            {


                // OOOOH - THIS IS WHY THE NAMES ARE SCREWY!
                // this will give us the name and amount to use as keys.
                amt = entry.Substring(abase.Item1, abase.Item2);
#if UPDATE_20190723
                name = entry.Substring(nbase.Item1, nbase.Item2);
#else
            name = entry.Substring(nbase.Item1, abase.Item2);
#endif
            }
            catch(IndexOutOfRangeException IOORE2)
            {
                Console.WriteLine(IOORE2);
                Console.WriteLine(IOORE2.StackTrace);
                Console.WriteLine(IOORE2.Source);
            }
            // alrighty -so the dictionary will get all of the name/amount combinations that we need. hopefully this won't cause any issues.
            // might encounter a duplicate key error... but I'm not sure.
            entryAddenda.Add(new Tuple<string, string>(name, amt), new Tuple<string, string>(entry, addenda));

        }

        /// <summary>
        /// Parse a list of strings into a dictionary representation of the ACH file.
        /// This splits the contents out into a series of string based references.
        /// </summary>
        /// <param name="raw_file">source lines to interpret.</param>
        public void parse_to_dict(string[] raw_file)
        {
            bool found_file = false;
            bool found_batch = false;
            bool found_bhead = false;
            // we'll only worry about the corrected file.
            for(int i = 0; i<raw_file.Length; i++)
            {
                // I'll eventually want to set these up to fit a dictionary object with their objects, but for now, I'll handle it as I am.
                switch(raw_file[i][0])
                {
                    case '1':
                        file_header = raw_file[i];
                        break;
                    case '5':
                        if(!found_bhead)
                        {
                            batch_header = raw_file[i];
                            found_bhead = true;
                        }
                        break;
                    case '6':
                        try
                        {


                            // forgot to add my trigger...
                            add_entry_addenda(raw_file, i); // this should do it.
                                                            // add_entry_addenda // we'll strip out the addenda records for now...
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e.StackTrace);
                            Console.WriteLine(e);
                        }
                        break;
                    case '7':
                        continue;
#if UPDATE_20190807 // removing obsolete and unecessary lines.
#else
                        break;
#endif
                    case '8':
                        if(!found_batch)
                        {
                            batch_footer = raw_file[i];
                            found_batch = true;
                        }

                        break;
                    case '9':
                        if(!found_file)
                        {
                            file_footer = raw_file[i];
                            found_file = true;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Load an ach file from the fileinfo - this shouldn't be used as it wasn't ever implemented.
        /// </summary>
        /// <param name="fii"></param>
        /// <returns></returns>
        [Obsolete("Never implemented, will eventually.")]
        public static ACH_FILE get_from_file(FileInfo fii)
        {
            // we'll use this to push a new ACH_FILE;
            return new ACH_FILE(new string[0]);
        }

        /// <summary>
        /// Construct a batch footer based on the contents of a series of batch_lines.
        /// </summary>
        /// <param name="batch_lines">Batch contents</param>
        /// <returns>batch footer matching the batch lines;</returns>
        public string construct_batch_footer (string[] batch_lines)
        {
            string class_code = "";
            string current_batch = "";
            int hash = 0;
            int entry_add_count = 0;
            int debit_dollar = 0;
            int credit_dollar = 0;
            string company_id = ""; // we'll need to extract from the original.
            string MAC = ""; // extract from the original;
            string originatingdfi = "";// extract from the original.
            string batch_number = ""; // extract from original.

            int try_parse_out = 0;
            // okay - we'll need to accumulate the entries in batch_lines.
            for(int i = 0; i<batch_lines.Length;i++)
            {
                switch (batch_lines[i][0])
                {
                    case '5':
                        break;
                    case '6':
                        var t_code = FORMS.ENTRY_FORM.field_map["Trans. Code"];
                        var a_code = FORMS.ENTRY_FORM.field_map["Amount     "];
                        var h_code = FORMS.ENTRY_FORM.field_map["Routing Num"];
                        var amount = batch_lines[i].Substring(a_code.Item1, a_code.Item2);
                        entry_add_count++;
                        if (new List<string>{ "27", "37" }.Contains(batch_lines[i].Substring(t_code.Item1,t_code.Item2))) // debit
                        {
                            Int32.TryParse(amount, out try_parse_out);
                            debit_dollar += try_parse_out;
                        }
                        else if (new List<string> { "22","32" }.Contains(batch_lines[i].Substring(t_code.Item1, t_code.Item2))) // credit
                        {
                            Int32.TryParse(amount, out try_parse_out);
                            credit_dollar += try_parse_out;
                        }
                        else //if (new List<string> { }.Contains(batch_lines[i].Substring(t_code.Item1, t_code.Item2))) // prenote
                        {
                            // prenotes don't require action.
                        }
                        Int32.TryParse(batch_lines[i].Substring(h_code.Item1, h_code.Item2),out try_parse_out);
                        hash += try_parse_out;
                        // reset try_parse_out.
                        break;
                    case '7':
                        entry_add_count += 1;
                        break;
                    case '8':
                        break;
                    default: // don't don anything.
                        break;
                }
                try_parse_out = 0;

            }
            
            // once again - extract and pad.
            class_code = GLOBALS._trun(GLOBALS._pad(batch_footer.Substring(GLOBALS.batch_fields["service code"].Item1, GLOBALS.batch_fields["service code"].Item2), GLOBALS.batch_fields["service code"].Item2), GLOBALS.batch_fields["service code"].Item2);

            // extract and pad.
            current_batch = GLOBALS._trun(GLOBALS._pad(batch_footer.Substring(GLOBALS.batch_fields["batch number"].Item1, GLOBALS.batch_fields["batch number"].Item2), GLOBALS.batch_fields["batch number"].Item2), GLOBALS.batch_fields["batch number"].Item2);

            // extract and pad (again)
            company_id = GLOBALS._trun(GLOBALS._pad(batch_footer.Substring(GLOBALS.batch_fields["company iden"].Item1, GLOBALS.batch_fields["company iden"].Item2), GLOBALS.batch_fields["company iden"].Item2), GLOBALS.batch_fields["company iden"].Item2);

            // 
            MAC = GLOBALS._trun(GLOBALS._pad(batch_footer.Substring(GLOBALS.batch_fields["message auth"].Item1, GLOBALS.batch_fields["message auth"].Item2), GLOBALS.batch_fields["message auth"].Item2), GLOBALS.batch_fields["message auth"].Item2);

            //
            originatingdfi = GLOBALS._trun(GLOBALS._pad(batch_footer.Substring(GLOBALS.batch_fields["origin route"].Item1, GLOBALS.batch_fields["origin route"].Item2), GLOBALS.batch_fields["origin route"].Item2), GLOBALS.batch_fields["origin route"].Item2);

            // redundant?
            batch_number = current_batch;

            string batch_out = "";
            batch_out += GLOBALS.trun_(GLOBALS.pad_("8", 1), 1);
            batch_out += class_code;
            batch_out += GLOBALS._trun(GLOBALS._pad(entry_add_count.ToString(), GLOBALS.batch_fields["entadd count"].Item2,'0'), GLOBALS.batch_fields["entadd count"].Item2);
            batch_out += GLOBALS._trun(GLOBALS._pad(hash.ToString(), GLOBALS.batch_fields["entry hash  "].Item2, '0'), GLOBALS.batch_fields["entry hash  "].Item2);
            batch_out += GLOBALS._trun(GLOBALS._pad(debit_dollar.ToString(), GLOBALS.batch_fields["debit contr."].Item2, '0'), GLOBALS.batch_fields["debit contr."].Item2);
            batch_out += GLOBALS._trun(GLOBALS._pad(credit_dollar.ToString(), GLOBALS.batch_fields["credit cont."].Item2, '0'), GLOBALS.batch_fields["credit cont."].Item2);
            batch_out += company_id;
            batch_out += MAC;
            batch_out += GLOBALS.pad_("", GLOBALS.batch_fields["reserved    "].Item2); // this should handle the output.
            batch_out += originatingdfi;
            batch_out += batch_number;
            //batch_out += GLOBALS.trun_(GLOBALS.pad_(""))
#if DEBUG
            Console.WriteLine(batch_out);
#endif
            return batch_out;
        }

        /// <summary>
        /// Construct a file footer for use in the output ach_file.
        /// Will account for any changes made in memory while the file was opened.
        /// </summary>
        /// <param name="file_lines">all of the lines associated with the file.</param>
        /// <returns>File footer as a string.</returns>
        public string construct_file_footer(string[] file_lines)
        {
            int batch_count = 0;
            int block_count = 0;
            int entry_addenda_count = 0;
            int entry_hash = 0;
            int total_debit_dollar = 0;
            int total_credit_dollar = 0;
#if UPDATE_20190807 // going forward remove this item.
#else
            string reserved = "";
#endif

            // the plus 1 is for this record.
            int m = file_lines.Length+1;
            while(m%10!=0)
            {
                m++;
            }
            // now we need to divide by 10 to get the block count.
            // I don't know how I deleted the div 10...
            block_count = m/10;
            

            int try_parse_out = 0;
            // okay - we'll need to accumulate the entries in batch_lines.
            for (int i = 0; i < file_lines.Length; i++)
            {
                switch (file_lines[i][0])
                {
                    case '5':
                        batch_count += 1;
                        break;
                    case '6':
                        var t_code = FORMS.ENTRY_FORM.field_map["Trans. Code"];
                        var a_code = FORMS.ENTRY_FORM.field_map["Amount     "];
                        var h_code = FORMS.ENTRY_FORM.field_map["Routing Num"];
                        var amount = file_lines[i].Substring(a_code.Item1, a_code.Item2);
                        entry_addenda_count++;
                        if (new List<string> { "27", "37" }.Contains(file_lines[i].Substring(t_code.Item1, t_code.Item2))) // debit
                        {
                            Int32.TryParse(amount, out try_parse_out);
                            total_debit_dollar += try_parse_out;
                        }
                        else if (new List<string> { "22", "32" }.Contains(file_lines[i].Substring(t_code.Item1, t_code.Item2))) // credit
                        {
                            Int32.TryParse(amount, out try_parse_out);
                            total_credit_dollar += try_parse_out;
                        }
                        else //if (new List<string> { }.Contains(batch_lines[i].Substring(t_code.Item1, t_code.Item2))) // prenote
                        {
                            // prenotes don't require action.
                        }
                        Int32.TryParse(file_lines[i].Substring(h_code.Item1, h_code.Item2), out try_parse_out);
                        entry_hash += try_parse_out;
                        // reset try_parse_out.
                        break;
                    case '7':
                        entry_addenda_count += 1;
                        break;
                    case '8':
                        break;
                    default: // don't don anything.
                        break;
                }
                try_parse_out = 0;
            }

            string file_out = "";
            file_out += "9";
            file_out += GLOBALS._trun(GLOBALS._pad(batch_count.ToString(), GLOBALS.file_fields["batch count "].Item2, '0'), GLOBALS.file_fields["batch count "].Item2);
            file_out += GLOBALS._trun(GLOBALS._pad(block_count.ToString(), GLOBALS.file_fields["block count "].Item2, '0'), GLOBALS.file_fields["block count "].Item2);
            file_out += GLOBALS._trun(GLOBALS._pad(entry_addenda_count.ToString(), GLOBALS.file_fields["entadd count"].Item2, '0'), GLOBALS.file_fields["entadd count"].Item2);
            file_out += GLOBALS._trun(GLOBALS._pad(entry_hash.ToString(), GLOBALS.file_fields["entry hash  "].Item2, '0'), GLOBALS.file_fields["entry hash  "].Item2);
            file_out += GLOBALS._trun(GLOBALS._pad(total_debit_dollar.ToString(), GLOBALS.file_fields["debit  cont."].Item2, '0'), GLOBALS.file_fields["debit  cont."].Item2);
            file_out += GLOBALS._trun(GLOBALS._pad(total_credit_dollar.ToString(), GLOBALS.file_fields["credit cont."].Item2, '0'), GLOBALS.file_fields["credit cont."].Item2);
            file_out += GLOBALS._trun(GLOBALS._pad("", GLOBALS.file_fields["reserved    "].Item2), GLOBALS.file_fields["reserved    "].Item2);

#if DEBUG
            //
            Console.WriteLine(file_out);
#endif

            return file_out; // placeholder.
        }

        /// <summary>
        /// Return the list for the internal file.
        /// </summary>
        /// <returns>String array representation of the ACH_FILE.</returns>
        public string[] get_file()
        {
            List<string> total_file = new List<string>(); // we'll clear batches for now.
            List<string> entry_addenda = new List<string>();
            foreach(Tuple<string,string> t in entryAddenda.Keys)
            {
                // that should have been avoided.
                entry_addenda.Add(entryAddenda[t].Item1);
                if(!new List<string> { " ","","0"}.Contains(entryAddenda[t].Item2) )
                {
                    entry_addenda.Add(entryAddenda[t].Item2);
                }
            }
            total_file.Add(file_header);
            total_file.Add(batch_header);
            foreach(string a in entry_addenda) { total_file.Add(a); }
            total_file.Add(construct_batch_footer(entry_addenda.ToArray()));
            total_file.Add(construct_file_footer(total_file.ToArray()));

            return GLOBALS.padded_file(total_file.ToArray());
        }

        /// <summary>
        /// Generate a new instance of an ACH_FILE object from a basic string array
        /// </summary>
        /// <param name="raw">string array representation of an ACH FILE - can be obtained by calling File.ReadAllLines(target)</param>
        public ACH_FILE(string[] raw)
        {
            raw_file = raw;
            parse_to_dict(raw); // this should populate our dictionary with information now.
        }

#region UPDATE_20190723
#if UPDATE_20190723
        // adding block for public accessor to the add_entry_addenda() method.
        
        // this is to update the current key list.

        /// <summary>
        /// replace an old key with the updated key.
        /// 
        /// seems to work as designed - couldn't be more pleased.
        /// </summary>
        /// <param name="key"> old key that will be replaced.</param>
        /// <param name="newValue">entry/addenda record to replace the current key/value pair. only requires value as key will be derived.</param>
        public void updateEntryAddenda(Tuple<string,string> key, Tuple<string,string> newValue)
        {
            this.entryAddenda.Remove(key);
            var nbase = FORMS.ENTRY_FORM.field_map["Rec. Name  "];
            var abase = FORMS.ENTRY_FORM.field_map["Amount     "];
            var amount = newValue.Item1.Substring(abase.Item1, abase.Item2);
            var name = newValue.Item1.Substring(nbase.Item1, nbase.Item2);
            this.entryAddenda.Add(new Tuple<string, string>(name, amount), newValue);
        }

        /// <summary>
        /// Add new entry addenda record to the entry addenda keys.
        /// </summary>
        /// <param name="nameAmount"></param>
        /// <param name="entryAddendaLines"></param>
        public void addNewEntryAddenda(Tuple<string,string> nameAmount,Tuple<string,string> entryAddendaLines)
        {
            this.entryAddenda.Add(nameAmount, entryAddendaLines); // that should handle the update.
        }
#endif
#endregion
#endregion
#if UPDATE_20190807
        // do we want to make this a static method to simply return the values - or should we make it an instance method.
        // I can see a case for both, so we'll do that.
        /// <summary>
        /// Calculate the batch debit amount from the provided entryaddedna strings.
        /// </summary>
        /// <param name="entryAddenda"></param>
        /// <returns></returns>
        public static float CalculateBatchDebitAmount(string[] entryAddenda)
        {
            float debit_amount = 0; // we'll need to convert the type later.
            int curData = 0;
            for(int i = 0; i<entryAddenda.Length; i++)
            {
                try
                {
                    var toggle = FORMS.ENTRY_FORM.field_map["Trans. Code"];
                    if (entryAddenda[i].Substring(toggle.Item1, toggle.Item2) != "27")
                    {
                        continue; // don't process if we're not on a deibt transaction.
                    }
                    Tuple<int, int> values = FORMS.ENTRY_FORM.field_map["Amount     "]; //
                                                                                        // we'll want to handle the case for credit.
                    Int32.TryParse(entryAddenda[i].Substring(values.Item1, values.Item2), out curData); // this should be it.
                    debit_amount += curData;
                }
                catch (NullReferenceException NRE)
                {
                    Program.encountered_Exceptions.Add(NRE);
                    Console.WriteLine(NRE);
                    continue;
                }



            }
            return debit_amount;
        }


        /// <summary>
        /// Calculate the batch credit amount from the provided entryaddedna strings.
        /// </summary>
        /// <param name="entryAddenda"></param>
        /// <returns></returns>
        public static float CalculateBatchCreditAmount(string[] entryAddenda)
        {
            float debit_amount = 0; // we'll need to convert the type later.
            int curData = 0;
            for (int i = 0; i < entryAddenda.Length; i++)
            {
                try
                {
                    var toggle = FORMS.ENTRY_FORM.field_map["Trans. Code"];
                    if (entryAddenda[i].Substring(toggle.Item1, toggle.Item2) != "22")
                    {
                        continue; // don't process if we're not on a deibt transaction.
                    }
                    Tuple<int, int> values = FORMS.ENTRY_FORM.field_map["Amount     "]; //
                                                                                        // we'll want to handle the case for credit.
                    Int32.TryParse(entryAddenda[i].Substring(values.Item1, values.Item2), out curData); // this should be it.
                    debit_amount += curData;
                }
                catch (NullReferenceException NRE)
                {
                    Program.encountered_Exceptions.Add(NRE);
                    Console.WriteLine(NRE);
                    continue;
                }



            }
            return debit_amount;
        }


        /// <summary>
        /// Calculate the batch hash amount - this should suffice to function with a static list of entry addenda should we
        /// expand this api.
        /// </summary>
        /// <param name="entryAddenda">String array representing the body of the ACH file.</param>
        /// <returns>float representation of the raw batch hash - this could be reestablished to handle a string.</returns>
        public static float CalculateBatchHashAmount(string[] entryAddenda)
        {
            float hash_amount = 0;
            int curData = 0;
            for(int i = 0; i<entryAddenda.Length; i++)
            {
                try
                {


                    Tuple<int, int> values = FORMS.ENTRY_FORM.field_map["Routing Num"];
                    Int32.TryParse(entryAddenda[i].Substring(values.Item1, values.Item2), out curData);
                    // so long as this doesn't error out, then cur data should be able to return the component.
                    hash_amount += curData;
                }
                catch (NullReferenceException NRE)
                {
                    Program.encountered_Exceptions.Add(NRE);
                    Console.WriteLine(NRE);
                    continue;
                }

            }
            return hash_amount;
        }

        /// <summary>
        /// Get the debit amount from this instance of the file.
        /// </summary>
        /// <returns></returns>
        public float GetDebitAmount()
        {
            // we'll tie in to the static variant of these methods, but require less input.
            string[] d = new string[this.entryAddenda.Count];
            try
            {


                int i = 0;
                foreach (var a in this.entryAddenda.Keys)
                {
                    d[i] = this.entryAddenda[a].Item1; // use the entry
                    i++;
                }

            }
            catch(Exception e)
            {
                Program.encountered_Exceptions.Add(e);
#if DEBUG
                Console.WriteLine(e);
                System.Diagnostics.Debugger.Break();
#endif
            }
            return ACH_FILE.CalculateBatchDebitAmount(d);
        }

        /// <summary>
        /// Get the batch credit amount
        /// </summary>
        /// <returns></returns>
        public float GetCreditAmount()
        {

            // we'll tie in to the static variant of these methods, but require less input.
            string[] d = new string[this.entryAddenda.Count];
            try
            {


                int i = 0;
                foreach (var a in this.entryAddenda.Keys)
                {
                    d[i] = this.entryAddenda[a].Item1; // use the entry
                    i++;
                }

            }
            catch (Exception e)
            {
                Program.encountered_Exceptions.Add(e);
#if DEBUG
                Console.WriteLine(e);
                System.Diagnostics.Debugger.Break();
#endif
            }
            return ACH_FILE.CalculateBatchCreditAmount(d);
        }


        /// <summary>
        /// get the batch hash amount from this file.
        /// </summary>
        /// <returns></returns>
        public float GetBatchHash()
        {
            string[] d = new string[this.entryAddenda.Count];
            try
            {
                // we'll tie in to the static variant of these methods, but require less input.
                int i = 0;
                foreach (var a in this.entryAddenda.Keys)
                {
                    d[i] = this.entryAddenda[a].Item1; // use the entry
                    i++;
                }
            }
            catch(Exception e)
            {
                Program.encountered_Exceptions.Add(e);
#if DEBUG
                Console.WriteLine(e);
                System.Diagnostics.Debugger.Break();
#endif
            }
            return ACH_FILE.CalculateBatchHashAmount(d);
        }
#endif
    }
}
