using UnityEngine;
using System.Collections;

public enum PCState {Idle, Defend, ChargeMain, ChargeChild, Avoid, Dead};

public enum PSState {Idle, Attack, Defend, Produce, FindResource, Dead, Avoid };

public enum SCState {Idle, Attack, Defend, Produce, FindResource, Dead, Avoid };

public enum EMState {Production, Maintain, Defend, AggressiveAttack, CautiousAttack, Landmine, Stunned, Die, Win};

public enum ECState {Idle, Attack, ChargeMain, ChargeChild, TrickAttack, Defend, Avoid, Landmine, Dead};




public enum MessageType {Empty, Idle, Attack, ChargeMain, ChargeChild, TrickAttack, Defend, Avoid, Landmine, Dead};

public enum PositionType {Empty, Aggressive, Neutral, Defensive};

public enum QueryType   {Exact, Around, Path};

public enum TargetType  {PlayerMain, PlayerChild, EnemyMain, EnemyChild, Others};

public enum RangeValue  {Min, Max, None};

public enum Directness  {Low, Mid, High};

public enum Formation {Empty, QuickCircle, Ladder, ReverseCircular, Turtle};




public enum PlayerAttackMode {BurstShot, SwarmTarget, ScatterShot};

public enum Node {LeftNode, RightNode, None};

public enum pcStatus {DeadState, InLeftNode, InRightNode, Attacking};

public enum MenuSFX {PressSelection, PressCancel};

public enum EnemySFX {AbsorbNutrient,CellChargeTowards,Defend,DeployLandmine,IdleContract,IdleExpand,LandmineBeeping,LandmineExplode,Stunned,MainBeingHit,MainDeath};

public enum PlayerSFX {AbsorbNutrient, ActionSelectAppear, ActionSelectDissapear, ActionSelecteSelected, BurstShot, ScatterShot, Swarm, SpawnCell, TapNutrient, Win, Lose, MainBeingHit, MainDeath};

public enum SquadSFX {SpawnCell, ChildAttack};