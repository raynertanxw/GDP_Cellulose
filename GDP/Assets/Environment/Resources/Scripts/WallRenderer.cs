using UnityEngine;
using System.Collections;

public enum WallRendererType { SideWall, Background };
// WallRenderer.cs: Handles the rendering and the movement effect of the wall (THIS SCRIPT DOES NOT CONTAIN THE COLLISION OF THE WALL)
public class WallRenderer : MonoBehaviour
{
    // Static Fields
    private static WallRenderer s_lastWallSideInstance = null;          // s_lastWallSideInstance: The most recent wall side instance
    private static WallRenderer s_lastWallBackgroundInstance = null;    // s_lastWallBackgroundInstance: The most recent wall background instance

    // Editable Fields
    [SerializeField] private WallRendererType m_WallRendererType;       // m_WallRendererType: Is the wall a side-wall or a background

    // Uneditable Fields
    private Color colorArtillery;           // colorArtillery: The color of the blood vessel. The color will be only defined at the START()
    private float fMovementSpeed = 0f;      // fMovementSpeed: The speed in which the blood vessel is parallax-scrolling
    private float fSpriteLength = 0f;       // fSpriteLength: The y-length of the sprite
    private bool bIsSpawnNext = false;      // bIsSpawnNext: Used for stuff

    // Component/GameObject References
    private SpriteRenderer m_spriteRenderer = null;    // m_SpriteRenderer: The reference to the sprite renderer
                                                // NOTE: USE THIS AS A GETTER ONLY!
                                                //       USE GetComponent<SpriteRenderer>() TO SET VALUES!

    // Private Functions
    // Start(): Use this for initialisation
    void Start()
    {
        // Fields Initialisation
        colorArtillery = Wall.Instance.ArtilleryColor;

        if (m_WallRendererType == WallRendererType.SideWall)
        {
            // Since both left and right wall is the same, m_SpriteRenderer will get information from the left wall only
            m_spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            for (int i = 0; i < 2; i++)
            {
                transform.GetChild(i).GetComponent<SpriteRenderer>().color = colorArtillery;
            }
        }
        else
        {
            m_spriteRenderer = GetComponent<SpriteRenderer>();
            GetComponent<SpriteRenderer>().color = colorArtillery;
        }

        fSpriteLength = m_spriteRenderer.bounds.size.y - 1.5f;

        transform.position = Constants.s_farfarAwayVector;
        this.enabled = false;
    }

    // OnEnable(): is called when the gameObject becomes active
    public void BecomesEnable()
    {
        if (m_WallRendererType == WallRendererType.SideWall)
        {
            if (Wall.Instance != null)
                fMovementSpeed = Wall.Instance.WallSidesSpeed;
            // if: Spawns the new instance based on the position of the last instance
            if (s_lastWallSideInstance == null)
            {
                transform.position = Vector3.zero;
            }
            else
                transform.position = s_lastWallSideInstance.transform.position + new Vector3(0f, fSpriteLength, 0f);

            // Sets this to the most recent instance
            s_lastWallSideInstance = this;
        }
        else
        {
            if (Wall.Instance != null)
                fMovementSpeed = Wall.Instance.WallBackgroundSpeed;

            // if: Spawns the new instance based on the position of the last instance
            if (s_lastWallBackgroundInstance == null)
                transform.position = Vector3.zero;
            else
                transform.position = s_lastWallBackgroundInstance.transform.position + new Vector3(0f, fSpriteLength, 0f);

            // Sets this to the most recent instance
            s_lastWallBackgroundInstance = this;
        }
    }

    // FixedUpdate(): is called 20 times every second. Used for physics handling
    void Update()
    {
		// if: This needed so to prevent error during reset
		if (Wall.Instance == null)
			return;

        if (this.enabled)
        {
            transform.position -= new Vector3(0f, fMovementSpeed, 0f) * Time.deltaTime;

            if (!bIsSpawnNext && transform.position.y < 0f)
            {
                if (m_WallRendererType == WallRendererType.SideWall)
                    Wall.Instance.SpawnWallSides();
                else
                    Wall.Instance.SpawnWallBackground();
                bIsSpawnNext = true;
            }

            // if: The camera can no longer see this sprite, disable the script
            if (transform.position.y < -20f)
            {
                transform.position = Constants.s_farfarAwayVector;
                this.enabled = false;
                bIsSpawnNext = false;
            }
        }
    }










	public static void ResetStatics()
	{
		s_lastWallBackgroundInstance = null;
		s_lastWallSideInstance = null;
	}
}
