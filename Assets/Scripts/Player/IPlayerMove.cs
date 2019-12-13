using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerMove
{
    void Move(Vector3 dir, float amount);
    void Stop();
}
