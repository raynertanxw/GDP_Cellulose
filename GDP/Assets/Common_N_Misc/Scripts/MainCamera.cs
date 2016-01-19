using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour 
{
    // Static Fields
    public static MainCamera m_Instance = null;

    // Uneditable Fields
    private static bool bIsShaking = false;     // bIsShaking: Returns if the camera is shaking
    private static float nShake = 0;           // fShake: The number of shake the camera will perform
    private static float fIntensity = 0f;       // fIntensity: The intensity of the shake
    private static Vector3 vCameraPosition;      // vCameraPosition: The initial camera position

    // Co-Routines
    // ShakeRoutine(): The co-routine that handles the shaking
    IEnumerator ShakeRoutine()
    {
        for (int i = 0; i < nShake; i++)
        {
            transform.position = vCameraPosition + new Vector3((UnityEngine.Random.value * 2f - 1f) * fIntensity, (UnityEngine.Random.value * 2f - 1f) * fIntensity, 0.0f);
            yield return new WaitForSeconds(0.05f);
        }
        transform.position = vCameraPosition;

        // Change Variables back to defaults
        nShake = 0;
        fIntensity = 0.0f;
        bIsShaking = false;
    }

    // Private Functions
    // Awake(): is called at the start of the program
    void Awake()
    {
        // Singleton
        if (m_Instance == null)
            m_Instance = this;
        else
            Destroy(this.gameObject);
    }

    // Start(): Use this for initialisation
    void Start()
    {
        vCameraPosition = transform.position;
    }

    // Public Static Functions
    /// <summary>
    /// Shakes the camera
    /// </summary>
    /// <returns> Returns if the current request shake is executed </returns>
    public static bool CameraShake()
    {
        if (!bIsShaking)
        {
            CameraShake(7, 0.3f);
            return true;
        }
        else return false;
    }

    /// <summary>
    /// Shakes the camera
    /// </summary>
    /// <param name="_nShake"> The number of shakes to perform </param>
    /// <param name="_fIntensity"> The shake intensity of the camera </param>
    /// <returns> Returns if the current request shake is executed </returns>
    public static bool CameraShake(int _nShake, float _fIntensity)
    {
        nShake = _nShake;
        fIntensity = _fIntensity;

        // if: The camera is not shaking currently
        if (!bIsShaking)
        {
            bIsShaking = true;
            m_Instance.StartCoroutine("ShakeRoutine");
            return true;
        }
        else return false;
    }
}
