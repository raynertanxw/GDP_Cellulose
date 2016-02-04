using UnityEngine;
using System.Collections;

public class MutantSprawlRenderer : MonoBehaviour 
{
    // Eidatable Fields
    [Tooltip("The array to store all the object that is going to pulse")]
    [SerializeField] private Transform[] array_Transform;

    // Uneditable Fields
    private Animate[] array_Animate;
    private Animate mAnimate;

    void Start()
    {
        array_Animate = new Animate[array_Transform.Length];
        mAnimate = new Animate(this.transform);

        for (int i = 0; i < array_Animate.Length; i++)
        {
            array_Animate[i] = new Animate(array_Transform[i]);
            array_Animate[i].ExpandContract(1000f, 500, 1.3f);
        }
    }
}
