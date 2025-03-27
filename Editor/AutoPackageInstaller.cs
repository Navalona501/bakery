using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

[InitializeOnLoad]
public class AutoPackageInstaller
{
    private const string PackageName = "ch.kainoo.extplugins.bakery";
    private const string PackageRelativePath = "Assets/settings.unitypackage"; // Relative path inside the package
    private const string CheckFilePath = "Assets/Settings/Bakery/ftLocalStorage.asset"; // A file to check if it's installed
    //private const string InstallFlagKey = "AutoPackageInstaller_Installed_unitask";
    private static bool isInstalled = false; // Static variable to track installation

    static AutoPackageInstaller()
    {
        // Reset flag on Unity startup
        isInstalled = false;

        // Check if package files exist
        if (File.Exists(CheckFilePath))
        {
            isInstalled = true;
        }

        if (!isInstalled)
        {
            Debug.Log("Package not installed. Searching for the .unitypackage...");
            FindAndImportPackage();
        }
    }

    private static void FindAndImportPackage()
    {
        var request = Client.List();
        EditorApplication.update += () => OnPackageListReceived(request);
    }

    private static void OnPackageListReceived(ListRequest request)
    {
        if (!request.IsCompleted) return;

        foreach (var package in request.Result)
        {
            if (package.name == PackageName)
            {
                string packagePath = package.resolvedPath;
                string unityPackagePath = Path.Combine(packagePath, PackageRelativePath);

                if (File.Exists(unityPackagePath))
                {
                    Debug.Log("Importing package: " + unityPackagePath);
                    AssetDatabase.ImportPackage(unityPackagePath, false);
                    //EditorPrefs.SetBool(InstallFlagKey, true); // Mark as installed
                }
                else
                {
                    Debug.LogError($"UnityPackage not found at: {unityPackagePath}");
                }
                break;
            }
        }

        EditorApplication.update -= () => OnPackageListReceived(request);
    }

   
}
