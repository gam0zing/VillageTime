using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMCmpt : MonoBehaviour {

    private List<IState> _states = new();

    private void OnEnable() {
        this._states.Clear();
        Component[] cmpts = this.GetComponents<Component>();
        foreach (Component cmpt in cmpts) {
            if (typeof(IState).IsAssignableFrom(cmpt.GetType())) {
                IState state = (IState)cmpt;
                this._states.Add(state);
            }
        }
    }

    private void FixedUpdate() {
        foreach (var state in this._states) {
        }
    }
}
