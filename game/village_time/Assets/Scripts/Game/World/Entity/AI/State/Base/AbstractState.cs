using UnityEngine;

public abstract class AbstractState : Attribute, IState {
    public virtual void OnEnter() {
        Debug.Log(this.GetType().Name.Replace("Cmpt", " ") + "OnEnter");
    }

    public virtual void OnExit() {
        Debug.Log(this.GetType().Name.Replace("Cmpt", " ") + "OnExit");
    }

    public virtual void OnTick() {
        // Debug.Log(this.GetType().Name.Replace("Cmpt", " ") + "OnTick");
    }
}