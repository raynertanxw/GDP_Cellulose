using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

	private AudioSource BackgroundAudioSource;
	private List<AudioSource> SFXAudioSources;
	public AudioClip[] BackgroundTracks;
	
	private float BackgroundClipLength;
	private string CurrentSceneName;
	
	private static List<AudioSource> MenuTracks;
	private static List<AudioSource> PlayerMainTracks;
	private static List<AudioSource> EnemyMainTracks;
	
	private static List<AudioClip> PlayerChildTracks;
	private static List<AudioClip> EnemyChildTracks;
	private static List<AudioClip> SquadChildTracks;
	
	private SceneType CurrentSceneType;
	private enum SceneType{Menu,Gameplay,Null};

	// Use this for initialization
	void Start () 
	{
		DontDestroyOnLoad(gameObject);
	
		BackgroundAudioSource = GetComponent<AudioSource>();
		SFXAudioSources = new List<AudioSource>();
		
		MenuTracks = new List<AudioSource>();
		PlayerMainTracks = new List<AudioSource>();
		EnemyMainTracks = new List<AudioSource>();
		
		PlayerChildTracks = new List<AudioClip>();
		EnemyChildTracks = new List<AudioClip>();
		SquadChildTracks = new List<AudioClip>();
		
		CurrentSceneName = Application.loadedLevelName;
		
		LoadTracksToLists();
		LoadRandomBackgroundTrack();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//if the scene had been change, reload a random background track
		if(CurrentSceneName != Application.loadedLevelName)
		{
			LoadRandomBackgroundTrack();
			CurrentSceneName = Application.loadedLevelName;
			BackgroundAudioSource.volume = 0f;
			StartCoroutine(FadeInBackgroundMusic());
		}
	
		//Fading in/Fading out of BGM
		if(BackgroundAudioSource.time <= 3.5f)
		{
			StartCoroutine(FadeInBackgroundMusic());
		}
		if(BackgroundAudioSource.time >= BackgroundClipLength - 3.5f)
		{
			StartCoroutine(FadeOutBackgroundMusic());
		}
	}
	
	private void LoadTracksToLists()
	{
		for(int i = 0; i < transform.childCount; i++)
		{
			GameObject Child = transform.GetChild(i).gameObject;
			
			if(Child.name.Contains("Menu"))
			{
				MenuTracks.Add(Child.GetComponent<AudioSource>());
			}
			else if(Child.name.Contains("Player"))
			{
				PlayerMainTracks.Add(Child.GetComponent<AudioSource>());
			}
			else if(Child.name.Contains("Enemy"))
			{
				EnemyMainTracks.Add(Child.GetComponent<AudioSource>());
			}
		}
		
		PlayerChildTracks.Add(Resources.Load("Audio/Sound Effects/Player_BurstShotv2") as AudioClip);
		PlayerChildTracks.Add(Resources.Load("Audio/Sound Effects/Player_Scattershotv2") as AudioClip);
		PlayerChildTracks.Add(Resources.Load("Audio/Sound Effects/Player_Swarm") as AudioClip);
		
		EnemyChildTracks.Add(Resources.Load("Audio/Sound Effects/Enemy_CellChargeTowards") as AudioClip);
		EnemyChildTracks.Add(Resources.Load("Audio/Sound Effects/Enemy_Defend") as AudioClip);
		EnemyChildTracks.Add(Resources.Load("Audio/Sound Effects/Enemy_DeployLandmine") as AudioClip);
		EnemyChildTracks.Add(Resources.Load("Audio/Sound Effects/Enemy_LandmineBeeping") as AudioClip);
		EnemyChildTracks.Add(Resources.Load("Audio/Sound Effects/Enemy_LandmineExplode") as AudioClip);
		
		SquadChildTracks.Add(Resources.Load("Audio/Sound Effects/Squad_SpawnCell") as AudioClip);
		SquadChildTracks.Add(Resources.Load("Audio/Sound Effects/Squad_ChildAttack") as AudioClip);
		
		Debug.Log("PlayerChildTrack count: " + PlayerChildTracks.Count);
		Debug.Log("EnemyChildTracks count: " + EnemyChildTracks.Count);
		Debug.Log("SquadChildTracks count: " + SquadChildTracks.Count);
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
			BackgroundAudioSource.volume += 0.0075f;
			yield return new WaitForSeconds(0.75f);
		}
	}
	
	private IEnumerator FadeOutBackgroundMusic()
	{
		while(BackgroundAudioSource.volume > 0.0f)
		{
			BackgroundAudioSource.volume -= 0.0075f;
			yield return new WaitForSeconds(0.75f);
		}
		LoadRandomBackgroundTrack();
	}
	
	public static void PlayMenuSoundEffect(MenuSFX _sfx)
	{
		if (MenuTracks == null)
			return;
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
	
	public static void PlayEMSoundEffect(EnemyMainSFX _sfx)
	{
		if (EnemyMainTracks == null)
			return;
		EnemyMainTracks[(int) _sfx].Stop();
		EnemyMainTracks[(int) _sfx].Play();
	}
	
	public static void PlayPCSoundEffect(PlayerChildSFX _sfx, AudioSource _Source)
	{
		if (PlayerChildTracks == null)
			return;

		_Source.Stop();
		_Source.clip = PlayerChildTracks[(int) _sfx];
		_Source.Play();
	}
	
	public static void PlayECSoundEffect(EnemyChildSFX _sfx, AudioSource _Source)
	{
		if (EnemyChildTracks == null)
			return;

		_Source.Stop();
		_Source.clip = EnemyChildTracks[(int) _sfx];
		Debug.Log((int) _sfx);
		_Source.Play();
	}
	
	public static void PlaySquadSoundEffect(SquadSFX _sfx, AudioSource _Source)
	{
		if (SquadChildTracks == null)
			return;

		_Source.Stop();
		_Source.clip = SquadChildTracks[(int) _sfx];
		_Source.Play();
	}
}
