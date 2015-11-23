using UnityEngine;
using System.Collections;

public enum PCState {Idle, Defend, ChargeMain, ChargeChild, Avoid, Dead};

public enum SCState {Idle, Attack, Defend, Produce, FindResource, Dead };

public enum MessageType {Empty, Idle, Attack, Defend, Avoid, Landmine, Dead};
