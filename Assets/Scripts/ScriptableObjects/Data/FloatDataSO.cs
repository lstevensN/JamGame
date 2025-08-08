using UnityEngine;

//[CreateAssetMenu(fileName = "FloatData", menuName = "Scriptable Objects/Float Data")]
[CreateAssetMenu(fileName = "FloatData", menuName = "Scriptable Objects/Data/FloatData")]

public class FloatDataSO : ValueData<float>
{
    /// <summary>
    /// Implicit conversion operator to int.
    /// </summary>
    public static implicit operator float(FloatDataSO variable)
    {
        return variable != null ? variable.Value : 0;
    }

    //float value;
}
