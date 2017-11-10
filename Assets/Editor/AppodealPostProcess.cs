using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using Unity.Appodeal.Xcode;
using Unity.Appodeal.Xcode.PBX;

using System;
using System.Diagnostics;
using System.IO;
using System.Collections;

#if UNITY_IPHONE
<<<<<<< HEAD
public class AppodealPostProcess : MonoBehaviour
{
=======
public class AppodealPostProcess : MonoBehaviour {
>>>>>>> soloappodealios
	private static string suffix = ".framework";
	private static string absoluteProjPath;

	#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
	private static string AppodealFramework = "Plugins/iOS/Appodeal.framework";
	private static string AppodealBundle = "Plugins/iOS/Appodeal.bundle";
	#endif

	private static string[] frameworkList = new string[] {
		"Twitter", "AdSupport", "AudioToolbox",
		"AVFoundation", "CoreFoundation", "CFNetwork",
		"CoreGraphics", "CoreImage", "CoreMedia",
		"CoreLocation", "CoreTelephony", "GLKit",
		"JavaScriptCore", "EventKitUI", "EventKit",
		"MediaPlayer", "MessageUI", "QuartzCore", 
		"MobileCoreServices", "Security", "StoreKit",
		"SystemConfiguration", "Twitter", "UIKit",
<<<<<<< HEAD
		"CoreBluetooth" 
=======
		"CoreBluetooth", "ImageIO"
>>>>>>> soloappodealios
	};

	private static string[] weakFrameworkList = new string[] {
		"CoreMotion", "WebKit", "Social"
	};


	private static string[] platformLibs = new string[] {
		"libc++.dylib",
		"libz.dylib",
		"libsqlite3.dylib",
		"libxml2.2.dylib"
	};

