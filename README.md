NUnit Project Editor
====================

Standalone editor for nunit project files.

Historically, the NUnit project editing function was incorporated in the Forms-based test runner, 
nunit.exe. This made editing of NUnit projects somewhat inconvenient for those not using the Gui 
and complicated the Gui code unnecessarily. 

Begining with NUnit 2.6, the project editor was made into a standalone executable in order to allow 
editing of projects outside the Gui environment. The Gui uses the standalone editor and will recognizes
changes to the open project by watching for any file changes.

For NUnit 3.0, the project editor is being managed as an independent project and will be delivered as a 
separate download, in addition to being bundled with the Gui.

