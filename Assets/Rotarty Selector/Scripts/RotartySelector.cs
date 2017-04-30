using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rotary seelction tool
/// - Populate a circle of gameobjects and select by rotating through the menu
/// </summary>
public class RotartySelector : MonoBehaviour
{
    // Event for selecting an index
    public delegate void SelectedIndexEvent(int i);
    public event SelectedIndexEvent OnSelectedIndexEvent;

    // The type of mesh primative to use
    [SerializeField]
    PrimitiveType m_PrimativeTpe = PrimitiveType.Sphere;

    // The selected object index 
    [SerializeField]
    int m_SelectedIndex = 0;
    public int SelectedIndex
    {
        set
        {
            m_SelectedIndex = value;
            if (OnSelectedIndexEvent != null) OnSelectedIndexEvent(m_SelectedIndex);
        }
    }

    // List of all selectable objects
    protected List<GameObject> m_SelectionObjects = new List<GameObject>();
    public List<GameObject> SelectionObjects { get { return m_SelectionObjects; } }

    // Number of items in the list
    int m_NumItems = 6;
    int NumItems { get { return m_NumItems; } }

    // Scales for unselected and selected
    public float m_UnselectedScale = .02f;
    public float m_SelectedScale = .05f;

    // The curve that defines the scale of the objects
    public AnimationCurve m_ScaleCurve;

    // The radius of the circle
    public float m_Radius = .1f;
   
    Transform m_Rotator;

    float IncrementAngle { get { return 360f/ (float)m_NumItems; } }
    float TargetAngle { get { return (float)m_SelectedIndex * IncrementAngle; } }

    float m_RotationSpeed = 16;

    // Is the selector rotating between objects
    bool m_Rotating = true;

    [SerializeField]
    protected bool m_AutoPopulate = false;

    [SerializeField]
    bool m_DebugInput = false;

    protected virtual void Start ()
    {
        // Create rotator and parent
        m_Rotator = new GameObject("_Rotator").transform;
        m_Rotator.SetParent(transform);
        m_Rotator.localPosition = Vector3.zero;
        m_Rotator.localRotation = Quaternion.identity;

        // Create curve
        //m_ScaleCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(.3f, 0), new Keyframe(1, 0) }); 
    }

    public virtual void Populate(int count)
    {
        m_NumItems = count;
        for (int i = 0; i < count; i++)
        {
            // create objects and set transform and scale
            GameObject newT = GameObject.CreatePrimitive(m_PrimativeTpe);
            newT.name = "Selection Object " + i;
            newT.transform.SetParent(m_Rotator);
            newT.transform.localScale = Vector3.one * m_UnselectedScale;           
            m_SelectionObjects.Add(newT);

            float norm = (float)i / (float)m_NumItems;
            norm += .5f;
            Vector3 pos = Vector3.zero;
            pos.x = Mathf.Sin(norm * Mathf.PI * 2) * m_Radius;
            pos.z = Mathf.Cos(norm * Mathf.PI * 2) * m_Radius;
            newT.transform.localPosition = pos;

            // Set angle
            newT.transform.LookAt(transform.position);
        }

        SetScales();
    }

    protected void SetScales()
    {
        for (int i = 0; i < m_SelectionObjects.Count; i++)
        {
            float normDot = Vector3.Dot( transform.TransformDirection(-Vector3.forward), m_Rotator.rotation * m_SelectionObjects[i].transform.localPosition.normalized);      
            m_SelectionObjects[i].transform.localScale = Vector3.one * (m_ScaleCurve.Evaluate(normDot).ScaleFrom01(m_UnselectedScale, m_SelectedScale));
        }

        Debug.DrawLine(transform.position, transform.position + transform.TransformDirection(Vector3.forward));
    }
	
	void Update ()
    {
        if (m_DebugInput)
        {
            // Debug input for next and prev
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                Prev();

            if (Input.GetKeyDown(KeyCode.RightArrow))
                Next();
        }
        
        if (m_Rotating)
        {
            // Update rotation
            m_Rotator.rotation = Quaternion.Slerp(m_Rotator.rotation, Quaternion.Euler(0, TargetAngle, 0), Time.deltaTime * m_RotationSpeed);

            // Update scales of objects
            SetScales();

            // If less than a degree then stop moving
            if (Mathf.Abs(m_Rotator.rotation.eulerAngles.y - TargetAngle) < 1)                          
                m_Rotating = false;            
        }
    }

    void Next()
    {
        int newIndex = m_SelectedIndex + 1;
        SelectedIndex = newIndex.WrapIntToRange(0, m_NumItems - 1);
        m_Rotating = true;
    }

    void Prev()
    {
        int newIndex = m_SelectedIndex - 1;
        SelectedIndex = newIndex.WrapIntToRange(0, m_NumItems - 1);
        m_Rotating = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, m_Radius);
        Gizmos.DrawSphere(transform.TransformPoint(-Vector3.forward * m_Radius), m_SelectedScale);
        Gizmos.DrawSphere(transform.TransformPoint(Vector3.forward * m_Radius), m_UnselectedScale);
    }
}
