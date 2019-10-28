#define HIGHLIGHT_LEGACY
//
//
// we're going to set these in the compilation versions.
// I'm leaving these in the code files where they are necessary, in case you need to manually compile this without the IDE.
//
//
using System;
using System.Collections.Generic;
using System.Windows.Forms;


// changes:
//   - 2019-07-23: Adding in a search submodule and form.
//   - 2019-07-23: Restructured configuration scheme - now configurations are more dependent on the IDE configurationmanager.
//   - 2019-07-23: Adding documentation xml strings to all public members - this will allow IDE generated documentation.
//   - 2019-07-23: Breaking the file up into larger regional chunks for future maintenance.
//   - 2019-07-25: Completed search integration with "ACCESS_RECORD" - can now move seamlessly through the search object to the editor.
//   - 2019-07-25: Adding option to interrupt the immediate edit to allow for deletion on off chance that deletion is necessary.
//   - 2019-08-07: Updated the search mechanism and several other files with updated docstrings.
//   - 2019-08-07: Updated display for the landing page to show file totals.
//   - 2019-08-13: Updating with an additional method for editing the ACH file in a more direct output.
//   
//
// todo:
//   - *add mechanism for creating a new file.
//   - adding mechanism to allow handling duplicates in the editor.
//   - add a search function to this page.
//   - add an update unit to the dictionary/listbox.
//   - add autosave based on intervals
//   - add undo feature for an arbitrary number of frames. --> this will likely spawn a submodule to try and allow it to be resurrected later.
//   - add capability to reselect the same key from the original frame
//   - add delete functionality to the "ENTRY_FORM" form object.
//
namespace CORRECTION_MASTER.FORMS
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ACCESS_RECORD : Form
    {
#if DEBUG
        int toggleCounter = 1;
#endif
        #region pre_7_18_2019
        private string selected_key { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ACH_FILE working_file { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, Tuple<string, string>> display_keys { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Obsolete("Burried within buttonObjectClick for now.")]
        public void AddNewEntry()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        [Obsolete("Burried within buttonObjectClick for now.")]
        public void DeleteEntry()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        [Obsolete("Burried within buttonObjectClick for now.")]
        public void WriteFile()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ButtonObjectClick(object sender, EventArgs e)
        {
            if (sender.Equals(button1))
            {
                // might want to update the key - so I'll have to look at ways to do that.
                Tuple<string, string> Curr_Key = this.display_keys[selected_key];
                // edit entry/addenda
                Tuple<string, string> My_Entry = working_file.entryAddenda[this.display_keys[selected_key]];
                int pos = 0;
                foreach (var a in working_file.entryAddenda)
                {
                    if (a.Value.Equals(My_Entry))
                    {
                        break;
                    }
                    else
                    {
                        pos++;
                    }
                }
                ENTRY_FORM My_Form = new ENTRY_FORM(My_Entry.Item1, My_Entry.Item2, false, pos);
                My_Form.ShowDialog();
                string new_entry = ENTRY_FORM.last_entry; // we'll handle this after the fact.
#if DEBUG
                Console.WriteLine(new_entry); // alrighty - confirmed that we can write output back to file.
#endif
                working_file.entryAddenda[Curr_Key] = new Tuple<string, string>(new_entry, My_Entry.Item2);

                // okay - so if the amount has changed, we'll trigger the update unit.
#if UPDATE_20190723
                // trying an "Update every time" approach.
                // there appears to be a negligible difference in this...
#if DEBUG
                var dbgtime = DateTime.Now;
                if (toggleCounter % 2 == 0)
                {
                    if (working_file.entryAddenda[this.display_keys[selected_key]].Item1.Substring(FORMS.ENTRY_FORM.field_map["Amount     "].Item1, FORMS.ENTRY_FORM.field_map["Amount     "].Item2) !=
                    working_file.entryAddenda[this.display_keys[selected_key]].Item2)
                    {
                        // okay - I think that this should update the output.
                        working_file.updateEntryAddenda(this.display_keys[this.selected_key], working_file.entryAddenda[Curr_Key]);
                    }
                    Console.WriteLine($"W/IF {DateTime.Now - dbgtime}");
                    this.label3.Text = $"W/IF {DateTime.Now - dbgtime}";
                }
                else
                {
                    {
                        // okay - I think that this should update the output.
                        working_file.updateEntryAddenda(this.display_keys[this.selected_key], working_file.entryAddenda[Curr_Key]);
                    }
                    Console.WriteLine($"N/IF {DateTime.Now - dbgtime}");
                    this.label3.Text = $"N/IF {DateTime.Now - dbgtime}";
                }
                toggleCounter++; // so we'll try benchmarking it.

                
#else
                //if (working_file.entryAddenda[this.display_keys[selected_key]].Item1.Substring(FORMS.ENTRY_FORM.field_map["Amount     "].Item1, FORMS.ENTRY_FORM.field_map["Amount     "].Item2) !=
                //    working_file.entryAddenda[this.display_keys[selected_key]].Item2)
                {
                    // okay - I think that this should update the output.
                    working_file.updateEntryAddenda(this.display_keys[this.selected_key], working_file.entryAddenda[Curr_Key]);
                }
#endif

#endif

#if UPDATE_20190718
                DisplayKeysUpdate(); // I think this will work...
                ListBoxItemsUpdate(); // let's see where this goes.
                MidInterrupt(); // we'll try this one then.
#endif
            }
            else if (sender.Equals(button2))
            {
                // quit
                this.DestroyHandle(); // remove self. the ACH_FILE class should hold all of the changes.
            }
            else if(sender.Equals(button3))
            {
                // add
                string new_entry = "";
                string new_addenda = "";
                ENTRY_FORM My_Form = new ENTRY_FORM("", "", false, display_keys.Count); // will be the last record available.
                My_Form.ShowDialog(); // get the new entry value.
                new_entry = ENTRY_FORM.last_entry; // not thread safe.
#if DEBUG
                Console.WriteLine(new_entry);
#endif
                working_file.add_entry(new string[2] { new_entry,new_addenda }, 0); // will always be the first entry.

                // we'll need to update keys.
                this.DisplayKeysUpdate();
                this.ListBoxItemsUpdate();
            }
            else if (sender.Equals(button4))
            {
                // delete
                // we'll need to update keys.
                working_file.entryAddenda.Remove(display_keys[selected_key]);
                this.DisplayKeysUpdate();
                this.ListBoxItemsUpdate();
            }
            else
            {
                // suppress error?
            }
        }


        /// <summary>
        /// DisplayKeysUpdate -
        /// Update the key amounts based on the amounts contained in the line item.
        /// </summary>
        public void DisplayKeysUpdate()
        {
            this.display_keys.Clear(); // display keys are fine - but we need to edit working file.
#if UPDATE_20190723
#if DEBUG
            // we're going to try the 
            List<Tuple<string, string>> workingFileKeys = new List<Tuple<string, string>>();
            foreach(Tuple<string,string>tss in working_file.entryAddenda.Keys)
            {
                workingFileKeys.Add(tss);
            }
            // okay - with the keys obtained - we're going to get the entry addedna value.
#endif
#endif
            foreach (Tuple<string, string> a in working_file.entryAddenda.Keys)
            {
                // set it up to pretty print the amount and pair with the name.
                // it almost looked like item 2 was assigned as amount.
#if UPDATE_20190723
                // updating this to only show a more manageable output value.
                string my_key = $"{GLOBALS.PadString(a.Item1,15)},\t{ENTRY_FORM.padding_router("Amount     ", a.Item2, true)}";
#else
                string my_key = $"{a.Item1},\t{ENTRY_FORM.padding_router("Amount     ", a.Item2, true)}";
#endif
                try
                {
                    display_keys.Add(my_key, a); // update with the new keys amount.
                }
                catch (Exception e)
                {
#if UPDATE_20190807
                    Program.encountered_Exceptions.Add(e);
#endif
                    // looks like this case is that key is duplicate - we'll need to adjust the key.
                    my_key = $"[DUPLICATE]{my_key}";
                    display_keys.Add(my_key, a);
                }// this will help identify duplicates while we're at it.
            }
        }

        // so this updates the ListboxItems- which I should look into.
        /// <summary>
        /// 
        /// </summary>
        public void ListBoxItemsUpdate()
        {
            this.listBox1.Items.Clear(); // clear them all.
            foreach(string a in this.display_keys.Keys)
            {
                this.listBox1.Items.Add(a);
            }
            // we'll tie in the update method for the textboxes.
#if UPDATE_20190807
            this.updateTextAmounts(); // should be good to go.
#endif
        }

        // we'll need to pass the listbox object amounts here.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ListBoxItemClick(object sender, EventArgs e)
        {
            // in the event that we click an item - load the requisite entry in the entry box.
            // get the key.
            string matched_key="";
            foreach(string a in display_keys.Keys)
            {
#if DEBUG
                Console.WriteLine(a);
#endif
                // this.ListBox1.SelectedItem is null - this is what was happening.
                if (this.listBox1.SelectedItem.Equals(a)) { matched_key = a; break; }
            }
            // get the value for item 1.
            this.selected_key = matched_key;
            this.textBox1.Text = working_file.entryAddenda[this.display_keys[matched_key]].Item1;


            //
        }

#if UPDATE_20190723
        private string getDisplayKeyFromValue(Tuple<string,string> value)
        {
            string key = "";
            foreach(var key_ in this.display_keys.Keys)
            {
                if (display_keys[key_].Equals(value))
                {
                    key = key_;
                    break;
                }
            }
            return key;
        }

        private void searchInteractionClick(object sender, EventArgs e)
        {
            try
            {


                // let's see what we want to push through here.
                var m = new ENTRY_SEARCH();
                m.setSourceKeyVals(this.working_file.entryAddenda); // this should now contain the entry/addenda necessary.
                m.setSourceSearch(this.display_keys);
                m.ShowDialog();
                if (m.selectedKey != null)
                {
                    this.listBox1.ClearSelected();
                    var k1 = m.selectedKey; // 
                    // I need to add a copy of my keyselection unit to this module - which will be fine.
                    // add a simplification step to this, where we can call it from the parent program.
                    //this.listBox1.SelectedItem = m.selectedKey; // okay - so m.selectedkey is my current tuple - but my listbox uses a string representation
                    try
                    {
                        var k2 = this.getDisplayKeyFromValue(k1); // I think this should work.
                        this.listBox1.SelectedItem = k2; // we'll try this.
                    }
                    catch
                    {

                    }
//                    this.listBox1.SelectedItems.Add(m.selectedKey); // this should work.
                    // okay - this is where I'm hitting the error - let's look further into ListBoxItemClick;
                    ListBoxItemClick(sender, e); // should update the display.

                    // limit the immediate execution to only if selected by a user to do so.
                    if (Program.AUTOMATICALLY_OPEN_SEARCHED_ENTRY)
                    {
                        ButtonObjectClick(this.button1, e); // send this event.
                    }
                    
                }
                else
                {
                    Console.WriteLine("Looks like something happened here...");
                }
            }
            catch(Exception ex)
            {
                // Don't do anything but clear out I guess.
#if DEBUG
                Console.WriteLine(ex);
#endif
            }
        }
#endif


        // UPDATING 7-18-2019 - removing some of the input parameters (reforming through preprocessor commands.
        // allowing these commands to be processed later will allow us to construct most of the form, while separating it from the underlying code.
        /// <summary>
        /// 
        /// </summary>
        public ACCESS_RECORD(
#if UPDATE_20190718
#else
            ACH_FILE search_file
#endif
            )
        {
            this.display_keys = new Dictionary<string, Tuple<string, string>>(); // forgot that this needed to be initialized.
#if UPDATE_20190718
            // following portion immediately attempts to establish the keys on initialization - 
            // we're going to set up a process to set the keys up as a prerequisite to display - 
            // possibly with an override to the "Show()" and "ShowDialog()" methods.
#else
            this.working_file = search_file;
            foreach(Tuple<string,string> a in working_file.entryAddenda.Keys)
            {
                // this is the location that causes an issue.
                // set it up to pretty print the amount and pair with the name.
                // it almost looked like item 2 was assigned as amount.
                /*
                 */
                string my_key = $"{a.Item1},\t{ENTRY_FORM.padding_router("Amount     ", a.Item2, true)}";
                try
                {
                    display_keys.Add(my_key, a); // update with the new keys amount.
                }
                catch(Exception e)
                {
                    // we'll, I would think this should capture the duplicate keys.
                    // looks like this case is that key is duplicate - we'll need to adjust the key.
                    my_key = $"[DUPLICATE]{my_key}";
                    display_keys.Add(my_key, a);
                }// this will help identify duplicates while we're at it.
            }
#endif
            // this function has been modified in the 20190723 update.
            InitializeComponent();

            this.label1.Text = "Selected Record";
            this.label2.Text = "Original Names and Amounts:";
            this.label3.Text = "Amounts to left will show the original names and amounts remaining\nin the file. As you add and delete, they will appear to the left.\nEdits can be viewed in the window above.";
            this.button1.Text = "Edit";
            this.button2.Text = "Save and Quit";
            this.button3.Text = "Add";
            this.button4.Text = "Delete";
#if UPDATE_20190723
#if UPDATE_20190725
            foreach(Control c in this.Controls)
            {
                Program.set_font(c); // let's see if this works.
            }
#if UPDATE_20190807
            //this.label4.Text = "File Hash";
            this.label4.Text = "File Credit Amount"; // we'll do the same output for this as we did with the debit.
            this.label5.Text = "File Debit Amount"; // focusing on how much we are drafting, not paying (for now);
            this.textBox2.ReadOnly = true;
            this.textBox3.ReadOnly = true; // we'll need to add some update methods.
#endif
#endif

            this.button5.Text = "Search";
            this.button5.Click += searchInteractionClick;

            // let's try this on for size.
            // we're going to need to outsource these as a function...
            //this.listBox1.Font = new System.Drawing.Font("Consolas",10);

#endif

            this.textBox1.Multiline = true;
            this.textBox1.ScrollBars = ScrollBars.Horizontal;
            this.textBox1.ReadOnly = true;

            this.listBox1.SelectedIndexChanged += ListBoxItemClick;// let's give this a go
            // 
            foreach(string a in this.display_keys.Keys)
            {
                listBox1.Items.Add(a);
            }

            foreach(Control c in this.Controls)
            {
                if (c.Name.Contains("button"))
                {
                    c.Click += ButtonObjectClick; // this should handle all of the clicks in a centralized place.
                }
            }


        }
#endregion
#region added_7_18_2019
#if UPDATE_20190718
        // adding this section for the new constructor module, that should make the old version obsolete.
        private void original_key_format(ACH_FILE search_file)
        {
            this.working_file = search_file;
            foreach (Tuple<string, string> a in working_file.entryAddenda.Keys)
            {
                string my_key = $"{a.Item1},\t{ENTRY_FORM.padding_router("Amount     ", a.Item2, true)}";
                try
                {
                    display_keys.Add(my_key, a); // update with the new keys amount.
                }
                catch (Exception e)
                {
#if UPDATE_20190807
                    Program.encountered_Exceptions.Add(e);
#endif
                    string e_Key = $"{my_key}";
                    int allowed_runs = 100;
                    bool m = false;
                    while(m!=true)
                    {
                        try
                        {
                            e_Key = $"[DUPLICATE]{e_Key}";
                            display_keys.Add(e_Key, a); // this should be fine.
                            m = true;
                            break; // not relying on the system to work correctly.
                        }
                        catch (Exception e2)
                        {
#if UPDATE_20190807
                            Program.encountered_Exceptions.Add(e2);
#endif
                            // this would be the alternative.
                            if (allowed_runs <= 0) { break; }
                            allowed_runs--;
                        }
                    }
                }
            }
        }

        // attempt tems:
        /// <summary>
        /// 
        /// </summary>
        private void MidInterrupt()
        {
            this.listBox1.Items.Clear();
            foreach(string a in this.display_keys.Keys)
            {
                listBox1.Items.Add(a);
            }
        }

        /// <summary>
        /// Add anything that needs to run after this object has been initialized,
        /// but before the form should be used.
        /// </summary>
        private void Interrupt()
        {
            // objects that need to be called for this variant.
            this.listBox1.Items.Clear(); // update the items.
            foreach (string a in this.display_keys.Keys)
            {
                listBox1.Items.Add(a);
            }
            // this should belong down here to avoid the issues presented with NRES.
#if UPDATE_20190807
            this.updateTextAmounts();
#endif
        }

        /// <summary>
        /// Use this instead of the "Show" method.
        /// </summary>
        /// <param name="search_file"></param>
        public void InterruptShow(ACH_FILE search_file)
        {
            original_key_format(search_file);
            Interrupt();
            this.Show();
        }

        /// <summary>
        /// use this instead of the "ShowDialog" method.
        /// </summary>
        /// <param name="search_file"></param>
        public void InterruptShowDialog(ACH_FILE search_file)
        {
            original_key_format(search_file); // these versions should keep *most* of the behavior the same
            Interrupt();
            this.ShowDialog();
        }
#endif

#endregion
#if UPDATE_20190723

        //  probably a better expression for this unit.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        [Obsolete("This is new, but already obsolete - Won't be expanded on in future official releases.")]
        public List<Control> traverseControls(Control parent)
        {
            List<Control> myCon = new List<Control>();
            myCon.Add(parent);
            if(parent.Controls.Count>0)
            {
                foreach(Control c in parent.Controls)
                {
                    myCon.AddRange(traverseControls(c)); // that should handle everything there.
                }
            }
            return myCon;
        }

        // I could just return the dictionary objet itself, which could be a faster representation.
        // just going to give this a go.
        /// <summary>
        /// Get a dictionary representing all of the controls and their provided names.
        /// We can use this as a hacky way to trigger certain events.
        /// </summary>
        /// <returns></returns>
        [Obsolete("Won't be expanded upon in official future releases. May have some debugging utilities.")]
        public Dictionary<string,Control> getControlsByNames()
        {
            Dictionary<string, Control> controlNames = new Dictionary<string, Control>();
            foreach(Control con in traverseControls(this))
            {
                try
                {
                    controlNames.Add(con.Name, con);
                }
                catch(InvalidOperationException IOE)
                {
                    // I believe this should catch duplicate keys...
#if UPDATE_20190807
                    Program.encountered_Exceptions.Add(IOE);
#endif
                }
                
            }
            return controlNames;
        }

#endif
        // additional methods based on the update setup.
#if UPDATE_20190807
            /// <summary>
            /// Update the amounts displayed by text - added in the most recent update file.
            /// </summary>
        protected void updateTextAmounts()
        {
            try
            {


                // reset the amounts for the hash and debit amounts - might replace hash with credit amount down the line.
                // this will increase the class coupling more than likely.
                float debt = this.working_file.GetDebitAmount();
                string d = debt.ToString();
                if (d.Length < 2)
                {
                    d = $"$00.{d}";
                }
                else
                {
                    d = $"${d.Substring(0, d.Length - 2)}.{d.Substring(d.Length - 2, 2)}";
                }
                this.textBox3.Text = d;

                // rearranging to see if the amount is functioning and we're erroring out on the hash portion.
                //float hash = this.working_file.GetBatchHash();
                float hash = this.working_file.GetCreditAmount();
                // we can recycle d to string now - since we've already updated the other box.
                d = hash.ToString();
                //h = h.Substring(h.Length - 10, 10); // this might throw an error I'll double check at runtime.
                if (d.Length < 2)
                {
                    d = $"$00.{d}";
                }
                else
                {
                    d = $"${d.Substring(0, d.Length - 2)}.{d.Substring(d.Length - 2, 2)}";
                }
                this.textBox2.Text = d;
                // Let's see what happens.
            }
            catch(Exception e)
            {
                Program.encountered_Exceptions.Add(e);
            }
        }
#endif

    }


#if UPDATE_20190807 // removing the unecessary search module, slated for complete removal on next update.
#else
    // I'm not sure that anything necessary was actually added in this class -Might push to a different code file.
#if UPDATE_20190723
#region 20190723_Expansion
    /// <summary>
    /// 
    /// </summary>
    public class SEARCH_MODULE
    {
        // todo:
        //   - add in hooks to the ACCESS_RECORD controls    
        //   - update the visibility or add accessor methods to necessary controls.
        //   - add a hook to manage the "ctrl+f" shortcut - which we could then use to launch the search function.

        // might want to change this depending on how memory intensive the item is.
        // It won't throw an error now, which is a nice change.
        ACCESS_RECORD source_search = new ACCESS_RECORD();
        ENTRY_SEARCH search_view = new ENTRY_SEARCH(); // so we can create an entry search unit - make sure we can pass the data back to the ACCESS_RECORD.

        // okay - capture the entries, then return them in a list to be used as necessary.
        /// <summary>
        /// Capture entries, then return them in a list to be used as necessary.
        /// </summary>
        public List<Tuple<string, string>> returned_entries = new List<Tuple<string, string>>();


        /// <summary>
        /// Accessor method for the source_search object
        /// </summary>
        /// <param name="source"></param>
        public void set_source(ACCESS_RECORD source)
        {
            source_search = source;
        }

        /// <summary>
        /// Accessor method for the search_view object;
        /// </summary>
        /// <param name="search"></param>
        public void set_view(ENTRY_SEARCH search)
        {
            search_view = search;
        }

        /// <summary>
        /// Show the search_view object as a result of the button press.
        /// </summary>
        public void getSearchEntry()
        {
            this.search_view.ShowDialog(); //
        }

        // need to come up with a list of valid search conditions...
        /// <summary>
        /// get entries based on the provided values.
        /// </summary>
        /// <param name="searchEntry">determine the type of search we are using</param>
        /// <param name="searchValue">the value by which we are searching.</param>
        public void getEntries(string searchEntry, string searchValue)
        {
            List<Tuple<string, string>> matched_entries = new List<Tuple<string, string>>(); //
            Tuple<int, int> dex = new Tuple<int, int>(0, 0);
            switch(searchEntry)
            {
                case "amount":
                    dex = ENTRY_FORM.field_map["Amount     "];
                    break;
                case "name":
                    dex = ENTRY_FORM.field_map["Rec. Name  "];
                    break;
                case "account":
                    dex = ENTRY_FORM.field_map["Account Num"];
                    break;
                case "entry":
                    //dex = ENTRY_FORM.field_map[""];
                    // we're not going to let this one work.
                    ////
                    dex = new Tuple<int, int>(0, -1); // we'll see what this does.
                    break;
                default:
                    goto case "amount";
#if UPDATE_20190807 // removing extraneous breaks
#else
                    break; // I know it isn't used, but it isn't hurting anybody.
#endif
            }

            // let's see here - I need to iterate through the entries I have now.
            foreach(var tss in source_search.display_keys.Values)
            {
                var mss = source_search.working_file.entryAddenda[tss]; // so this should be the line that we're looking at.
                //
                if (dex.Item2 < 1)
                {
                    dex = new Tuple<int, int>(dex.Item1, mss.Item1.Length); // set this up so that dex reads the entire line.
                }
                //
                // get the mapped values from that com
                if(mss.Item1.Substring(dex.Item1,dex.Item2).Contains(searchValue))
                {
                    matched_entries.Add(mss); // so mss should be the entry/addenda from the current file.
                }
            }
            this.returned_entries = matched_entries; // adding the extra layer just in case there's something we need to do to the dataset.
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        public SEARCH_MODULE()
        {

        }

    }
#endregion
#endif
#endif

}
