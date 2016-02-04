using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

	public static AudioManager s_Instance;

	private AudioSource m_BackgroundAudioSource;
	public AudioClip[] m_BackgroundTracks;
	
	private float m_BackgroundClipLength;
	private string m_CurrentSceneName;
	private int m_CurrentBackgroundTrackIndex;
	
	private static AudioSource[] m_MenuTracks;
	private static AudioSource[] m_PlayerMainTracks;
	private static AudioSource[] m_EnemyMainTracks;
	private static AudioSource[] m_SquadMainTracks;
	
	private static AudioClip[] m_EnemyChildTracks;
	
	private SceneType m_CurrentSceneType;
	private enum SceneType{Menu,Gameplay,Null};
	
	private bool m_SceneTransitionInProgress;

	public static AudioManager Instance {get{return s_Instance;}}

	// Use this for initialization
	void Start () 
	{
		if(s_Instance == null)
		{
			s_Instance = this;
		}
		if(GameObject.FindGameObjectsWithTag("AudioController").Length > 1){Destroy(gameObject);}
	
		DontDestroyOnLoad(gameObject);
	
		m_BackgroundAudioSource = GetComponent<AudioSource>();

		m_MenuTracks = new AudioSource[2];
		m_PlayerMainTracks = new AudioSource[14];
		m_EnemyMainTracks = new AudioSource[7];
		m_SquadMainTracks = new AudioSource[2];

		m_EnemyChildTracks = new AudioClip[5];
		
		m_CurrentSceneName = Application.loadedLevelName;
		m_SceneTransitionInProgress = false;
		
		LoadTracksToLists();
		LoadRandomBackgroundTrack();
		
		m_BackgroundAudioSource.volume = 0f;
		StartCoroutine(FadeInBackgroundMusic());
	}
	
	// Update is called once per frame
	void Update () 
	{
		//if the scene had been change, reload a random background track
		if(m_CurrentSceneName != Application.loadedLevelName && !Application.loadedLevelName.Contains("Tutorial") && !m_SceneTransitionInProgress)
		{
			StopCoroutine(FadeInBackgroundMusic());
			StopCoroutine(FadeOutBackgroundMusic());
			StartCoroutine(SceneTransition());
		}
	
		//Fading in/Fading out of BGM
		if(m_BackgroundAudioSource.time <= 3.5f && !m_SceneTransitionInProgress)
		{
			StartCoroutine(FadeInBackgroundMusic());
		}
		if(m_BackgroundAudioSource.time >= m_BackgroundClipLength - 3.5f && !m_SceneTransitionInProgress)
		{
			StartCoroutine(FadeOutBackgroundMusic());
		}
	}
	
	private void LoadTracksToLists()
	{
		int MenuTrackCount = 0;
		int PlayerMainTrackCount = 0;
		int EnemyMainTrackCount = 0;
		int SquadTrackCount = 0;
		
		for(int i = 0; i < transform.childCount; i++)
		{
			GameObject Child = transform.GetChild(i).gameObject;
			
			if(Child.name.Contains("Menu"))
			{
				m_MenuTracks[MenuTrackCount] = Child.GetComponent<AudioSource>();
				MenuTrackCount++;
			}
			else if(Child.name.Contains("Player"))
			{
				m_PlayerMainTracks[PlayerMainTrackCount] = Child.GetComponent<AudioSource>();
				PlayerMainTrackCount++;
			}
			else if(Child.name.Contains("Enemy"))
			{
				m_EnemyMainTracks[EnemyMainTrackCount] = Child.GetComponent<AudioSource>();
				EnemyMainTrackCount++;
			}
			else if(Child.name.Contains("Squad"))
			{
				m_SquadMainTracks[SquadTrackCount] = Child.GetComponent<AudioSource>();
				SquadTrackCount++;
			}
		}

		m_EnemyChildTracks[0] = (Resources.Load("Audio/Sound Effects/Enemy SFX/ADPCM/Enemy_CellChargeTowards",typeof(AudioClip)) as AudioClip);
		m_EnemyChildTracks[1] = (Resources.Load("Audio/Sound Effects/Enemy SFX/ADPCM/Enemy_Defend",typeof(AudioClip)) as AudioClip);
		m_EnemyChildTracks[2] = (Resources.Load("Audio/Sound Effects/Enemy SFX/ADPCM/Enemy_DeployLandmine",typeof(AudioClip)) as AudioClip);
		m_EnemyChildTracks[3] = (Resources.Load("Audio/Sound Effects/Enemy SFX/PCM/Enemy_LandmineBeeping",typeof(AudioClip)) as AudioClip);
		m_EnemyChildTracks[4] = (Resources.Load("Audio/Sound Effects/Enemy SFX/ADPCM/Enemy_LandmineExplode",typeof(AudioClip)) as AudioClip);
	}
	
	private SceneType DetermineCurrentSceneType()
	{
		if(Application.loadedLevelName.Contains("Menu"))
		{
			return SceneType.Menu;
		}
		else if(Application.loadedLevelName.Contains("Level"))
		{
			return SceneType.Gameplay;
		}
		return SceneType.Null;
	}
	
	public void LoadRandomBackgroundTrack()
	{
		m_CurrentSceneType = DetermineCurrentSceneType();
		
		if(m_CurrentSceneType == SceneType.Null){return;}
		
		int RandomTrackIndex = m_CurrentBackgroundTrackIndex;
		
		while(RandomTrackIndex == m_CurrentBackgroundTrackIndex)
		{
			if(m_CurrentSceneType == SceneType.Menu){RandomTrackIndex = Random.Range(0,3);}
			if(m_CurrentSceneType == SceneType.Gameplay){RandomTrackIndex = Random.Range(3,7);}
		}
		
		LoadAndPlayBackgroundTrack(m_BackgroundTracks[RandomTrackIndex]);
	
		if(RandomTrackIndex == 1 || RandomTrackIndex == 2){m_BackgroundClipLength = m_BackgroundTracks[RandomTrackIndex].length * 6f; return;}
		m_BackgroundClipLength = m_BackgroundTracks[RandomTrackIndex].length;
		
		m_CurrentBackgroundTrackIndex = RandomTrackIndex;
	}
	
	public void LoadAndPlayBackgroundTrack(AudioClip _Clip)
	{
		m_BackgroundAudioSource.Stop();
		m_BackgroundAudioSource.clip = _Clip;
		m_BackgroundAudioSource.Play();
	}
	
	private IEnumerator FadeInBackgroundMusic()
	{
		while(m_BackgroundAudioSource.volume < 1.0f)
		{
			m_BackgroundAudioSource.volume += 0.01f;
			yield return new WaitForSeconds(0.75f);
		}
	}
	
	private IEnumerator FadeOutBackgroundMusic()
	{
		while(m_BackgroundAudioSource.volume > 0.0f)
		{
			m_BackgroundAudioSource.volume -= 0.01f;
			yield return new WaitForSeconds(0.75f);
		}
		LoadRandomBackgroundTrack();
	}
	
	public void PlayMenuSoundEffect(MenuSFX _sfx)
	{
		if (m_MenuTracks == null)
			return;
		LoadTracksToLists();
		m_MenuTracks[(int) _sfx].Stop();
		m_MenuTracks[(int) _sfx].Play();
	}
	
	public static void PlayPMSoundEffect(PlayerMainSFX _sfx)
	{
		if (m_PlayerMainTracks == null)
			return;
		m_PlayerMainTracks[(int) _sfx].Stop();
		m_PlayerMainTracks[(int) _sfx].Play();
	}
	
	public static void PlayPMSoundEffectNoOverlap(PlayerMainSFX _sfx)
	{
		if (m_PlayerMainTracks == null)
			return;
		if(!m_PlayerMainTracks[(int) _sfx].isPlaying)
		{
			m_PlayerMainTracks[(int) _sfx].Stop();
			m_PlayerMainTracks[(int) _sfx].Play();
		}
	}
	
	public static void PlayEMSoundEffect(EnemyMainSFX _sfx)
	{
		if (m_EnemyMainTracks == null)
			return;
		m_EnemyMainTracks[(int) _sfx].Stop();
		m_EnemyMainTracks[(int) _sfx].Play();
	}
	
	public static void PlayECSoundEffect(EnemyChildSFX _sfx, AudioSource _Source)
	{
		if (m_EnemyChildTracks == null)
			return;
		_Source.Stop();
		_Source.clip = m_EnemyChildTracks[(int) _sfx];
		_Source.Play();	
	}
	
	public static void PlayEMSoundEffectNoOverlap(EnemyMainSFX _sfx)
	{
		if (m_EnemyMainTracks == null)
			return;
		if(!m_EnemyMainTracks[(int) _sfx].isPlaying)
		{
			m_EnemyMainTracks[(int) _sfx].Stop();
			m_EnemyMainTracks[(int) _sfx].Play();
		}
	}

	public static void PlaySquadSoundEffect(SquadSFX _sfx)
	{
		if (m_SquadMainTracks == null)
			return;

		m_SquadMainTracks[(int) _sfx].Stop();
		m_SquadMainTracks[(int) _sfx].Play();
	}
	
	private IEnumerator SceneTransition()
	{		
		m_SceneTransitionInProgress = true;
	
		while(m_BackgroundAudioSource.volume > 0.0f)
		{
			m_BackgroundAudioSource.volume -= 0.03f;
			yield return new WaitForSeconds(0.0025f);
		}
		
		LoadRandomBackgroundTrack();
		
		m_CurrentSceneName = Application.loadedLevelName;
		
		while(m_BackgroundAudioSource.volume < 1.0f)
		{
			m_BackgroundAudioSource.volume += 0.03f;
			yield return new WaitForSeconds(0.0025f);
		}
		
		m_SceneTransitionInProgress = false;
	}
	
	private IEnumerator EndGameTransition()
	{
		while(m_BackgroundAudioSource.volume > 0.2f)
		{
			m_BackgroundAudioSource.volume -= 0.03f;
			yield return new WaitForSeconds(0.0025f);
		}
	}
	
	public void SoftenInEndGame()
	{
		StartCoroutine(EndGameTransition());
	}
	
	public void PauseFadeOut()
	{
		while(m_BackgroundAudioSource.volume > 0.0f)
		{
			m_BackgroundAudioSource.volume -= 0.03f;
			StartCoroutine(WaitForRealSeconds(0.05f));
		}
		
		m_BackgroundAudioSource.Pause ();
	}
	
	public void PauseFadeIn()
	{
		m_BackgroundAudioSource.Play ();
		
		while(m_BackgroundAudioSource.volume < 1.0f)
		{
			m_BackgroundAudioSource.volume += 0.03f;
			StartCoroutine(WaitForRealSeconds(0.05f));
		}
	}
	
	private IEnumerator WaitForRealSeconds( float delay )
	{
		float start = Time.realtimeSinceStartup;
		while( Time.realtimeSinceStartup < start + delay )
		{
			yield return null;
		}
	}
	
	public void ReloadForSceneChange()
	{
		StartCoroutine(SceneTransition());
	}
}
