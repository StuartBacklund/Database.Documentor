Installing Database Documenter

	- Click on the MSI file named Database,Documentor.msi
	- Follow the instructions as prompted
	- A shortcut icon will be placed on the machine desktop




	- Once installation is complete, right click on either the desktop shortcut icon
	Or the executable file in the installed location, default is C:\Program Files (x86)\Database Documentor
	- Under the compatibility tab, select run as Adminstrator. 
	- This is important as files are written to the file system and without the required permissions the application will not work properly
	

![image](https://github.com/user-attachments/assets/c662692d-1885-45b9-8a24-a77da279f454)
Running the Application

	- Double Clicking on the icon will start the application
	- A simple UI will appear once the application has loaded
	- There are only certain required fields, most of them is an either or choice

Instance Dropdown / Textbox

	- On Start up the combobox might be populated with database server instances known to your machine
	- These instances are dependent on the required permissions assigned to your machine to access those database servers
	- If no items are present in the dropdown, the textbox on the right allows for typing in your server name

SQL Authentication

- If your server needs sql authenitication credentials, enter them in the textboxes provided
- If they are not required, check the integrated security chekbox
- Once either an instance or server name has been entered, and credentials option selected, click "List Databases"
- A list of available databases will populated the dropdown
- Select the database to be used by the application
- Click "Build"


Results
	- If successful the application will display an activity log 
![image](https://github.com/user-attachments/assets/bdf8d74a-2553-4c5e-997b-56dec55e0f60)
Application Output

	- Navigate to the application's location on yur machine
	- Default location is shown in the screenshot below
	- Open the output folder
	
	
	
	
	
	
	CHM documentation
	- A chm file is created by the application which utilizes Mcrosofts HtmllHelp feature
	- This file is compressed, has additional feature like searching





MSDN Style Web Help Documentation

	- Extract the zipped archive folder in a location of your choosing, be aware that extraction in the current location needs adminstrator permissions
	- Copy all the image files (.gif), the js and css files into the extracted folder
	- Open the html file with Index in it's name




![image](https://github.com/user-attachments/assets/7dd77e06-33c8-40a8-afd5-421dead6efae)
