using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Interact : State
{
    protected Vector3 m_TargetVec;
    protected Quaternion m_NewAngle;
    protected float m_Timer;
    protected float m_MaxTime;

    public Interact(PlayerController playerController) : base(playerController)
    {

    }

    public override void Enter()
    {
        Assert.IsNotNull(m_PlayerController.m_InteractObj);
        if(m_PlayerController.m_InteractObj.GetComponent<InteractComponent>().GetUsed())
        {
            m_EventTransition.Invoke(new Idle(m_PlayerController), false);
        }
        base.Enter();
        m_InteractType = m_PlayerController.m_InteractObj.GetComponent<InteractComponent>().GetInteractionType();
        m_TargetVec = m_PlayerController.m_InteractObj.transform.position - m_CharController.transform.position;
        m_TargetVec = Vector3.Normalize(m_TargetVec);
        m_NewAngle = Quaternion.LookRotation(m_TargetVec, Vector3.up);
        m_Timer = m_PlayerController.m_InteractObj.GetComponent<InteractComponent>().GetTime();
        m_MaxTime = m_Timer;
        m_PlayerController.m_Canvas.transform.GetChild(0).GetComponent<ProgBarLoad>().ResetProgBar();
        m_PlayerController.m_Canvas.transform.GetChild(0).GetComponent<ProgBarLoad>().SetVisible(true);
        //m_AnimComponent.SetBool("Interact", true);
    }


    public override void HandleInput()
    {
        base.HandleInput();
        if (!Input.GetButton("Interact"))
        {
            m_EventTransition.Invoke(new Idle(m_PlayerController), false);
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        m_CharController.transform.rotation =  Quaternion.Lerp(m_CharController.transform.rotation, m_NewAngle, 0.1f);
        m_Timer -= Time.deltaTime;
        m_PlayerController.m_Canvas.transform.GetChild(0).GetComponent<ProgBarLoad>().UpdateProgBar(1.0f - m_Timer / m_MaxTime);
        if(m_Timer <= 0)
        {
            Debug.Log("Exiting Interact");
            m_PlayerController.m_InteractObj.GetComponent<InteractComponent>().SetUsed(true);
            m_PlayerController.m_InteractObj.GetComponent<InteractComponent>().SetButtonPrompt(false);
            if(m_InteractType == InteractTypes.InteractType.Cutting)
            {
                Vector3 logSpawnPos = m_PlayerController.m_InteractObj.transform.position + m_PlayerController.m_InteractObj.transform.GetChild(0).gameObject.transform.localPosition;
                Quaternion logSpawnRot = m_PlayerController.m_InteractObj.transform.rotation;
                Object.Destroy(m_PlayerController.m_InteractObj.transform.GetChild(0).gameObject);
                Object.Destroy(m_PlayerController.m_InteractObj.transform.GetChild(1).gameObject);
                GameObject logSpawnRef = Object.Instantiate(m_PlayerController.m_InteractObj.GetComponent<InteractComponent>().m_TreeLogRef, logSpawnPos, logSpawnRot);
                logSpawnRef.GetComponent<Rigidbody>().AddForceAtPosition(new Vector3(50,0,0),logSpawnRef.transform.position + new Vector3(0,5,0));
            }
            m_EventTransition.Invoke(new Idle(m_PlayerController), false);
        }
    }

    public override void Exit()
    {
        base.Exit();
        m_InteractType = InteractTypes.InteractType.None;
        m_PlayerController.m_Canvas.transform.GetChild(0).GetComponent<ProgBarLoad>().SetVisible(false);
        //m_AnimComponent.SetBool("Interact", false);
    }
}
