using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMenuStateListener
{

    public void OnMenuStateEnter();

    public void OnMenuStateExit();
}
