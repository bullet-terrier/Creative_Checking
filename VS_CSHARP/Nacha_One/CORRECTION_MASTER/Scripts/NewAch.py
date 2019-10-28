#
"""
short mechanism to generate an empty ACH file - 
we'll be using more scripts like this to populate things that
"""

import clr;
import datetime;
import sys; # we'll use this to see if we are loading it as a main loop.

# adjust these fields based on the needs of the company - should be a one time configuration.
# file configurations.
globaltoday = datetime.datetime.now().strftime("%y%m%d%H%M");# get the creationdatetime.\
immediateDestination = "";
immediateOrigin = "";
fileidModifier = "";
referencecode = "";
immediateDestinationname = "My Bank";
immediateOriginName = "My Company"

def padBasic(string,length):
    if(len(string)>length): string = string[:length];
    while(len(string)<length): string+=" ";
    return string;

def padNum(string,length):
    if(len(string)>length): string = string[len(string)-length:];
    while(len(string)<length): string = '0'+string;
    return string;

def padLft(string, length):
    if(len(string)>length): string = string[len(string)-length:];
    while(len(string)<length): string= ' '+string;
    return string;

# these are the best ways to toss them into the file header.
globaltoday = padBasic(globaltoday,10);
immediateDestination = padLft(immediateDestination,10);
immediateOrigin = padLft(immediateOrigin,10);
fileidModifier = padLft(fileidModifier,1);
immediateDestinationname = padBasic(immediateDestinationname,23);
immediateOriginName = padBasic(immediateOriginName,23);
referencecode = padLft(referencecode,8); 

# we just want to create a new file from here - return the total result from this document.
def newFileHeader():
    return """101%s%s%s094101%s%s%s"""%(immediateDestination,immediateOrigin,globaltoday,fileidModifier,immediateDestinationname,immeidateOriginname);

def newBatchHeader():
    return """"""


if __name__=="__main__":
    print("This is actually executing as main, even when embedded. Sweet.");
