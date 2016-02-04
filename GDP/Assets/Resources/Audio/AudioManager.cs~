using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

	public static AudioManager s_Instance;

	private AudioSource BackgroundAudioSource;
	public AudioClip[] BackgroundTracks;
	
	private float BackgroundClipLength;
	private string CurrentSceneName;
	
	private static AudioSource[] MenuTracks;
	private static AudioSource[] PlayerMainTracks;
	private static AudioSource[] EnemyMainTracks;
	private static AudioSource[] SquadMainTracks;
	
	private static AudioClip[] EnemyChildTracks;
	private static AudioClip[] SquadChildTracks;
	
	private SceneType CurrentSceneType;
	private enum SceneType{Menu,Gameplay,Null};
	
	private bool SceneTransitionInProgress;

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
	
		BackgroundAudioSource = GetComponent<AudioSource>();

		MenuTracks = new AudioSource[2];
		PlayerMainTracks = new AudioSource[14];
		EnemyMainTracks = new AudioSource[7];
		SquadMainTracks = new AudioSource[2];

		EnemyChildTracks = new AudioClip[5];
		SquadChildTracks = new AudioClip[2];
		
		CurrentSceneName = Application.loadedLevelName;
		SceneTransitionInProgress = false;
		
		LoadTracksToLists();
		LoadRandomBackgroundTrack();
		
		BackgroundAudioSource.volume = 0f;
		StartCoroutine(FadeInBackgroundMusic());
	}
	
	// Update is called once per frame
	void Update () 
	{
		//if the scene had been change, reload a random background track
		if(CurrentSceneName != Application.loadedLevelName && !SceneTransitionInProgress)
		{
			StopCoroutine(FadeInBackgroundMusic());
			StopCoroutine(FadeOutBackgroundMusic());
			StartCoroutine(SceneTransition());
		}
	
		//Fading in/Fading out of BGM
		if(BackgroundAudioSource.time <= 3.5f && !SceneTransitionInProgress)
		{
			StartCoroutine(FadeInBackgroundMusic());
		}
		if(BackgroundAudioSource.time >= BackgroundClipLength - 3.5f && !SceneTransitionInProgress)
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
				MenuTracks[MenuTrackCount] = Child.GetComponent<AudioSource>();
				MenuTrackCount++;
			}
			else if(Child.name.Contains("Player"))
			{
				PlayerMainTracks[PlayerMainTrackCount] = Child.GetComponent<AudioSource>();
				PlayerMainTrackCount++;
			}
			else if(Child.name.Contains("Enemy"))
			{
				EnemyMainTracks[EnemyMainTrackCount] = Child.GetComponent<AudioSource>();
				EnemyMainTrackCount++;
			}
			else if(Child.name.Contains("Squad"))
			{
				SquadMainTracks[SquadTrackCount] = Child.GetComponent<AudioSource>();
				SquadTrackCount++;
			}
		}

		EnemyChildTracks[0] = (Resources.Load("Audio/Sound Effects/Enemy SFX/ADPCM/Enemy_CellChargeTowards",typeof(AudioClip)) as AudioClip);
		EnemyChildTracks[1] = (Resources.Load("Audio/Sound Effects/Enemy SFX/ADPCM/Enemy_Defend",typeof(AudioClip)) as AudioClip);
		EnemyChildTracks[2] = (Resources.Load("Audio/Sound Effects/Enemy SFX/ADPCM/Enemy_DeployLandmine",typeof(AudioClip)) as AudioClip);
		EnemyChildTracks[3] = (Resources.Load("Audio/Sound Effects/Enemy SFX/PCM/Enemy_LandmineBeeping",typeof(AudioClip)) as AudioClip);
		EnemyChildTracks[4] = (Resources.Load("Audio/Sound Effects/Enemy SFX/ADPCM/Enemy_LandmineExplode",typeof(AudioClip)) as AudioClip);
		
		Debug.Log("SquadMainTracks count: " + SquadMainTracks.Length);
		for(int i = 0; i < SquadMainTracks.Length; i++){Debug.Log(SquadMainTracks[i].name);}
		
		/*Debug.Log("PlayerChildTrack count: " + PlayerChildTracks.Length);
		for(int i = 0; i < PlayerChildTracks.Length; i++){Debug.Log(PlayerChildTracks[i].name);}
		
		Debug.Log("EnemyChildTracks count: " + EnemyChildTracks.Length);
		for(int i = 0; i < EnemyChildTracks.Length; i++){Debug.Log(EnemyChildTracks[i].name);}
		
		Debug.Log("SquadChildTracks count: " + SquadChildTracks.Length);
		for(int i = 0; i < SquadChildTracks.Length; i++){Debug.Log(SquadChildTracks[i].name);}*/
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
		CurrentSceneType = DetermineCurrentSceneType();
		
		if(CurrentSceneType == SceneType.Null){return;}
		
		int RandomTrackIndex = 0;
		
		if(CurrentSceneType == SceneType.Menu){RandomTrackIndex = Random.Range(0,3);}
		if(CurrentSceneType == SceneType.Gameplay){RandomTrackIndex = Random.Range(3,7);}
		
		LoadAndPlayBackgroundTrack(BackgroundTracks[RandomTrackIndex]);
	
		if(RandomTrackIndex == 1 || RandomTrackIndex == 2){BackgroundClipLength = BackgroundTracks[RandomTrackIndex].length * 6f; return;}
		BackgroundClipLength = BackgroundTracks[RandomTrackIndex].length;
	}
	
	public void LoadAndPlayBackgroundTrack(AudioClip _Clip)
	{
		BackgroundAudioSource.Stop();
		BackgroundAudioSource.clip = _Clip;
		BackgroundAudioSource.Play();
	}
	
	private IEnumerator FadeInBackgroundMusic()
	{
		while(BackgroundAudioSource.volume < 1.0f)
		{
			BackgroundAudioSource.volume += 0.01f;
			yield return new WaitForSeconds(0.75f);
		}
	}
	
	private IEnumerator FadeOutBackgroundMusic()
	{
		while(BackgroundAudioSource.volume > 0.0f)
		{
			BackgroundAudioSource.volume -= 0.01f;
			yield return new WaitForSeconds(0.75f);
		}
		LoadRandomBackgroundTrack();
	}
	
	public void PlayMenuSoundEffect(MenuSFX _sfx)
	{
		if (MenuTracks == null)
			return;
		LoadTracksToLists();
		MenuTracks[(int) _sfx].Stop();
		MenuTracks[(int) _sfx].Play();
	}
	
	public static void PlayPMSoundEffect(PlayerMainSFX _sfx)
	{
		if (PlayerMainTracks == null)
			return;
		PlayerMainTracks[(int) _sfx].Stop();
		PlayerMainTracks[(int) _sfx].Play();
	}
	
	public static void PlayPMSoundEffectNoOverlap(PlayerMainSFX _sfx)
	{
		if (PlayerMainTracks == null)
			return;
		if(!PlayerMainTracks[(int) _sfx].isPlaying)
		{
			PlayerMainTracks[(int) _sfx].Stop();
			PlayerMainTracks[(int) _sfx].Play();
		}
	}
	
	public static void PlayEMSoundEffect(EnemyMainSFX _sfx)
	{
		if (EnemyMainTracks == null)
			return;
		EnemyMainTracks[(int) _sfx].Stop();
		EnemyMainTracks[(int) _sfx].Play();
	}
	
	public static void PlayECSoundEffect(EnemyChildSFX _sfx, AudioSource _Source)
	{
		if (EnemyChildTracks == null)
			return;
		_Source.Stop();
		_Source.clip = EnemyChildTracks[(int) _sfx];
		_Source.Play();	
	}
	
	public static void PlayEMSoundEffectNoOverlap(EnemyMainSFX _sfx)
	{
		if (EnemyMainTracks == null)
			return;
		if(!EnemyMainTracks[(int) _sfx].isPlaying)
		{
			EnemyMainTracks[(int) _sfx].Stop();
			EnemyMainTracks[(int) _sfx].Play();
		}
	}

	public static void PlaySquadSoundEffect(SquadSFX _sfx)
	{
		if (SquadChildTracks == null)
			return;

		SquadMainTracks[(int) _sfx].Stop();
		SquadMainTracks[(int) _sfx].Play();
	}
	
	private IEnumerator SceneTransition()
	{		
		SceneTransitionInProgress = true;
		//Debug.Log("start transition");
	
		while(BackgroundAudioSource.volume > 0.0f)
		{
			//Debug.Log(BackgroundAudioSource.volume);
			BackgroundAudioSource.volume -= 0.03f;
			yield return new WaitForSeconds(0.0025f);
		}
		
		//Debug.Log("fade out done");
		
		LoadRandomBackgroundTrack();
		
		CurrentSceneName = Application.loadedLevelName;
		
		//Debug.Log("track loaded");
		
		while(BackgroundAudioSource.volume < 1.0f)
		{
			BackgroundAudioSource.volume += 0.03f;
			yield return new WaitForSeconds(0.0025f);
		}
		
		SceneTransitionInProgress = false;
		//Debug.Log("fade in done");
	}
	
	public void PauseFadeOut()
	{
		while(BackgroundAudioSource.volume > 0.0f)
		{
			BackgroundAudioSource.volume -= 0.03f;
			StartCoroutine(WaitForRealSeconds(0.05f));
		}
		
		BackgroundAudioSource.Pause ();
	}
	
	public void PauseFadeIn()
	{
		BackgroundAudioSource.Play ();
		
		while(BackgroundAudioSource.volume < 1.0f)
		{
			BackgroundAudioSource.volume += 0.03f;
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
