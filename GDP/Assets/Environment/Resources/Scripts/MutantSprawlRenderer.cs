using UnityEngine;
using System.Collections;

public class MutantSprawlRenderer : MonoBehaviour 
{
    // Editable Fields
    [Tooltip("The array to store all the object that is going to pulse")]
    [SerializeField] private Transform[] array_Transform;

    // Static Fields
    private static MutantSprawlRenderer s_Instance;

    // Uneditable Fields
    private Animate[] array_Animate;
    private Animate mAnimate;

    // Private Functions
    void OnDestroy()
    {
        s_Instance = null;
    }

    // Awake(): Calls at the start of the program
    void Awake()
    {
        if (s_Instance == null)
            s_Instance = this;
        else
            Destroy(this.gameObject);
    }

    // Start(): Us this for initialisation
    void Start()
    {
        array_Animate = new Animate[array_Transform.Length];
        mAnimate = new Animate(this.transform);

        for (int i = 0; i < array_Animate.Length; i++)
        {
            array_Animate[i] = new Animate(array_Transform[i]);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PulseSprawl();
        }

        for (int i = 0; i < array_Animate.Length; i++)
        {
            if (!array_Animate[i].IsExpandContract)
                array_Animate[i].ExpandContract(4f, 1, 1.1f);
        }
    }

    // PulseSprawl(): Pulse the sprawl once
    public void PulseSprawl()
    {
        mAnimate.ExpandContract(0.5f, 1, 1.1f, true, 0.5f);
    }

    // Getter-Setter Functions
    public static MutantSprawlRenderer Instance { get { return s_Instance; } }
}
