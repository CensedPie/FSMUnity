using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class InteractComponent : MonoBehaviour
{
    public InteractTypes.InteractType m_Interaction = InteractTypes.InteractType.None;
    public GameObject m_ButtonPrompt;
    public GameObject m_TreeLogRef;
    public float m_Time = 0.0f;
    protected GameObject m_CharRef;
    private bool m_IsUsed;

    void Start()
    {
        Assert.IsNotNull(m_ButtonPrompt);
        if(m_Interaction == InteractTypes.InteractType.Cutting)
        {
            Assert.IsNotNull(m_TreeLogRef);
        }
        m_ButtonPrompt.GetComponent<SpriteRenderer>().enabled = false;
        m_IsUsed = false;
    }

    public InteractTypes.InteractType GetInteractionType()
    {
        return m_Interaction;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            m_ButtonPrompt.GetComponent<Billboarding>().SetDisplay(true);
            if(!m_IsUsed)
            {
                m_ButtonPrompt.GetComponent<SpriteRenderer>().enabled = true;
            }
            other.GetComponent<PlayerController>().SetInteractObj(this.gameObject);
            m_CharRef = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            m_ButtonPrompt.GetComponent<Billboarding>().SetDisplay(false);
            m_ButtonPrompt.GetComponent<SpriteRenderer>().enabled = false;
            other.GetComponent<PlayerController>().SetInteractObj(null);
            m_CharRef = null;
        }
    }

    public void SetButtonPrompt(bool enabled)
    {
        m_ButtonPrompt.GetComponent<SpriteRenderer>().enabled = enabled;
    }
    public bool GetUsed()
    {
        return m_IsUsed;
    }
    public void SetUsed(bool isUsed)
    {
        m_IsUsed = isUsed;
    }
    public float GetTime()
    {
        return m_Time;
    }
}
