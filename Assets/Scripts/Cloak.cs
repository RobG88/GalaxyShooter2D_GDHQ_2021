using System.Collections;
using UnityEngine;

public class Cloak : MonoBehaviour
{
    Material material;

    [SerializeField] bool _isCloaking = false;
    [SerializeField] bool _deCloaking = false;
    [SerializeField] bool _cloaked = false;
    [SerializeField] float _fade = 1f;
    [SerializeField] float _decloakTimer;

    void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O) && !_cloaked)
        {
            //cloaked = !cloaked;
            _isCloaking = true;
        }
        else if (Input.GetKeyDown(KeyCode.O) && _cloaked)
        {
            _deCloaking = true;
        }

        if (_isCloaking)
        {
            //Debug.Log("Trying to cloak");
            _fade -= Time.deltaTime;
            this.gameObject.GetComponent<Collider2D>().enabled = false;
            if (_fade <= 0f)
            {
                /*
                _fade = 0f;
                _isCloaking = false;
                _cloaked = true;
                */
            }

            //material.SetFloat("_Fade", _fade);
            //material.SetFloat("_FadeAmount", _fade);
            StartCoroutine(FadeTo(1f));
            if (_cloaked)
            {
                // disable collider
                //this.gameObject.GetComponent<Collider2D>().enabled = false;
                //_decloakTimer = Random.Range(3f, 6f);
            }
        }



        if (_decloakTimer > 0f)
        {
            _decloakTimer -= Time.deltaTime;

            if (_decloakTimer <= 0)
                _deCloaking = true;
        }

        if (_deCloaking)
        {
            _fade -= Time.deltaTime;

            if (_fade <= 0)
            {
                _fade = 0f;
                _deCloaking = false;
                _cloaked = false;
            }

            //material.SetFloat("_Fade", _fade);
            material.SetFloat("_FadeAmount", _fade);


            if (_deCloaking)
            {
                this.gameObject.GetComponent<Collider2D>().enabled = true;
            }
        }
    }

    public void Cloaking()
    {
        if (!_deCloaking)
        {
            _isCloaking = true;
            _deCloaking = false;
        }
    }

    public void DeCloakin()
    {
        _isCloaking = false;
        _deCloaking = true;
    }

    IEnumerator EnableCloak() // WHY??? is Raycast being used?? Is it???
    {
        //lineRenderer.SetPosition(0, firePoint.position);

        //lineRenderer.SetPosition(1, _powerUpTarget.position);

        //lineRenderer.enabled = true;

        yield return new WaitForSeconds(0.2f);

        //lineRenderer.enabled = false;
    }


    IEnumerator FadeTo(float aTime)
    {

        //material.SetFloat("_FadeAmount", _fade);
        //float alpha = material.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            //Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
            material.SetFloat("_FadeAmount", t); ;
            yield return null;

        }
        _fade = 1f;
        _isCloaking = false;
        _cloaked = true;
        _decloakTimer = Random.Range(3f, 6f);
    }
}