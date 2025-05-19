using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeGateBehaviour : MonoBehaviour
{
    [SerializeField] GameObject gateEnter;
    [SerializeField] GameObject gateExit;

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CloseGates(); 
        }    
    }

    public void OpenGates()
    {
        gateEnter.GetComponent<Animator>().SetTrigger("Open");
        gateExit.GetComponent<Animator>().SetTrigger("Open");
    }

    void CloseGates()
    {
        gateEnter.GetComponent<Animator>().SetTrigger("Close");
        gateExit.GetComponent<Animator>().SetTrigger("Close");
    }
}
