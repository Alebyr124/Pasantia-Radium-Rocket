using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class FirebaseInit : MonoBehaviour
{
    public static DatabaseReference DBreference;

    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                DBreference = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("✅ Firebase inicializado correctamente");
            }
            else
            {
                Debug.LogError($"❌ No se pudo inicializar Firebase: {dependencyStatus}");
            }
        });
    }
}
