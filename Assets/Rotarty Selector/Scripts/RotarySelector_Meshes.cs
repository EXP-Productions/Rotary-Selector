using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotarySelector_Meshes : RotartySelector
{
    RotartySelector m_Selector;

    // Meshes to populate the selector with
    [SerializeField]
    Mesh[] m_Meshes;

    protected override void Start()
    {
        m_AutoPopulate = false;
        base.Start();

        // Populate with the amount of meshes in the array
        Populate(m_Meshes.Length);
    }

    public override void Populate(int count)
    {
        base.Populate(count);

        // Set all 
        for (int i = 0; i < m_SelectionObjects.Count; i++)        
            m_SelectionObjects[i].GetComponent<MeshFilter>().mesh = m_Meshes[i];        
    }
}
