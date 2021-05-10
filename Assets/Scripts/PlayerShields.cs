using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShields : MonoBehaviour
{
    /// 
    /// SHIELD VARIABLES
    /// Inner Hex Color = 6200FF
    /// Outer Glow Color = 3B00FF
    /// 
    [SerializeField] int _shieldPower;
    [SerializeField] int _shieldBonus;
    /// 
    /// SHIELD VARIABLES - END
    /// 

    private void OnEnable()
    {
        transform.localScale = new Vector3(1, 1, 1);
        _shieldPower = 3;
    }

    public void Damage()
    {
        if (_shieldPower > 0)
        {
            _shieldPower--;
            transform.localScale -= new Vector3(0.20f, 0.20f, 0.20f);
        }
        if (_shieldPower == 0)
        {
            Player.instance.ShieldsDestroyed();
        }
    }
}
