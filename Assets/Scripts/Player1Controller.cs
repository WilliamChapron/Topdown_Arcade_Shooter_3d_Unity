using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Controller : PlayerController
{
    int isRunningHash;
    PlayerControl input;
    Vector2 currentMovement;
    Vector2 currentAim;

     
    private void Shield()
    {
        shieldPower();
    }

    private void Dash()
    {
        dashPower();
    }

    private void shoot()
    {
        shootingAction();
    }

    void Awake()
    {

        input = new PlayerControl();

        input.Player1Controls.Movement.performed += ctx =>
        {
            if (_currentState != PlayerState.Shield && _currentState != PlayerState.Dead) 
            {
                currentMovement = ctx.ReadValue<Vector2>();
                SetPlayerState(PlayerState.Running);
            }
        };

        input.Player1Controls.Aim.performed += ctx =>
        {
            if (_currentState != PlayerState.Dead)
            {
                currentAim = ctx.ReadValue<Vector2>();
            }
        };

        input.Player1Controls.Shield.performed += ctx =>
        {
            if (_currentState != PlayerState.Dead)
            {
                Shield();
                SetPlayerState(PlayerState.Shield);
            }
        };

        input.Player1Controls.Dash.performed += ctx =>
        {
            if (_currentState != PlayerState.Dead)
            {
                Dash();
                SetPlayerState(PlayerState.Dash);
            }
        };

        input.Player1Controls.Shoot.performed += ctx =>
        {
            if (_currentState != PlayerState.Dead)
            {
                shoot();
            }
        };
    }

    void RotateControl()
    {
        if (currentAim != Vector2.zero)
        {
            float angle = Mathf.Atan2(currentAim.x, currentAim.y) * Mathf.Rad2Deg;
            angle += 90f;
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }
    }

    void MovementControl()
    {
        if (_currentState != PlayerState.Shield && _currentState != PlayerState.Dead) 
        {
            Vector3 movementDirection = new Vector3(currentMovement.x, 0, currentMovement.y);
            movementDirection = Quaternion.Euler(0, 90, 0) * movementDirection;
            transform.Translate(movementDirection.normalized * Time.deltaTime * _moveSpeed, Space.World);

            if (currentMovement.magnitude < 0.1f)
            {
                SetPlayerState(PlayerState.Idle);
            }
        }
        
    }

    void Update()
    {
        playerState();
        MovementControl();
        RotateControl();
        UpdatePowerState();
        
    }

    void OnEnable()
    {
        input.Player1Controls.Enable();
    }

    void OnDisable()
    {
        input.Player1Controls.Disable();
    }

    public void SetInputVector(Vector2 input)
    {
        currentMovement = input;
    }

}
