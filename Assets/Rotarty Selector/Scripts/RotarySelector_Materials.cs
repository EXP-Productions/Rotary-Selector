using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotarySelector_Materials : RotartySelector
{
    RotartySelector m_Selector;

    // Meshes to populate the selector with
    [SerializeField]
    Material[] m_Mats;

    protected override void Start()
    {
        m_AutoPopulate = false;
        base.Start();

        // Populate with the amount of materials in the array
        Populate(m_Mats.Length);
    }

    public override void Populate(int count)
    {
        base.Populate(count);

        // Set all mats
        for (int i = 0; i < m_SelectionObjects.Count; i++)        
            m_SelectionObjects[i].GetComponent<MeshRenderer>().material = m_Mats[i];        
    }
}
