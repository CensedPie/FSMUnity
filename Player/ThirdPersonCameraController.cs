using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    public float m_MouseSensitivity = 2f;
    public Vector2 m_ClampValues = new Vector2(-35, 60); // Arbirtrary clamp values in degrees.
    public Transform m_HeadTarget, m_Player;
    public float m_AimDistance = 5f, m_AimOffset = 1.2f, m_DistanceFromTarget = 10f; // These are arbitrary and need to be changed based on player model.
    public Vector3 m_PositionSmoothVelocity;
    public float m_PositionSmoothTime = 0.1f;
    public Texture m_CursorImg;

    private float m_MouseX, m_MouseY;

    // Start is called before the first frame update
    void Start()
    {
        // Lock the cursor to the center of the screen so you can't click off the game.
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnGUI()
    {
        // Draws a dot cursor on the middle of the screen.
        // 8 is the width of the cursor image aka how small the texture should be.
        GUI.DrawTexture(new Rect((Screen.width / 2) - (8 / 2), (Screen.height / 2) - (8 / 2), 8, 8), m_CursorImg);
    }

    void LateUpdate()
    {
        // Get X and Y mouse movements.
        m_MouseX += Input.GetAxis("Mouse X") * m_MouseSensitivity;
        m_MouseY -= Input.GetAxis("Mouse Y") * m_MouseSensitivity;
        // Clamp the Y axis movement so you can only look a certain distance up or down.
        m_MouseY = Mathf.Clamp(m_MouseY, m_ClampValues.x, m_ClampValues.y);

        // Check if RMB (Right Mouse Button) was pressed.
        if (Input.GetButton("Aim"))
        {
            // Smooth the movement of the camera and transition next to the player.
            // Aim down sights.
            transform.position = Vector3.SmoothDamp(transform.position, m_HeadTarget.position - transform.forward * m_AimDistance + transform.right * m_AimOffset, ref m_PositionSmoothVelocity, m_PositionSmoothTime);
            // We need to rotate the player along with the camera.
            m_Player.rotation = Quaternion.Euler(0f, m_MouseX, 0f);
        }
        else
        {
            // If we are not in Aiming we need to look at the head of the player with the camera so we can rotate around that target.
            m_HeadTarget.LookAt(m_HeadTarget);
            // Smooth the movement back to original camera position behind player.
            transform.position = Vector3.SmoothDamp(transform.position, m_HeadTarget.position - transform.forward * m_DistanceFromTarget + transform.right * 0.6f, ref m_PositionSmoothVelocity, m_PositionSmoothTime);
        }
        // Rotate the head target to rotate the camera.
        // If we rotate the camera we get shakey cam.
        m_HeadTarget.transform.rotation = Quaternion.Euler(m_MouseY, m_MouseX, 0f);
        // Follow the player position with the head target to move the camera after the player.
        // Add a new vector with 1.75 on Y to determine the height of the head target because it needs to be in the head not the base of the character.
        m_HeadTarget.transform.position = m_Player.position + new Vector3(0, 1.75f, 0);
    }

}
