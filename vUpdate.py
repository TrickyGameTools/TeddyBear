"""   -- Start License block
***********************************************************
vUpdate.py
This particular file has been released in the public domain
and is therefore free of any restriction. You are allowed
to credit me as the original author, but this is not
required.
This file was setup/modified in:

If the law of your country does not support the concept
of a product being released in the public domain, while
the original author is still alive, or if his death was
not longer than 70 years ago, you can deem this file
"(c) Jeroen Broks - licensed under the CC0 License",
with basically comes down to the same lack of
restriction the public domain offers. (YAY!)
***********************************************************
Version 19.03.20
-- End License block   """


# This small script only serves to make my auto updater renumber all 
# version numbers correctly, but as I needed to prevent DOS vs Unix 
# conflicts :-/


from glob import glob
from os import system

def doIt(file):	
	system("txt2unix %s"%file)

system("zip -r -9 MKL_Backup/preunix . -i *.cs*")

for f in glob("TeddyLaunch/*.cs"):
    doIt(f)
for f in glob("TeddyWizard/*.cs"):
    doIt(f)
for f in glob("TeddyEdit/*.cs"):
    doIt(f)
for f in glob("TeddyEdit/Stages/*.cs"):
    doIt(f)


system("MKL_Update")
