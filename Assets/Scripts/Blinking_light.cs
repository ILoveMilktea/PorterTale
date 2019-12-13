using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinking_light : MonoBehaviour
{

    public GameObject[] Emissions;
    public Light[] lights;

    private int ran;
    private float counter;

 

    // Use this for initialization
    void Start()
    {

      

  


    }

    // Update is called once per frame
    void Update()
    {



        counter += Time.deltaTime;

        if (counter > 0.5f)
        {
            ran = Random.Range(0, 2);

            //Debug.Log(ran);
            counter = 0;

        }

        for (int i = 0; i < Emissions.Length; i++)
        {
            Renderer renderer = Emissions[i].GetComponentInChildren<Renderer>();
            Material mat = renderer.material;

            if (ran == 0)
            {
              
                mat.SetColor("_EmissionColor", Color.white);
            }
            else
            {
               
                mat.SetColor("_EmissionColor", Color.black);
            }
        }

        for (int i = 0; i < lights.Length; i++)
        {

            lights[i] = GetComponentInChildren<Light>();

            if (ran == 0)
            {

                lights[i].intensity = 1F;

            }
            else
            {

                lights[i].intensity = 0F;

            }

        }

    }
}
