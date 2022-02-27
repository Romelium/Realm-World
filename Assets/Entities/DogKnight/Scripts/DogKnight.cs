using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DogKnight : MonoBehaviour, IDamageable, IDizzyable
{
    public bool Dizzy { get => dizzy; set => SetDizzy(value); }
    public bool Die { get => die; set => SetDie(value); }
    bool dizzy = false;
    bool die = false;

    private Animator _animator;
    private int _animIDAttack01;
    private int _animIDAttack02;
    private int _animIDGetHit;
    private int _animIDDizzy;
    private int _animIDDie;

    // Start is called before the first frame update
    void Awake()
    {
        InitAnimation();
    }
    void InitAnimation()
    {
        _animator = GetComponent<Animator>();
        _animIDAttack01 = Animator.StringToHash("Attack01");
        _animIDAttack02 = Animator.StringToHash("Attack02");
        _animIDGetHit = Animator.StringToHash("GetHit");
        _animIDDizzy = Animator.StringToHash("Dizzy");
        _animIDDie = Animator.StringToHash("Die");
    }
    public void Attack01()
    {
        _animator.SetTrigger(_animIDAttack01);
    }
    public void Attack02()
    {
        _animator.SetTrigger(_animIDAttack02);
    }
    public void Hit(float damage)
    {
        _animator.SetTrigger(_animIDGetHit);
    }
    private void SetDizzy(bool value)
    {
        _animator.SetBool(_animIDDizzy, value);
    }
    private void SetDie(bool value)
    {
        _animator.SetBool(_animIDDie, value);
    }
}
