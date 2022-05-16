using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    NavMeshAgent agent;
    
   
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //this should be updated when interfaces are added
        //we want to only allow movement clicks when we are not in an UI
        //eg, if (input.mouse(0) && inInterface == false)

        //also worth noting that when time comes to interact with an object in the world
        //we shouldn't necessarily be navigating into it (unless this produces the same effect
        //as runescape? then we keep it)

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //when clicked, cast a ray out from the screen
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                //set destination for the agent
                agent.SetDestination(hit.point);
            }
        }
    }
}