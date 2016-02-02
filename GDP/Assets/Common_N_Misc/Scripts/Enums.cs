using UnityEngine;
using System.Collections;

#region State Machines Enums
public enum PCState {Idle, Defend, ChargeMain, ChargeChild, Avoid, Dead};

public enum PSState {Idle, Attack, Defend, Produce, FindResource, Dead, Avoid };

public enum SCState {Idle, Attack, Defend, Produce, FindResource, Dead, Avoid };

public enum EMState {Production, Maintain, Defend, AggressiveAttack, CautiousAttack, Landmine, Stunned, Die, Win};

public enum ECState {Idle, Attack, ChargeMain, ChargeChild, TrickAttack, Defend, Avoid, Landmine, Dead};
#endregion


#region Enemy Child Cell
public enum MessageType {Empty, Idle, Attack, ChargeMain, ChargeChild, TrickAttack, Defend, Avoid, Landmine, Dead};

public enum PositionType {Empty, Aggressive, Neutral, Defensive};

public enum QueryType   {Exact, Around, Path};

public enum TargetType  {PlayerMain, PlayerChild, EnemyMain, EnemyChild, Others};

public enum RangeValue  {Min, Max, None};

public enum Directness  {Low, Mid, High};

public enum Formation {Empty, QuickCircle, Ladder, ReverseCircular, Turtle};
#endregion


#region Player
public enum PlayerAttackMode {BurstShot, SwarmTarget, ScatterShot};

public enum Node {LeftNode, RightNode, None};

public enum pcStatus {DeadState, InLeftNode, InRightNode, Attacking};
#endregion


#region Sound Effects
public enum MenuSFX {PressSelection, PressCancel};

public enum EnemyMainSFX {AbsorbNutrient,IdleContract,IdleExpand,Stunned,MainBeingHit,MainDeath,LandmineBeeping};

public enum EnemyChildSFX {CellChargeTowards,Defend,DeployLandmine,LandmineBeeping,LandmineExplode};

public enum PlayerMainSFX {AbsorbNutrient, ActionSelectAppear, ActionSelectDissapear, ActionSelecteSelected, TapNutrient, Win, Lose, MainBeingHit, MainDeath, SpawnCell};

public enum PlayerChildSFX {BurstShot, ScatterShot, Swarm};

public enum SquadSFX {SpawnCell, ChildAttack};
#endregion



#region Main Menu
public enum MainMenuPosition {Top, Center, Bottom};
#endregion



