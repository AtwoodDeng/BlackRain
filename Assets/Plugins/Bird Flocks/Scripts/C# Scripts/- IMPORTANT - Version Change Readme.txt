#
#
#	First consider making a BACKUP of project before changing script language.
#	The following procedure will replace scripts. 
#	
#	
#	HOW TO CHANGE FROM JS TO C SHARP:
#
#		To use the C# scripts: 
#			Delete the 7 JS scripts BEFORE importing.
#				- Delete:
#					-FlockChild.js
#					-FlockChildSound.js		-- This file might be located in another folder called "Bird Flock Sound Addon"
#					-FlockController.js
#					-FlockWaypointTrigger.js
#					-LandingButtons.js
#					-LandingSpot.js
#					-LandingSpotController.js
#				- Double Click "Bird Flock C Scripts".
#				- Import.
#
#
#		
#		To change back to JS scripts:
#			Delete the 7 C# scripts BEFORE importing.
#				- Delete:
#					-FlockChild.cs
#					-FlockChildSound.cs
#					-FlockController.cs
#					-FlockWaypointTrigger.cs
#					-LandingButtons.cs
#					-LandingSpot.cs
#					-LandingSpotController.cs
#				- Double Click "Bird Flock JS Scripts".
#				- Import.
#		
#			- or -
#
#				Re-import the scripts from the Unity asset package.
#
#
#		If a replaced file is not deleted before importing the C# version will have .JS extension.
#			This will produce errors.
#			Simply Delete the file and import C# version again.