using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodScript : MonoBehaviour
{
    public static BloodScript Instance;
    public GameObject[] BloodParticles;
  

    void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InvokeBlood(Transform pos)
    {
        int rnd = Random.Range(0,BloodParticles.Length-1);
        Instantiate(BloodParticles[rnd], pos.position, Quaternion.identity);
    }
}
