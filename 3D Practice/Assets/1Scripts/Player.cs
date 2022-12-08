using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class Player : MonoBehaviour
{   
    private Rigidbody CRigidbody;
    private Animator CAnimator;
    private GameObject PrefabGameObject;
    [SerializeField] private GameObject[] Weapons; // 무기 아이템 관리
    private Weapon EquipWeapon; // 현재 장착한 아이템

    private Vector3 PlayerMoving;

    private int PlayerHealth; 
    
    private float HorizontalMove;
    private float VerticalMove;
    private float MovingSpeed; // 걷기 속도
    private float RunningSpeed; // 달리기 속도
    private float JumpPower;

    [SerializeField] private bool[] hasWeapon;
    private bool isShift; // 달리기
    private bool isSpace; // 점프
    private bool isF; // 회피
    private bool isE; // 아이템 줍기
    private bool isJumping;
    private bool isDodging;
    private bool isWeaponSwapHammer;
    private bool isWeaponSwapHandGun;
    private bool isWeaponSwapMachineGun;
    private bool isSwapWeapon;
    private bool isLeftCtrl; // 공격

    private void Awake()
    {
        CRigidbody = GetComponent<Rigidbody>();
        CAnimator = GetComponentInChildren<Animator>();
        isShift = false;
        isSpace = false;
        isJumping = false;
        isDodging = false;
        isF = false;
        isE = false;
        isWeaponSwapHammer = false;
        isWeaponSwapHandGun = false;
        isWeaponSwapMachineGun = false;
        isSwapWeapon = false;
        isLeftCtrl = false;
    }
    
    private void Update()
    {
        InputKey();
        LookAtFace();
        PlayerMovingControl();
        PlayerJumpControl();
        PlayerDodgeControl();
        PlayerWeaponSwapControl();
        PlayerPickUpItemControl();                      
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bottom"))
        {
            CAnimator.SetBool("isJump", false);
            isJumping = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Item CItem = other.GetComponent<Item>();

        if (other.CompareTag("Item"))
        {            
            switch (CItem.ItemType)
            {
                case Item.ItemEnum.Coin:
                    break;
                case Item.ItemEnum.Heart:
                    break;
                default:
                    break;
            }

            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Weapon"))
            PrefabGameObject = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Weapon"))
            PrefabGameObject = null;
    }

    private void InputKey()
    {
        HorizontalMove = Input.GetAxisRaw("Horizontal");
        VerticalMove = Input.GetAxisRaw("Vertical");
        isShift = Input.GetButton("Run");
        isSpace = Input.GetButtonDown("Jump");
        isF = Input.GetButtonDown("Dodge");
        isE = Input.GetButtonDown("PickUp");
        isWeaponSwapHammer = Input.GetButtonDown("WeaponSwapHammer");
        isWeaponSwapHandGun = Input.GetButtonDown("WeaponSwapHandGun");
        isWeaponSwapMachineGun = Input.GetButtonDown("WeaponSwapMachineGun");
        isLeftCtrl = Input.GetButtonDown("Fire1");
    }

    private void PlayerMovingControl()
    {       
        MovingSpeed = 8.0f;
        RunningSpeed = 2.0f;

        PlayerMoving = new Vector3(HorizontalMove, 0, VerticalMove).normalized;

        if (isShift)
            this.transform.position += MovingSpeed * RunningSpeed * Time.deltaTime * PlayerMoving;
        else
            this.transform.position += MovingSpeed * Time.deltaTime * PlayerMoving;

        CAnimator.SetBool("isWalk", PlayerMoving != Vector3.zero);
        CAnimator.SetBool("isRun", isShift);        
    }

    private void LookAtFace()
    {
        transform.LookAt(this.transform.position + PlayerMoving);
    }   

    private void PlayerJumpControl()
    {
        JumpPower = 10.0f;

        if (isSpace && !isJumping && !isDodging)
        {
            CRigidbody.AddForce(JumpPower * Vector3.up, ForceMode.Impulse);
            CAnimator.SetBool("isJump", true);
            CAnimator.SetTrigger("isJumpTrigger");
            isJumping = true;
        }
    }

    private void PlayerDodgeControl()
    {
        if (isF && !isJumping && !isDodging)
        {
            MovingSpeed += 3.5f;
            CAnimator.SetTrigger("isDodgeTrigger");
            isDodging = true;

            StartCoroutine(PlayerDogeEnd());
        }
    }

    private IEnumerator PlayerDogeEnd()
    {
        float CoroutineDelayTime = 0.5f;

        yield return new WaitForSeconds(CoroutineDelayTime);

        isDodging = false;
    }

    private void PlayerPickUpItemControl()
    {
        Item CItem;
        int ItemIndex;

        if (isE && !isJumping && !isShift)
        {
            if (PrefabGameObject.CompareTag("Weapon"))
            {
                CItem = PrefabGameObject.GetComponent<Item>();
                ItemIndex = CItem.ItemValue;
                hasWeapon[ItemIndex] = true;
                Destroy(PrefabGameObject);
            }
        }
    }

    private void PlayerWeaponSwapControl()
    {
        int ItemIndex = -2;

        if (isWeaponSwapHammer)
            ItemIndex = 0;
        else if (isWeaponSwapHandGun)
            ItemIndex = 1;
        else if (isWeaponSwapMachineGun)
            ItemIndex = 2;

        if ((isWeaponSwapHammer || isWeaponSwapHandGun || isWeaponSwapMachineGun) && !isJumping && !isShift)
        {
            if (EquipWeapon != null)
                EquipWeapon.gameObject.SetActive(false);

            EquipWeapon = Weapons[ItemIndex].GetComponent<Weapon>();
            EquipWeapon.gameObject.SetActive(true);

            CAnimator.SetTrigger("isSwapTrigger");
            isSwapWeapon = true;

            StartCoroutine(PlayerWeaponSwapEnd());
        }          
    }

    private IEnumerator PlayerWeaponSwapEnd()
    {
        float CoroutineDelayTime = 0.5f;

        yield return new WaitForSeconds(CoroutineDelayTime);

        isSwapWeapon = false;
    }

    private void PlayerAttackControl()
    {
        if (EquipWeapon == null)
            return;

        if (isLeftCtrl && !isJumping && !isDodging && !isSwapWeapon)
        {
            EquipWeapon.UseWeapon();
            CAnimator.SetTrigger("isSwingTrigger");
        }            
    }
}