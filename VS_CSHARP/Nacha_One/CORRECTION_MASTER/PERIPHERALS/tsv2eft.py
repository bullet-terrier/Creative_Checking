#
"""
TSV to EFT
cursory function to write an EFT function to TSV

Works well enough to get through the formatting process.
Doesn't create any header or footer information, but will provide the entry body based on the
provided TSV.

We'll try loading these in the clr.

"""

import sys;
sys.path.append("Lib");
import clr;
import sys;


fields = [1,2,8,1,17,10,15,22,2,1,15]

def padPrefunc(stringSource,length):
    if stringSource is None: stringSource="";
    if(len(stringSource)>length):
        stringSource=stringSource[len(stringSource)-length:];
    while(len(stringSource)<length):
        stringSource=" "+stringSource;
    return stringSource;

def padNumeric(stringSource,length):
    # adding this to pad the zeroes to the
    # numeric things
    if stringSource is None: stringSource="";
    if(len(stringSource)>length):
        stringSource=stringSource[len(stringSource)-length:]; # right justify
    while(len(stringSource)<length):
        stringSource="0"+stringSource;
    return stringSource;

def padFuncs(stringSource,length):
    if stringSource is None: stringSource=""
    if(len(stringSource)>length):
        stringSource=stringSource[:length];
    while(len(stringSource)<length):
        stringSource+=" ";
    return stringSource;

def tsvToETF(tsvString):
    """
    wasn't necessarily intending to use these in this format - but
    it looks like these were fine.
    not the most intuitive way to wrrite this, but it seems to work pretty well.
    """
    mStr = tsvString.split('\t');
    tStr = ""
    comp = "";
    # we'll need to revisit and upgrade this.
    for a in range(0,len(fields)-1):
        padFunc = lambda string, integer : padFuncs(string, integer);
        if a>=len(mStr):
            tStr = "";
        elif a in [2,5,8,9,10]:
            if '.' in mStr[a]: mStr[a] = mStr[a].split(".")[0]+mStr[a].split(".")[1]
            padFunc = lambda string, integer : padNumeric(string,integer);
            tStr = mStr[a];
        elif a in [6]:
            padFunc = lambda string, integer : padPrefunc(string,integer);
            tStr = mStr[a];
        else:
            tStr = mStr[a];
        comp+=padFunc(tStr,fields[a]);
    return comp;

def fullFunc(stringName):
    with open(stringName,'r') as src:
        with open(stringName+"_eft.ach",'w') as dst:
            m = src.readlines()
            z = []
            for a in m:
                z.append(tsvToETF(a))
                print(a);
            for a in z:
                dst.write(a+"\n");
                print(a);
        
if __name__=="__main__":
    for a in sys.argv[1:]:
        try:
            print(a);
            fullFunc(a)
        except Exception as e:
            print(e);
            print(e.stacktrace);
            print("NO HERE");
    