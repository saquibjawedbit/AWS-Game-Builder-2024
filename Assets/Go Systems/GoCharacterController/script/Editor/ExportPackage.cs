using UnityEngine;
using UnityEditor;
using System.Security.Principal;
public class ExportPackage
{
    [MenuItem("Export/MyExport")]
    static void export()
    {
        AssetDatabase.ExportPackage(AssetDatabase.GetAllAssetPaths(), PlayerSettings.productName + ".unitypackage", ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies | ExportPackageOptions.IncludeLibraryAssets);
    }
    [MenuItem("Import/MyImport")]
    static void Import()
    {

        string operatingSystem = SystemInfo.operatingSystem;
        if (operatingSystem.ToLower().Contains("win"))
        {
            try
            {
                string packagePath = @"C:\Users\" + System.Environment.UserName + @"\AppData\Roaming\Unity\Asset Store-5.x\Go Systems\Complete ProjectsSystems\Go Systems Third Person Controller Template.unitypackage";
                RestSystems(packagePath);

            }
            catch
            {

                Debug.LogWarning("Import canceled. Please select a valid package.");
            }
        }else if (operatingSystem.ToLower().Contains("mac"))
        {
            try
            {
                string packagePath ="~/ Library / Unity / Asset Store - 5.xGo Systems/Complete ProjectsSystems/Go Systems Third Person Controller Template.unitypackage";
                RestSystems(packagePath);
            }
            catch
            {
                Debug.LogWarning("Import canceled. Please select a valid package.");
            }
        }
    }

   static void RestSystems(string Path)
    {
        AssetDatabase.ImportPackage(Path, true);
        Debug.Log("Settings package imported successfully.");
    }

}
public class ImportAssetStorePackage : EditorWindow
{
    private string packagePath = "";

    [MenuItem("Assets/Import Asset Store Package")]
    private static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ImportAssetStorePackage));
    }

    private void OnGUI()
    {
        GUILayout.Label("Import Asset Store Package", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Package Path:");
        packagePath = EditorGUILayout.TextField(packagePath);
        if (GUILayout.Button("Browse", GUILayout.Width(80)))
        {
            packagePath = EditorUtility.OpenFilePanel("Select Asset Store Package", "", "unitypackage");
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        if (GUILayout.Button("Import Package"))
        {
            if (!string.IsNullOrEmpty(packagePath))
            {
                AssetDatabase.ImportPackage(packagePath, true);
                Debug.Log("Package imported successfully.");
               
            }
            else
            {
                Debug.LogWarning("Import canceled. Please select a valid package.");
            }
        }
    }
}