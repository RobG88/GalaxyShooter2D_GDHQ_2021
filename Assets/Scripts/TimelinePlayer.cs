using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
public class TimelinePlayer : MonoBehaviour
{
    [SerializeField] GameObject _controlPanel;
    [SerializeField] PlayableDirector _director;

    void Awake()
    {
        //_director = GetComponent<PlayableDirector>();
        _director.played += Director_Player;
        _director.stopped += Director_Stopped;
    }

    public void StartTimeline()
    {
        //_director.playOnAwake = true;
        _director.Play();
    }
    void Director_Player(PlayableDirector obj)
    {
        _controlPanel.SetActive(false);
    }

    void Director_Stopped(PlayableDirector obj)
    {
        _controlPanel.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _director.Play();
        }
    }
}