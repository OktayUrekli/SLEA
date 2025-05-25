using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonLeadAfterDead : MonoBehaviour
{
    [Header("Other Demons Creating")]
    [SerializeField] GameObject demon;
    [SerializeField] Transform[] createDemonPos;
    [SerializeField] Transform demonParentTransform;// buraya npc nin bulunduu stage konulur

    public void CreateDemon()
    {
        for (int i = 0; i < 2; i++)
        {
            Instantiate(demon, createDemonPos[i].position, Quaternion.identity, demonParentTransform);
        }
    }
}
