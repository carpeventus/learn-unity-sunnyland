using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour {

    [Header("Physics")]
    private Rigidbody2D rb;

    private BoxCollider2D col;
    public LayerMask groundLayer;

    [Header("Move")] 
    public float walkSpeed = 5f;
    public float maxWalkSpeed = 8f;
    public float runSpeed = 13f;
    public float maxRunSpeed = 16f;
    public float linearDrag = 4f;
    public float hurtBackVx = 8f;
    public float maxHurtDistance = 4f;
    public Vector2 rayCastOffset;
    public float groundLength = 1.1f;
    public float headLength = 0.4f;
    private float hurtPoint;
    private bool hurting;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;
    private Vector2 crouchColliderSize;
    private Vector2 crouchColliderOffset;
    
    private Vector2 direction;
    private bool faceRight;
    private bool holdCrouching;
    private bool holdJumping;
    private bool holdRuning;
    
    [Header("Jump")] 
    public bool onGround;
    public float jumpDelay = 0.1f;
    public float jumpForce = 10f;
    public float gravity = 1;
    public float fallMulti = 10f;
    public int remainJumpCount = 2;
    private bool canChargeJump = true;
    private float chargeJumpTime;
    private float jumpTimer;

    [Header("Collections")] 
    public int gemNum;
    public int cherryNum;
    public int score;
    public int enemyScore = 1000;
    public TMP_Text gemText;
    public TMP_Text cherryText;
    public TMP_Text scoreText;

    [Header("Sound")] 
    private AudioSource soundPlayer;
    public AudioSource bgm;
    public AudioClip jumpSound;
    public AudioClip hurt;
    public AudioClip death;
    public AudioClip charged;
    
    public bool ownDoubleJumpAbility { set; get; }
    public bool ownChargeJumpAbility { set; get; }
    
    [Header("Health")] 
    public Image[] heartContainer;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public int totalHeart;
    public int health;
    
    [Space]
    private Animator animator;
    private static string RUN_ANIMATO_PARAM = "runSpeed";
    private static string JUMP_ANIMATO_PARAM = "jumping";
    private static string FALL_ANIMATO_PARAM = "falling";
    private static string IDLE_ANIMATO_PARAM = "idle";
    private static string HURT_ANIMATO_PARAM = "isHurt";
    private static string CROUCH_ANIMATO_PARAM = "crouching";
    private static string CHARGED_ANIMATO_PARAM = "charged";
    
    void Start() {
        faceRight = true;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        soundPlayer = GetComponent<AudioSource>();
        originalColliderSize = col.size;
        originalColliderOffset = col.offset;
        // 我也不知道怎么计算可以适配🦊蹲下时的Collider范围，所以下面的魔法值是在编辑器里调整后得到的经验值。大概还有更好的方案
        crouchColliderSize = new Vector2(col.size.x, col.size.y * 0.7f);
        crouchColliderOffset = new Vector2(col.offset.x, col.offset.y * 1.5f);
    }


    // Update is called once per frame
    void Update() {
        onGround = Raycast(rayCastOffset, Vector2.down, groundLength, groundLayer)
            || Raycast(-rayCastOffset, Vector2.down, groundLength, groundLayer);
     direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));


        /**
         * 先按一次跳，在下一次FixedUpdate中，会跳起来，随后jumpTimer置为0
         * 此时在半空中，再次输入跳跃，jumpTimer比Time.time大jumpDelay
         * 如果下一次执行FixedUpdate时，玩家在地上且jumpTimer还大于Time.time
         * 且就会自动跳跃起来。和格斗游戏里常见的预输入很像。
         */
        if (Input.GetButtonDown("Jump")) {
            jumpTimer = Time.time + jumpDelay;
        }
        
        holdRuning = Input.GetButton("Fire");
        holdCrouching = Input.GetButton("Crouch");
        holdJumping = Input.GetButton("Jump");
        
        if (onGround && holdCrouching && direction.x == 0) {
            chargeJumpTime += Time.deltaTime;
        } else {
            // 跳跃/站起来/移动都会取充能
            ResetChargeJump();
        }
        
        if (onGround) {
            ResetJumpCount();
        }

        SwitchAnimation();
        ChangeHealth();
    }

    
    private void ChangeHealth() {
        // 不能超过当前上限
        if (health > totalHeart) {
            health = totalHeart;
        }

        for (int i = 0; i < heartContainer.Length; i++) {
            if (i < health) {
                heartContainer[i].sprite = fullHeart;
            }
            else {
                heartContainer[i].sprite = emptyHeart;
            }
            
            if (i < totalHeart) {
                heartContainer[i].enabled = true;
            }
            else {
                heartContainer[i].enabled = false;
            }
        }
        
    }

    private void DecreaseHealth() {
        health--;
    } 
    
    private void ResetChargeJump() {
        StopAnimation(CHARGED_ANIMATO_PARAM);
        chargeJumpTime = 0;
        canChargeJump = true;
    }

    private bool JumpCharged() {
        return chargeJumpTime >= 1.5f;
    }


    private void FixedUpdate() {
        Move(direction.x);
        Jump(jumpForce);
        modifyPhysics();
    }


    void modifyPhysics() {
        bool changingDirections = (direction.x > 0 && rb.velocity.x < 0) || (direction.x < 0 && rb.velocity.x > 0);
        if (onGround) {
            if (Mathf.Abs(direction.x) < 0.4f || changingDirections) {
                rb.drag = linearDrag;
            }
            else {
                rb.drag = 0f;
            }
            rb.gravityScale = 0f;
        }
        // 离开里面时，增加重力效果
        else {
            rb.gravityScale = gravity;
            // 消除线性阻力（不完全）
            rb.drag = linearDrag * 0.15f;
            // 下落时
            if (rb.velocity.y < 0) {
                rb.gravityScale = gravity * fallMulti;
            }
            // 上升过程中，但是已经释放了跳跃
            else if (rb.velocity.y > 0 && !holdJumping) {
                rb.gravityScale = gravity * (fallMulti / 2);
            }
        }
    }

    void Move(float horizontal) {
        if (hurting) {
            return;
        }
        if (shouldFilp(horizontal)) {
            Flip();
        }


        if (holdCrouching) {
            rb.AddForce(Vector2.right * horizontal * walkSpeed / 3);
            if (Mathf.Abs(rb.velocity.x) > maxWalkSpeed / 3) {
                rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxWalkSpeed / 3, rb.velocity.y);
            }
        }
        else if (holdRuning) {
            rb.AddForce(Vector2.right * horizontal * runSpeed);
            if (Mathf.Abs(rb.velocity.x) > maxRunSpeed) {
                rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxRunSpeed, rb.velocity.y);
            }
        } else {
            rb.AddForce(Vector2.right * horizontal * walkSpeed);
            if (Mathf.Abs(rb.velocity.x) > maxWalkSpeed) {
                rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxWalkSpeed, rb.velocity.y);
            }
        }
        
        Crouch(holdCrouching);
    }

    private bool shouldFilp(float horizontal) {
        return horizontal > 0 && !faceRight || horizontal < 0 && faceRight;
    }

    void Flip() {
        faceRight = !faceRight;
        transform.rotation = Quaternion.Euler(0, faceRight ? 0 : 180, 0);
    }

    void Crouch(bool crouch) {
        
        if (crouch) {
            PlayAnimation(CROUCH_ANIMATO_PARAM);
            col.size = crouchColliderSize;
            col.offset = crouchColliderOffset;
            if (JumpCharged()) {
                PlayChargedAnimation();
            }
        } else {
            // 编辑器里调整后的经验值
            bool touchHead = Raycast(new Vector2(0, -0.7f), Vector2.up, headLength, groundLayer);
            // 没有接触到
            if (!touchHead) {
                col.size = originalColliderSize;
                col.offset = originalColliderOffset;
                StopAnimation(CROUCH_ANIMATO_PARAM);
            }
        }
    }

    private void PlayChargedAnimation() {
        PlayAnimation(CHARGED_ANIMATO_PARAM);
        if (canChargeJump) {
            soundPlayer.PlayOneShot(charged);
            canChargeJump = false;
        }
    }
    public void Jump(float jumpForce) {
        if (Time.time < jumpTimer) {
            Jump(jumpForce, true);
        }
    }

    public void Jump(float jumpForce, bool playSound) {
        if (hurting) {
            return;
        }
        
        if (JumpCharged()) {
            jumpForce *= 1.5f;
        }
        
        if (remainJumpCount > 0) {
            // 重置Y轴速度
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            if (playSound) {
                soundPlayer.PlayOneShot(jumpSound);
            }

            DecreaseJumpCount();
            // 防止跳跃多次
            jumpTimer = 0f;
        }
    }

    private void ResetJumpCount() {
        remainJumpCount = ownDoubleJumpAbility ? 2 : 1;
    }
    
    private void DecreaseJumpCount() {
        remainJumpCount--;
    }

    public void IncreaseJumpCount() {
        remainJumpCount++;
    }

    void SwitchAnimation() {
        SetFloatAnimation(RUN_ANIMATO_PARAM, Mathf.Abs(rb.velocity.x));
        // 这里有BUG，暂时不知道怎么解决，可能出现onGround了但是动画没有回到IDLE，下面
        if (onGround) {
            if (animator.GetBool(FALL_ANIMATO_PARAM)) {
                StopAnimation(FALL_ANIMATO_PARAM);
            }
            else if (animator.GetBool(JUMP_ANIMATO_PARAM)){
                StopAnimation(JUMP_ANIMATO_PARAM);
            }
        } else if (!onGround && rb.velocity.y > 0) {
            PlayAnimation(JUMP_ANIMATO_PARAM);
        } else if (rb.velocity.y < 0) {
            StopAnimation(JUMP_ANIMATO_PARAM);
            PlayAnimation(FALL_ANIMATO_PARAM);
        }

        if (hurting) {
            PlayAnimation(HURT_ANIMATO_PARAM);
            // 当横行速度于哦，或者移动距离超过了1.5倍的击退距离，此时判断未受伤。加后者主要是因为在isHurt时玩家虽然不能移动但是可以控制方向使得rb.velocity.x变大
            if (Mathf.Abs(rb.velocity.x) < 0.3f || Mathf.Abs(transform.position.x - hurtPoint) > maxHurtDistance) {
                hurting = false;
                StopAnimation(HURT_ANIMATO_PARAM);
            }
        }
    }


    void StopAnimation(String param) {
        animator.SetBool(param, false);
    }

    void PlayAnimation(string param) {
        animator.SetBool(param, true);
    }

    void SetFloatAnimation(string param, float speed) {
        animator.SetFloat(param, speed);
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.tag.Equals("DeadLine")) {
            col.enabled = false;
            DeathAndRestart();
        }
    }
    
    private void DeathAndRestart() {
        bgm.Stop();
        soundPlayer.PlayOneShot(death);
        Invoke("ReStart", death.length);
    }
    
    public void Hurted(bool right) {
        hurting = true;
        hurtPoint = transform.position.x;
        rb.velocity = new Vector2(right ? hurtBackVx : -hurtBackVx, rb.velocity.y);
        soundPlayer.PlayOneShot(hurt);
        DecreaseHealth();
        
        if (health <= 0) {
            DeathByHurt();
        }
    }
    private void DeathByHurt() {
        GetComponent<Collider2D>().enabled = false;
        rb.velocity = new Vector2(rb.velocity.x, -20f);
        rb.velocity = new Vector2(rb.velocity.x, 10f);
        DeathAndRestart();
    }

    private void ReStart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void IncreaseCherryNum() {
        cherryNum++;
        cherryText.text = cherryNum.ToString();
    }
    
    public void IncreaseGemNum() {
        gemNum++;
        gemText.text = gemNum.ToString();
    }
    
    public void IncreaseScore() {
        score += enemyScore;
        scoreText.text = score.ToString();
    }

    public bool CanHitEnemy(float enemyY) {
        return rb.velocity.y < 0 && transform.position.y > enemyY;
    }

    RaycastHit2D Raycast(Vector2 offset, Vector2 direction, float distance, LayerMask layerMask) {
        Vector2 player = transform.position;
        return Physics2D.Raycast(player + offset, direction, distance, layerMask);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Vector2 position = transform.position;
        Gizmos.DrawLine(position + rayCastOffset, position + rayCastOffset + Vector2.down * groundLength);
        Gizmos.DrawLine(position - rayCastOffset, position - rayCastOffset + Vector2.down * groundLength);
        Gizmos.DrawLine(position + new Vector2(0, -0.7f) , position  + Vector2.up * headLength);
    }
}