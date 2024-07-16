using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackController : MonoBehaviour
{
    Animator _animator;
    CharacterController _controller;
    PlayerInput _input;
    PlayerInputManager _inputManager;
    PlayerWeaponController _playerWeaponController;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject bulletPos;

    [SerializeField] private AudioClip AttackAudioClip;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInput>();
        _inputManager = GetComponent<PlayerInputManager>();
        _playerWeaponController = GetComponent<PlayerWeaponController>();
    }

    public void OnAttack()
    {
        _animator.SetTrigger("Attack");
    }

    private void OnHit(AnimationEvent animationEvent)
    {
        if (AttackAudioClip != null)
        {
            SoundManager.Instance.SFXPlay("PlayerAttack", AttackAudioClip);
        }
    }

    private void OnAttackStart()
    {
        _input.enabled = false;
        _playerWeaponController.GetComponent<PlayerWeaponController>().SetWeaponCollider(true);
    }

    private void OnAttackEnd()
    {
        _input.enabled = true;
        _playerWeaponController.GetComponent<PlayerWeaponController>().SetWeaponCollider(false);

    }

    void OnShotAttack()
    {
        if (bulletPos == null || bulletPrefab == null) return;

        GameObject bullet = Instantiate(bulletPrefab, bulletPos.transform.position, bulletPos.transform.rotation);
        Rigidbody skillRigd = bullet.GetComponent<Rigidbody>();
        skillRigd.velocity = bulletPos.transform.forward * 10;
        if (AttackAudioClip != null) SoundManager.Instance.SFXPlay("PlayerAttack", AttackAudioClip);
    }
}
