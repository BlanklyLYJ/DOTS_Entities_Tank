class CameraSingleton : UnityEngine.MonoBehaviour
{
    public static UnityEngine.Camera Instance;

    private void Awake()
    {
        Instance = GetComponent<UnityEngine.Camera>();
    }
}