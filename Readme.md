# CreativeChecking
A NACHA File balancing and editing utility -

# USE AT YOUR OWN RISK

* Provides a basic interface for correcting fields in NACHA Text Files
* Provides hashing and checksumming for the files.


# Goals:
* Clean up the code within this application to make it more usable
* Reformat it to run across multiple platforms
* Make user experience better when dealing with these files.


# About

This project has been naturally evolving over the last year or so in response to a variety of pitfalls
involved in ACH processing with a plethora of banks.

This project is primarily aimed at allowing agents responsible for monitoring files to be able to edit them
with maximum efficiency.

My goal with turning the project open source is to gain better insights into ways to make the library more efficient,
as well as allow people that need a utility like this to have easy access.

Currently, it only operates on microsoft's .NET platform, favoring version 4+.

Primarily written in C#, I'm moving to replace the core functionality with IronPython, to increase portability.


