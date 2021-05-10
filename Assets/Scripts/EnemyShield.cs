using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    /// 
    /// SHIELD VARIABLES
    /// Full Strength = 0000FF
    /// Mid Strength = 00FFFF
    /// Low Strength = 313131
    /// 
    [SerializeField] int _shieldPower;
    /// 
    /// SHIELD VARIABLES - END
    /// 
    float _lastDamageTime = -1;
    float _currenDamageTime = -1;

    [SerializeField] GameObject spriteGameObject;
    SpriteRenderer spriteRenderer;
    private void OnEnable()
    {
        //transform.localScale = new Vector3(1, 1, 1);
        spriteRenderer = spriteGameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = GetColorFromString("0000FF");
        _shieldPower = 2;
        _lastDamageTime = -1;
    }

    public void Damage()
    {
        if (_shieldPower > 0)
        {
            _shieldPower--;
            //if (_shieldPower == 2) spriteRenderer.color = GetColorFromString("00FFFF");
            if (_shieldPower == 1) spriteRenderer.color = GetColorFromString("313131");
            //transform.localScale -= new Vector3(0.20f, 0.20f, 0.20f);
        }
        if (_shieldPower == 0)
        {
            GameObject enemy = transform.parent.gameObject;
            enemy.GetComponent<Enemy>().ShieldsDestroyed();
        }
    }

    Color GetColorFromString(string hexString)
    {
        float red = HexToFloatNormalized(hexString.Substring(0, 2));
        float green = HexToFloatNormalized(hexString.Substring(2, 2));
        float blue = HexToFloatNormalized(hexString.Substring(4, 2));
        float alpha = 1f;
        if (hexString.Length >= 8)
        {
            alpha = HexToFloatNormalized(hexString.Substring(6, 2));
        }
        return new Color(red, green, blue, alpha);
    }

    int HexToDec(string hex) {
        int dec = System.Convert.ToInt32(hex, 16);
        return dec;
    }

    string DecToHex(int value)
    {
        return value.ToString("X2");
    }

    string FloatNormalizedToHex(float value) {
        return DecToHex(Mathf.RoundToInt(value * 255f));
    }

    float HexToFloatNormalized(string hex) {
        return HexToDec(hex) / 255f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        transform.GetComponent<CircleCollider2D>().enabled = false;
    }
}