	[PostProcessBuild(100)]
<<<<<<< HEAD
	public static void OnPostProcessBuild (BuildTarget target, string pathToBuildProject)
	{		
=======
	public static void OnPostProcessBuild (BuildTarget target, string pathToBuildProject) {		
>>>>>>> soloappodealios
		if (target.ToString () == "iOS" || target.ToString () == "iPhone") {
			PrepareProject (pathToBuildProject);
			UpdatePlist(pathToBuildProject);
		}
<<<<<<< HEAD
=======

		#if UNITY_2017 || UNITY_2017_1_OR_NEWER
		if(PlayerSettings.GetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup).Equals("NET_2_0_Subset")) {
			if (EditorUtility.DisplayDialog("Appodeal Unity", "We have detected that you're using subset API compatibility level: " + PlayerSettings.GetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup) + " you should change it to NET_2_0 to be able using Ionic.ZIP.dll.", "Change it for me", "I'll do it")) {
				PlayerSettings.SetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup, ApiCompatibilityLevel.NET_2_0);
			}
		}
		#endif
>>>>>>> soloappodealios
	}

	private static void PrepareProject(string buildPath) {
		UnityEngine.Debug.Log("preparing your xcode project for appodeal");
		string projPath = Path.Combine (buildPath, "Unity-iPhone.xcodeproj/project.pbxproj");
		absoluteProjPath = Path.GetFullPath(buildPath);
		PBXProject project = new PBXProject ();
		project.ReadFromString (File.ReadAllText(projPath));
		string target = project.TargetGuidByName ("Unity-iPhone");

		AddProjectFrameworks (frameworkList, project, target, false);
		AddProjectFrameworks (weakFrameworkList, project, target, true);
		AddProjectLibs (platformLibs, project, target);
		project.AddBuildProperty (target, "OTHER_LDFLAGS", "-ObjC");
		project.AddBuildProperty (target, "ENABLE_BITCODE", "NO");
		project.AddBuildProperty (target, "LIBRARY_SEARCH_PATHS", "$(SRCROOT)/Libraries");

		string apdFolder = "Adapters";
		string fullPath = Path.Combine (Application.dataPath, string.Format ("Appodeal/{0}", apdFolder));
		if (Directory.Exists(fullPath)) {
			foreach (string file in System.IO.Directory.GetFiles(fullPath)) {
				if(Path.GetExtension(file).Equals(".zip")) {
<<<<<<< HEAD
=======
					UnityEngine.Debug.Log("unzipping:"+file);
>>>>>>> soloappodealios
					ExtractZip (file, Path.Combine (absoluteProjPath, apdFolder));
					AddAdaptersDirectory (apdFolder, project, target);
				}
			}
		}

		#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
		project.AddBuildProperty (target, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Frameworks/Plugins/iOS");
		project.SetBuildProperty (target, "LIBRARY_SEARCH_PATHS", "$(SRCROOT)/Libraries");
		CopyAndReplaceDirectory ("Assets/" + AppodealFramework, Path.Combine(buildPath, "Frameworks/" + AppodealFramework));
		project.AddFileToBuild(target, project.AddFile("Frameworks/" + AppodealFramework, "Frameworks/" + AppodealFramework, PBXSourceTree.Source));
		CopyAndReplaceDirectory ("Assets/" + AppodealBundle, Path.Combine(buildPath, "Frameworks/" + AppodealBundle));
		project.AddFileToBuild(target,  project.AddFile("Frameworks/" + AppodealBundle,  "Frameworks/" + AppodealBundle, PBXSourceTree.Source));
		#endif

		File.WriteAllText (projPath, project.WriteToString());
	}

<<<<<<< HEAD
	protected static void AddProjectFrameworks(string[] frameworks, PBXProject project, string target, bool weak)
	{
=======
	protected static void AddProjectFrameworks(string[] frameworks, PBXProject project, string target, bool weak) {
>>>>>>> soloappodealios
		foreach (string framework in frameworks) {
			if (!project.HasFramework (framework)) {
				project.AddFrameworkToProject (target, framework + suffix, weak);
			}
		}
	}

<<<<<<< HEAD
	protected static void AddProjectLibs(string[] libs, PBXProject project, string target)
	{
=======
	protected static void AddProjectLibs(string[] libs, PBXProject project, string target) {
>>>>>>> soloappodealios
		foreach (string lib in libs) {
			string libGUID = project.AddFile ("usr/lib/" + lib, "Libraries/" + lib, PBXSourceTree.Sdk);
			project.AddFileToBuild (target, libGUID);
		}	
	}

<<<<<<< HEAD
	private static void UpdatePlist (string buildPath)
	{
=======
	private static void UpdatePlist (string buildPath) {
>>>>>>> soloappodealios
		#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
		string plistPath = Path.Combine (buildPath, "Info.plist");
		PlistDocument plist = new PlistDocument ();
		plist.ReadFromString(File.ReadAllText (plistPath));	
		PlistElementDict dict = plist.root.CreateDict ("NSAppTransportSecurity");
		dict.SetBoolean ("NSAllowsArbitraryLoads", true);
		File.WriteAllText(plistPath, plist.WriteToString());
		#endif
	}

<<<<<<< HEAD
	private static void CopyAndReplaceDirectory(string srcPath, string dstPath)
	{
=======
	private static void CopyAndReplaceDirectory(string srcPath, string dstPath) {
>>>>>>> soloappodealios
		if (Directory.Exists(dstPath)) {
			Directory.Delete(dstPath);
		}
		if (File.Exists(dstPath)) {
			File.Delete(dstPath);
		}

		Directory.CreateDirectory(dstPath);

		foreach (var file in Directory.GetFiles(srcPath)) {
			if(!file.Contains(".meta")) {
				File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));
			}
		}

		foreach (var dir in Directory.GetDirectories(srcPath)) {
			CopyAndReplaceDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
		}
	}

<<<<<<< HEAD
	private static void ExtractZip(string filePath, string destFolder){
		using(Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(filePath)){			
=======
	private static void ExtractZip(string filePath, string destFolder) {
		using(Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(filePath)) {			
>>>>>>> soloappodealios
			foreach(Ionic.Zip.ZipEntry z in zip){
				z.Extract(destFolder, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
			}
		}
	}

<<<<<<< HEAD
	private static void AddAdaptersDirectory(string path, PBXProject proj, string targetGuid)
	{
=======
	private static void AddAdaptersDirectory(string path, PBXProject proj, string targetGuid) {
>>>>>>> soloappodealios
		if (path.EndsWith ("__MACOSX",StringComparison.CurrentCultureIgnoreCase))
			return;	

		if (path.EndsWith (".framework", StringComparison.CurrentCultureIgnoreCase)) {
			proj.AddFileToBuild (targetGuid, proj.AddFile (path, path));
			string tmp = Utils.FixSlashesInPath(string.Format ("$(PROJECT_DIR)/{0}", path.Substring (0, path.LastIndexOf (Path.DirectorySeparatorChar))));
			proj.AddBuildProperty (targetGuid, "FRAMEWORK_SEARCH_PATHS", tmp);
			return;
		} else if(path.EndsWith (".bundle", StringComparison.CurrentCultureIgnoreCase)){			
			proj.AddFileToBuild (targetGuid, proj.AddFile (path, path));
			return;
		}

		string fileName;
		bool libPathAdded = false;
		bool headPathAdded = false;

		string realDstPath = Path.Combine (absoluteProjPath, path);
		foreach (var filePath in Directory.GetFiles(realDstPath)) {
			fileName = Path.GetFileName (filePath);

			if (fileName.EndsWith (".DS_Store"))
				continue;

			proj.AddFileToBuild (targetGuid, proj.AddFile (Path.Combine (path, fileName), Path.Combine (path, fileName), PBXSourceTree.Source));
<<<<<<< HEAD
			if(!libPathAdded && fileName.EndsWith(".a")){				
=======
			if(!libPathAdded && fileName.EndsWith(".a")) {				
>>>>>>> soloappodealios
				proj.AddBuildProperty(targetGuid, "LIBRARY_SEARCH_PATHS", Utils.FixSlashesInPath(string.Format("$(PROJECT_DIR)/{0}", path)));			
				libPathAdded = true;	
			}

<<<<<<< HEAD
			if(!headPathAdded && fileName.EndsWith(".h")){				
=======
			if(!headPathAdded && fileName.EndsWith(".h")) {				
>>>>>>> soloappodealios
						proj.AddBuildProperty(targetGuid, "HEADER_SEARCH_PATHS", Utils.FixSlashesInPath(string.Format("$(PROJECT_DIR)/{0}", path)));			
				headPathAdded = true;	
			}
		}

		foreach (var subPath in Directory.GetDirectories(realDstPath)){	
			AddAdaptersDirectory(Path.Combine(path,Path.GetFileName(subPath)), proj, targetGuid);
		}
	}
<<<<<<< HEAD

=======
>>>>>>> soloappodealios
}
#endif