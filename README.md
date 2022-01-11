#### SpotYoink
- Simple terminal/console tool that lets you grab those amazing wallpapers Windows uses for its login screens.
---
##### How to use
- Simply open up your Windows console/terminal and type in **yoink**
- Follow the on screen instructions and you should have a new folder within your *pictures* folder named *Spotlight* with your new wallpapers
	- The login screen wallpapers don't update very often, so try to use the command maybe once a week *I've programmed in a daily useage just incase*

###### How it works
- Windows actually stores the wallpapers it uses for the login screen temporarily in a folder located at:`Appdata\Local\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets";
- The files don't have an extension and are a mixed batch of different sizes/aspect ratios
- What this tool does:
	-  it grab those files
	-  convert them to a jpeg format
	-  Filter out the non 1920 x 1080 files
	-  Copy them all to the new folder stated above

###### Dependencies 
- >= [.NETCORE 3.1 RUNTIME](https://download.visualstudio.microsoft.com/download/pr/4e95705e-1bb6-4764-b899-1b97eb70ea1d/dd311e073bd3e25b2efe2dcf02727e81/dotnet-runtime-3.1.22-win-x64.exe) 

