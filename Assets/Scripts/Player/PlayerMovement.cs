using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("GameObject")]
    Animator _animator;
    Camera _camera;
    CharacterController _controller;
    PlayerInputManager _input;
    PlayerStateController _state;

    [SerializeField] private Transform _cameraFollowTarget;
    [SerializeField] private float cameraHeight;
    public float CameraHeight => cameraHeight;
    [SerializeField] private TrailRenderer[] trailEffects;

    float inputMagnitude;
    float xRotation;
    float yRotation;

    [Header("Mouse with Movement")]
    [SerializeField] private float smoothness = 10;
    [SerializeField] private float sensitivity = 5f;
    [SerializeField] private float limitMinX = -80;
    [SerializeField] private float limitMaxX = 80;

    [Header("Movement")]
    Vector3 inputDirection;
    float targetRotation;
    Quaternion moveRotation;
    Quaternion cameraRotaion;

    Vector3 targetDirection;
    Vector3 velocity;
    float verticalVelocity;

    [Header("Speed")]
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float speed = 4f;
    [SerializeField] private float moveSpeed;

    [Header("State")]
    private bool isJumping;
    private bool isMoving;
    private bool isFalling;
    private bool isGrounded;
    private bool isDash;
    Coroutine dashCoroutine;

    [Header("Jump")]
    // [SerializeField] private float gravity = Physics.gravity.y;
    [SerializeField] private float gravity = -15;
    [SerializeField] private float jumpSpeed = 1.2f;
    private float lastJumpTime = 0f;
    private float jumpDealyTime = 1f;
    private float fallTimeoutdelta;
    private float fallTimeout = 0.15f;
    private float velocityOfApplyGravity = 53f;

    [Header("Dash")]
    [SerializeField] private float dashTime = 0.25f;
    [SerializeField] private float dashSpeed = 10f;
    private float dashStartTime = 0;
    [SerializeField] private float dashCoolTime = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioClip LandingAudioClip;
    [SerializeField] private AudioClip FootstepAudioClip;
    [SerializeField] private AudioClip JumpAudioClip;
    [SerializeField] private AudioClip DashAudioClip;

    [Header("Ground Check")]
    Vector3 spherePosition;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundedOffset = -0.14f;
    [SerializeField] private float groundedRadius = 0.28f;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _camera = Camera.main; // Main Camera 태그를 찾는다.
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInputManager>();
        _state = GetComponent<PlayerStateController>();

        isMoving = false;
        _animator.SetBool("isMoving", false);

        fallTimeoutdelta = fallTimeout;
    }

    void Update()
    {
        JumpAndGravity();
        GroundCheckWithCheckSphere();
        Move();
    }

    void LateUpdate()
    {
        CameraRotation();
    }

    // void FixedUpdate() {
    //     _controller.Move(targetDirection.normalized * moveSpeed * Time.fixedDeltaTime + new Vector3(0f, verticalVelocity, 0f) * Time.fixedDeltaTime);    
    // }

    void CameraRotation()
    {
        if (PlayerManager.Instance.PlayerSwapController.IsSwap)
        {
            xRotation = PlayerManager.Instance.PlayerSwapController.TempCameraRoot.x;
            yRotation = PlayerManager.Instance.PlayerSwapController.TempCameraRoot.y;
            PlayerManager.Instance.PlayerSwapController.IsSwap = false;

            if (xRotation >= limitMaxX) xRotation -= 360;
            else if (xRotation <= -limitMinX) xRotation = 360;
            xRotation *= -1;
        }

        xRotation += _input.look.y * sensitivity * Time.deltaTime;
        yRotation += _input.look.x * sensitivity * Time.deltaTime;

        // 상하각도 제한
        xRotation = Mathf.Clamp(xRotation, -limitMinX+1, limitMaxX);
        cameraRotaion = Quaternion.Euler(-xRotation, yRotation, 0);

        _cameraFollowTarget.rotation = cameraRotaion;
    }

    void GroundCheckWithCheckSphere()
    {
        spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        isGrounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayer, QueryTriggerInteraction.Ignore);
        _animator.SetBool("isGrounded", isGrounded);
    }

    public void Move()
    {
        inputDirection = new Vector3(_input.move.x, 0, _input.move.y);

        if (_input.move != Vector2.zero)
        {
            targetRotation = Quaternion.LookRotation(inputDirection).eulerAngles.y + _camera.transform.rotation.eulerAngles.y;
            moveRotation = Quaternion.Euler(0, targetRotation, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, moveRotation, smoothness * Time.deltaTime);
            inputMagnitude = Mathf.Clamp(targetDirection.magnitude, 0, 0.5f);

            if (_input.dash)
            {
                inputMagnitude *= 2;
                if (Input.GetKeyUp(KeyCode.LeftShift))
                {
                    _input.dash = false;
                }

                if (!_state.HasStamina("sprint"))
                {
                    _input.dash = false;
                }
            }

            _animator.SetFloat("Input Magnitude", inputMagnitude, 0.05f, Time.deltaTime);
            moveSpeed = inputMagnitude == 1 ? maxSpeed : speed;

            _animator.SetBool("isMoving", true);
        }
        else
        {
            moveSpeed = 0;
            inputMagnitude = 0;
            _animator.SetBool("isMoving", false);
            _input.dash = false;
        }
        targetDirection = Quaternion.Euler(0, targetRotation, 0) * Vector3.forward;

        isMoving = true;
        _animator.SetBool("isMoving", true);
        _animator.SetFloat("Blend_Run", inputMagnitude, 0.05f, Time.deltaTime);

        _state.IsSprinting = moveSpeed == maxSpeed ? true : false;
        if (moveSpeed == maxSpeed)
        {
            _state.SprintingWithStamina();
        }

        _controller.Move(targetDirection.normalized * moveSpeed * Time.deltaTime + new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);
    }

    public void JumpAndGravity()
    {
        if (isGrounded)
        {
            fallTimeoutdelta = fallTimeout;

            isJumping = false;
            _animator.SetBool("isJumping", false);

            isFalling = false;
            _animator.SetBool("isFalling", false);

            if (verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            if (_input.jump)
            {
                if (!(lastJumpTime == 0 || Time.time - lastJumpTime > jumpDealyTime))
                {
                    return;
                }
                if (!_state.HasStamina("jump"))
                {
                    return;
                }
                verticalVelocity = Mathf.Sqrt(jumpSpeed * -gravity * 2f);
                lastJumpTime = Time.time;

                isJumping = true;
                _animator.SetBool("isJumping", true);
                _state.DecreaseStamina(PlayerStateController.StaminaState.Jump);
            }
        }
        else
        {
            _input.jump = false;

            if (fallTimeoutdelta >= 0f)
            {
                fallTimeoutdelta -= Time.deltaTime;
            }
            else
            {
                isFalling = true;
                _animator.SetBool("isFalling", true);
            }

        }

        if (verticalVelocity < velocityOfApplyGravity)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    public IEnumerator ToDash()
    {
        float startTime = Time.time;
        if ((dashStartTime == 0 || Time.time - dashStartTime > dashCoolTime) && isGrounded)
        {
            if (isJumping || isFalling)
            {
                yield break;
            }
            dashStartTime = Time.time;
            SoundManager.Instance.SFXPlay("PlayerDash", DashAudioClip);

            velocity = transform.TransformDirection(Vector3.forward);
            _state.DecreaseStamina(PlayerStateController.StaminaState.Dash);
            List<TrailRenderer> dashEffects = new List<TrailRenderer>();
            isDash = true;

            inputMagnitude = 1;
            while (Time.time < startTime + dashTime)
            {
                _controller.Move(velocity * dashSpeed * Time.deltaTime);
                _animator.SetFloat("Input Magnitude", 1, 0.05f, Time.deltaTime);
                _animator.SetFloat("Blend_Run", 1, 0.001f, Time.deltaTime);

                foreach (TrailRenderer dashEffect in trailEffects)
                {
                    if (dashEffect.tag == "Effect_Dash")
                    {
                        dashEffects.Add(dashEffect);
                    }
                }
                foreach (TrailRenderer dashEffect in dashEffects)
                {
                    dashEffect.enabled = true;
                }

                yield return null;
            }

            yield return new WaitForSeconds(1f);
            isDash = false;
            foreach (TrailRenderer dashEffect in dashEffects)
            {
                dashEffect.enabled = false;
            }
        }
    }

    private void OnApplicationFocus(bool focusStatus)
    {
        if (focusStatus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void OnJumpSound(AnimationEvent animationEvent)
    {
        SoundManager.Instance.SFXPlay("Jump", JumpAudioClip);
    }

    private void OnLandSound(AnimationEvent animationEvent)
    {
        SoundManager.Instance.SFXPlay("Land", LandingAudioClip);
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f) // BlendTree
        {
            if (FootstepAudioClip != null)
            {
                SoundManager.Instance.SFXPlay("Footstep", FootstepAudioClip);
            }
        }
    }

    public bool GetIsDash()
    {
        return isDash;
    }

    public void StartDash()
    {
        dashCoroutine = StartCoroutine(ToDash());
    }

    public void PlayerSwapSetAction()
    {
        isDash = false;

        List<TrailRenderer> dashEffects = new List<TrailRenderer>();
        foreach (TrailRenderer dashEffect in trailEffects)
        {
            if (dashEffect.tag == "Effect_Dash")
            {
                dashEffects.Add(dashEffect);
            }
        }
        foreach (TrailRenderer dashEffect in dashEffects)
        {
            dashEffect.enabled = false;
        }
        if (dashCoroutine != null) StopCoroutine(dashCoroutine);
    }

    public void SetRunSpeed(float speed)
    {
        moveSpeed = speed;
    }
}
