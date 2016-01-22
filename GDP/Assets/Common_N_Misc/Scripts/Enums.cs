using UnityEngine;
using System.Collections;

public enum PCState {Idle, Defend, ChargeMain, ChargeChild, Avoid, Dead};

public enum PSState {Idle, Attack, Defend, Produce, FindResource, Dead };

public enum SCState {Idle, Attack, Defend, Produce, FindResource, Dead };

public enum EMState {Production, Maintain, Defend, AggressiveAttack, CautiousAttack, Landmine, Stunned, Die};

public enum ECState {Idle, Attack, ChargeMain, ChargeChild, TrickAttack, Defend, Avoid, Landmine, Dead};




public enum MessageType {Empty, Idle, Attack, ChargeMain, ChargeChild, TrickAttack, Defend, Avoid, Landmine, Dead};

public enum PositionType {Empty, Aggressive, Neutral, Defensive};

public enum QueryType   {Exact, Around, Path};

public enum TargetType  {PlayerMain, PlayerChild, EnemyMain, EnemyChild, Others};

public enum RangeValue  {Min, Max, None};

public enum Directness  {Low, Mid, High};

public enum Formation {Empty, QuickCircle, Ladder, ReverseCircular, Turtle};

public enum PlayerAttackMode {BurstShot, SwarmTarget, ScatterShot};

public enum Node {LeftNode, RightNode};

public enum pcStatus {DeadState, InLeftNode, InRightNode, Attacking};