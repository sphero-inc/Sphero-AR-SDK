using UnityEngine;
using UnityEditor;
using System.IO;
 
// Create a Sublime Text 2 project from a Unity project
// Includes folders and file types of your choosing
// Includes all assemblies for autocompletion in CompleteSharp package
// adapted from http://forum.unity3d.com/threads/128352-Using-Unity-with-Sublime-Text-2-(How-to-get-everything-set-up)/page2
 
public class SyncSublimeText2 : Editor
{
	// Put all Assets subfolders you want to include here
	private static string[] includeFolders = new[]{"/Scripts/", "/Shaders/", "/Plugins/"};
	
	// Put all extensions you want to include here
	private static string[] includeExtensions = new[]{"cs", "js", "txt", "shader", "cginc", "xml", "c", "cpp", "m", "mm", "h", "hpp"};
 
	// Put a custom path for the Unity Managed DLLs here, if you want one
	private const string customUnityDLLPath = "";
 
	private static string unityDLLPath = "";
 
	[MenuItem("Assets/Sync Sublime Text 2 Project")]
	static void SyncST2Project()
	{
		// Set the managed DLL path
		if(customUnityDLLPath != "")
		{
			unityDLLPath = customUnityDLLPath;
		}
		else
		{
			if (Application.platform == RuntimePlatform.OSXEditor)
			{
				unityDLLPath = EditorApplication.applicationContentsPath + "/Frameworks/Managed/";
			}
			else
			{
				unityDLLPath = EditorApplication.applicationContentsPath + "/Managed/";
			}
		}
 
		// Output file string
		string outFile = "";
		// Output file location
		string outFolder = Application.dataPath.Substring(0, Application.dataPath.Length - 7);
		// Get folder name for current project
		string projectFolderName = outFolder.Substring(outFolder.LastIndexOf("/") + 1);
 
		// Add project folders
		outFile = "{\n";
		outFile += "\t\"folders\":\n";
		outFile += "\t[\n";
 
		for(int n = 0; n < includeFolders.Length; n++)
		{
			string cFolder = includeFolders[n];
 
			outFile += "\t\t{\n";
			outFile += "\t\t\t\"file_include_patterns\":\n";
			outFile += "\t\t\t[\n";
			
			for(int i = 0; i < includeExtensions.Length; i++)
			{
				string cExtension = includeExtensions[i];
 
				outFile += "\t\t\t\t\"*." + cExtension + "\"";
				if(i != includeExtensions.Length-1)
				{
					outFile += ",";
				}
				outFile += "\n";
			}
 
			outFile += "\t\t\t],\n";
			outFile += "\t\t\t\"path\": \"" + Application.dataPath + cFolder + "\"\n";
			outFile += "\t\t}";
 
			if(n != includeFolders.Length-1)
			{
				outFile += ",";
			}
			outFile += "\n";
		}
 
		outFile += "\t],\n";
		outFile += "\n";
 
		// Add autocompletion assemblies
		outFile += "\t\"settings\":\n";
		outFile += "\t{\n";
		outFile += "\t\t\"completesharp_assemblies\":\n";
		outFile += "\t\t[\n";
		outFile += "\t\t\t\"" + unityDLLPath + "UnityEngine.dll\",\n";
		outFile += "\t\t\t\"" + unityDLLPath + "UnityEditor.dll\",\n";
		
		outFile += "\t\t\t\"" + Application.dataPath + "/../Library/ScriptAssemblies/Assembly-CSharp.dll\",\n";
		outFile += "\t\t\t\"" + Application.dataPath + "/../Library/ScriptAssemblies/Assembly-CSharp-Editor.dll\"";
 
		string[] dllFiles = Directory.GetFiles(Application.dataPath, "*.dll", SearchOption.AllDirectories);
		
		if(dllFiles.Length > 0)
		{
			outFile += ",\n";
		}
		else
		{
			outFile += "\n";
		}
 
		foreach(string file in dllFiles)
		{
			outFile += "\t\t\t\"" + file + "\"";
			if(file != dllFiles[dllFiles.Length-1])
			{
				outFile += ",";
			}
			outFile += "\n";
		}
		outFile += "\t\t]\n";
		outFile += "\t}\n";
		outFile += "}\n";
 
		// Write the file to disk
		StreamWriter sw = new StreamWriter(outFolder + "/" + projectFolderName + ".sublime-project");
		sw.Write(outFile);
		sw.Close();
	}
}