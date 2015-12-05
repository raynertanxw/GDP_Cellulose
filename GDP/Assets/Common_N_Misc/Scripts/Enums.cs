using UnityEngine;
using System.Collections;

public enum PCState {Idle, Defend, ChargeMain, ChargeChild, Avoid, Dead};

public enum SCState {Idle, Attack, Defend, Produce, FindResource, Dead };

public enum EMState {Production, Maintain, Defend, AggressiveAttack, CautiousAttack, Landmine, Stunned, Die};

public enum ECState {Idle, Attack, ChargeMain, ChargeChild, TrickAttack, Defend, Avoid, Landmine, Dead};

public enum MessageType {Empty, Idle, Attack, ChargeMain, ChargeChild, TrickAttack, Defend, Avoid, Landmine, Dead};

public enum PositionType {Aggressive, Neutral, Defensive};

