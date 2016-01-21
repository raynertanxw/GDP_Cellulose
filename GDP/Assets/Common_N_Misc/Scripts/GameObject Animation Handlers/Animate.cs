using UnityEngine;
using System.Collections;
using System.ComponentModel;

// Animate.cs: Handles all transform positional and rotational animations
public class Animate
{
    // Animation Definition Fields
    private float fExpandContract_Timer = 0f;           // fExpandContract_Timer: The time taken to complete the expand-contract sequence
    private int nExpandContract_Frequency = 0;          // nExpandContract_Frequency: The number of times the object pulses throughout the entire sequence
    private float fExpandContract_Size = 0f;            // fExpandContract_Size: The biggest size the object can get when it expands
    private Vector3 fExpandContract_InitialScale;       // fExpandContract_InitialScale: The initial scale of object
    private bool bExpandContract_IsOverridable = true;  // bExpandContract_IsOverridable: Determies if the current animation can be overriden

    // Timing Fields
    private float fExpandContract_CurrentTimer = 0f;    // fExpandContract_CurrentTimer: The current time for the expand-contract sequence

    // Transform Reference
    private Transform mTransform;                       // _mTransform: The transform reference of the object

    // Boolean Fields
    private bool bIsExpandContract = false;             // bIsExpandContract: Returns if the object is performing an expand-contract sequence

    // Constructor
    /// <summary>
    /// Enabling of animation effects
    /// </summary>
    /// <param name="_mTransform"> The transform of the object that is going to animate </param>
    public Animate(Transform _mTransform)
    {
        mTransform = _mTransform;
        fExpandContract_InitialScale = mTransform.localScale;
    }

    // Public Functions
    /// <summary>
    /// Performs the animation of the object expanding and contracting
    /// </summary>
    /// <param name="_fTimer"> The total time of the animation </param>
    /// <param name="_nFrequency"> The number of expansion and contraction it animates </param>
    /// <param name="_fSize"> The maximum size of the expansion, relative to the initial scale of the object when this method is called </param>
    /// <returns> Returns if the animation is executed </returns>
    public bool ExpandContract(float _fTimer, int _nFrequency, float _fSize)
    {
        return ExpandContract(_fTimer, _nFrequency, _fSize, true, 0.0f);
    }

    /// <summary>
    /// Performs the animation of the object expanding and contracting
    /// </summary>
    /// <param name="_fTimer"> The total time of the animation </param>
    /// <param name="_nFrequency"> The number of expansion and contraction it animates </param>
    /// <param name="_fSize"> The maximum size of the expansion, relative to the initial scale of the object when this method is called </param>
    /// <param name="_bIsOverridable"> Determines if this animation can be overriden by another function call </param>
    /// <param name="_fStartTime"> Use this function to start at a specific part of the animation. (0f = Start, 1f = End) </param>
    /// <returns> Returns if the animation is executed </returns>
    public bool ExpandContract(float _fTimer, int _nFrequency, float _fSize, bool _bIsOverridable, float _fStartTime)
    {
        // if: The animation is overridable
        if (bExpandContract_IsOverridable)
        {
            // if: It is not currently expanding and contracting, which means that this is a new animation and not an overriden one
            if (!bIsExpandContract)
            {
                // if: This animation cannot be passed to AnimateHandler.cs (cache is probably full)
                if (!AnimateHandler.ActivateExpandContract(this))
                {
                    return false;
                }
            }

            // Initialisation of Expand-Contract Animation Fields
            fExpandContract_Timer = _fTimer;
            nExpandContract_Frequency = _nFrequency * 2;
            fExpandContract_Size = _fSize - 1f;
            fExpandContract_CurrentTimer = _fStartTime * fExpandContract_Timer;
            bExpandContract_IsOverridable = _bIsOverridable;

            bIsExpandContract = true;
            return true;
        }
        return false;
    }

    // UpdateExpandContract(): The update call for expand-contract animation sequence.
    //                         Returns true when sequence is not complete, false for otherwise
    //[EditorBrowsable(EditorBrowsableState.Never)]
    public bool UpdateExpandContract(float _fDeltaTime)
    {
        // Update current time
        fExpandContract_CurrentTimer += _fDeltaTime;

        if (fExpandContract_CurrentTimer < fExpandContract_Timer)
        {
            // fCompletion: Returns the value of completion (0.0f = Begin, 1.0f = End)
            float fCompletion = fExpandContract_CurrentTimer / fExpandContract_Timer;

            mTransform.localScale =
                fExpandContract_InitialScale * (1f + Mathf.PingPong(fCompletion * (float)nExpandContract_Frequency * fExpandContract_Size, fExpandContract_Size));
            return true;
        }
        else
        {
            // Transform size back to default
            mTransform.localScale = fExpandContract_InitialScale;

            // Set all value back to zero
            fExpandContract_Timer = 0f;
            nExpandContract_Frequency = 0;
            fExpandContract_Size = 0f;
            fExpandContract_CurrentTimer = 0f;
            bExpandContract_IsOverridable = true;

            bIsExpandContract = false;
            return false;
        }
    }

    // Getter-Setter Functions
    public bool IsExpandContract { get { return bIsExpandContract; } }
}
