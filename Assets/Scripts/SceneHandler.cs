using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void onSimulate()
    {
        SceneManager.LoadScene("simulation");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}