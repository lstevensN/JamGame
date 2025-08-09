using UnityEngine;

public class Settings : MonoBehaviour
{
    private void Start()
    {
        if (TryGetComponent(out ResolutionSettings resolution)) resolution.Toggle();
    }
}
