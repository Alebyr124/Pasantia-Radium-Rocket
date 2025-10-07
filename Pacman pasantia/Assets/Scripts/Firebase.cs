using Firebase;
using Firebase.Database;
using UnityEngine;

public class FirebaseInit : MonoBehaviour
{
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                var app = FirebaseApp.DefaultInstance;

                // 👉 En vez de modificar Options, se lo pasamos directamente al GetInstance
                var db = FirebaseDatabase.GetInstance(app, "https://pacmanrr-43e78-default-rtdb.firebaseio.com/");

                Debug.Log("Firebase Database inicializado correctamente ✅");

                // Probamos escribir un dato de prueba
                db.RootReference.Child("test").SetValueAsync("Hola Firebase desde Unity");
            }
            else
            {
                Debug.LogError("No se pudieron resolver las dependencias de Firebase: " + task.Result);
            }
        });
    }
}
