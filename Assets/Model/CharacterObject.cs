using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioConfig;
using UnityEngine.UI;
using System.IO;

[RequireComponent(typeof(Rigidbody2D), typeof(BetterJumping), typeof(CharacterStat))]
public class CharacterObject : MonoBehaviour
{
    [Header("Static Trigger")]
    public bool canAttack = true;
    public bool canMove = true, canJump = true, isPlayer = false;

    [Header("Ienumarator Trigger")]
    public bool grounded = true;
    public bool death = false, run = false, invisible = false, diChuyen, attackable = true, flipable = true;
    public bool toggleRunEnable = true, holdWeapon = false, enableJumpDouble = false, enableFootSound = false;

    [Header("Animation Trigger")]
    public bool roll = false; 
    public bool atHome = false, takeDam = false, doubleJump = false;
    public int weaponAttack = 0, weaponAnimId = 0;

    [HideInInspector]
    public Rigidbody2D r2;
    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public GameObject colider;
    [HideInInspector]
    public AudioSource audioSource;
    [HideInInspector]
    public CharacterStat charStat;
    [HideInInspector]
    public GameController gc;

    int blendShapeCount;
    SkinnedMeshRenderer skinnedMeshRenderer;
    Mesh skinnedMesh;

    [Header("Value")]
    public int faceRight = 1;
    [Range(0f, 500f)]
    public float speed = 250f, maxspeed = 1, jumpPow = 350f, defaultSpeed;
    public string target1Tag, target2Tag;
    public bool layDsBieuCam = false;

    public void SetValuesStart()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        r2 = gameObject.GetComponent<Rigidbody2D>();//Lay nhan vat
        anim = gameObject.GetComponent<Animator>();//Bien chua animation cho Player
        diChuyen = true;//co the di chuyen
        defaultSpeed = speed;//speed ban dau
        colider = gameObject.transform.Find("Collider").gameObject;
        audioSource = gameObject.GetComponent<AudioSource>();
        charStat = gameObject.GetComponent<CharacterStat>();
        SoundManager.SetSoundVolumeToObject(gameObject);
        //Khoi tao cac bien bieu cam nhan vat
        GameObject face = gameObject.transform.Find("Face").gameObject;
        if (face != null)
        {
            skinnedMeshRenderer = face.GetComponentInChildren<SkinnedMeshRenderer>();
            skinnedMesh = skinnedMeshRenderer.sharedMesh;
            blendShapeCount = skinnedMesh.blendShapeCount;
        }
        //Lay danh sach bieu cam cua nhan vat
        if (skinnedMesh != null && layDsBieuCam)
        {
            string path = "Assets/Resources/BieuCam.txt";
            StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine(gameObject.name + ":");
            for (int i = 0; i < blendShapeCount; i++)
            {
                Debug.Log(i + " " + skinnedMesh.GetBlendShapeName(i));
                writer.WriteLine(i + " - " + skinnedMesh.GetBlendShapeName(i));
            }
            writer.WriteLine("--------------------");
            writer.Close();
        }
    }

    public void EnableCharacter()
    {
        canAttack = true;
        canJump = true;
        canMove = true; 
    }

    public void DisableCharacter()
    {
        DiChuyenNhanVat(0);
        canAttack = false;
        canJump = false;
        canMove = false;
    }

    public void SetAnimatiorAndValuesUpdate()
    {
        anim.SetInteger("WeaponAnimId", weaponAnimId);
        anim.SetInteger("weaponAttack", weaponAttack);
        anim.SetBool("Roll", roll);
        anim.SetBool("AtHome", atHome);
        anim.SetBool("DoubleJump", doubleJump);
        anim.SetBool("TakeDamage", takeDam);
        anim.SetBool("Death", death);
        if (Mathf.Abs(r2.velocity.x) == 0 && grounded)
        {
            anim.SetLayerWeight(0, 1);
            anim.SetLayerWeight(1, 0);
            anim.SetLayerWeight(2, 0);
        }

        if (grounded)
        {
            anim.SetLayerWeight(3, 0);
        }
        else
        {
            anim.SetLayerWeight(3, 1);
        }  
    }

    public void DiChuyenNhanVat(float move)
    {
        if (diChuyen && canMove)
        {
            KhoiTaoSpeed(move);
            r2.AddForce(Vector2.right * speed * move);//di chuyen
            if (run)
            {
                anim.SetLayerWeight(2, Mathf.Abs(move));
                anim.SetLayerWeight(1, 0);               
            }
            else
            {
                anim.SetLayerWeight(1, Mathf.Abs(move));
                anim.SetLayerWeight(2, 0);
            }

            //Ham gioi han toc do di chuyen
            if (r2.velocity.x > maxspeed) //Gioi han toc do di ve ben phais
                r2.velocity = new Vector2(maxspeed, r2.velocity.y);
            if (r2.velocity.x < -maxspeed)// Gioi han toc do di ve ben trai
                r2.velocity = new Vector2(-maxspeed, r2.velocity.y);

            if (flipable)
            {
                if (move > 0 && faceRight == -1)
                {
                    Flip();//Goi ham dao chieu 
                }
                if (move < 0 && faceRight == 1)
                {
                    Flip();
                }
            }

            if (grounded)
            {
                r2.velocity = new Vector2(r2.velocity.x * 0.7f, r2.velocity.y);
            }

            if (!grounded)
            {
                r2.velocity = new Vector2(r2.velocity.x * 0.7f, r2.velocity.y);
            }

            
        }
        else
        {
            move = 0;
            r2.AddForce(Vector2.right * speed * move);
        }
    }

    private void KhoiTaoSpeed(float move)
    {
        if (move != 0)//Kiem tra xem dang dung yen hay di chuyen
        {
            if (run)//neu chay
            {
                speed = defaultSpeed + 150f;//tang toc do di chuyen
            }
            else//neu di binh thuong
            {
                speed = defaultSpeed;//toc do mac dinh
                if (atHome || holdWeapon)//neu dang o nha hoac cam vu khi
                {
                    speed = defaultSpeed - 150f;//giam toc do di chuyen
                    return;
                }
            }
            if (isPlayer)
            {
                if (grounded && !enableFootSound && Mathf.Abs(r2.velocity.x) != 0)
                    if (run)
                        StartCoroutine(PlayFootStepSound(0.2f));
                    else
                        StartCoroutine(PlayFootStepSound(0.3f));
            }
                
        }
    }

    IEnumerator PlayFootStepSound(float delay)
    {
        enableFootSound = true;
        yield return new WaitForSeconds(delay);
        gc.PlayASound(Resources.Load<AudioClip>("Audio/SoundEffect/Player/Footstep04"));
        enableFootSound = false;
    }

    public void Flip() // Chuyen huong nhan vat
    {
        faceRight = -faceRight;
        gameObject.GetComponent<Transform>().localScale = new Vector3(1, 1, -gameObject.GetComponent<Transform>().localScale.z);
    }

    public void Jump()
    {
        if (diChuyen && canJump) // neu nut an xuong cua nguoi choi la Space va dang cho phep di chuyen (diChuyen = true)
        {
            gameObject.GetComponent<GroundCheck>();//Goi ham kiem tra xem Player co dang dung tren mat dat hay khong
            if (grounded)//neu dang dung tren mat dat
            {
                grounded = false;//cho grounded = false tuc la nguoi choi se nhay len khong
                r2.AddForce(Vector2.up * jumpPow);//thay doi vi tri nhan vat len tren dua vao jumpPow
                enableJumpDouble = true;
            }
            else
            {
                if (enableJumpDouble)
                    StartCoroutine(PerformDoubleJump());
            }
        }
    }

    private void RunToggle()
    {
        if (toggleRunEnable && canMove)//khi nhan Shift de chuyen doi giua chay nhanh hoac di bo
        {
            StartCoroutine(ToggleRun());
        }
    }

    private void Roll()
    {
        if (diChuyen && canMove && holdWeapon)
        {
            StartCoroutine(PerformRoll());
        }           
    }

    private void RollBackward()
    {
        if (diChuyen && canMove && grounded)
        {
            StartCoroutine(PerformRollBackward());
        }            
    }

    public void ThayDoiBieuCam(int bieuCamIndex, float mucDo)
    {
        if (bieuCamIndex <= blendShapeCount)
            skinnedMeshRenderer.SetBlendShapeWeight(bieuCamIndex, mucDo);
    }

    private IEnumerator ToggleRun()
    {
        toggleRunEnable = false;
        run = !run;
        ColorBlock colorButton = gc.touchButton.buttonRun.GetComponent<Button>().colors;
        if (run)
        {
            colorButton.normalColor = new Color((float)41 / 255, (float)245 / 255, (float)6 / 255, (float)150 / 255);
            colorButton.highlightedColor = new Color((float)41 / 255, (float)245 / 255, (float)6 / 255, (float)150 / 255);
            colorButton.pressedColor = new Color((float)41 / 255, (float)245 / 255, (float)6 / 255, (float)200 / 255);
            colorButton.selectedColor = new Color((float)41 / 255, (float)245 / 255, (float)6 / 255, (float)150 / 255);
        }
        else
        {
            colorButton.normalColor = new Color((float)255 / 255, (float)255 / 255, (float)255 / 255, (float)100 / 255);
            colorButton.highlightedColor = new Color((float)255 / 255, (float)255 / 255, (float)255 / 255, (float)100 / 255);
            colorButton.pressedColor = new Color((float)255 / 255, (float)255 / 255, (float)255 / 255, (float)150 / 255);
            colorButton.selectedColor = new Color((float)255 / 255, (float)255 / 255, (float)255 / 255, (float)100 / 255);
        }
        gc.touchButton.buttonRun.GetComponent<Button>().colors = colorButton;
        yield return new WaitForSeconds(0.5f);
        toggleRunEnable = true;
    }

    private IEnumerator PerformRoll()
    {
        diChuyen = false;
        if (weaponAttack > 0)
        {
            weaponAttack = 0;
            yield return new WaitForSeconds(0.1f);
        }       
        r2.velocity = Vector2.zero;
        roll = true;
        r2.AddForce(Vector2.right * 250f * 2f * faceRight);
        invisible = true;
        yield return new WaitForSeconds(0.5f);
        invisible = false;
        r2.velocity = Vector2.zero;
        roll = false;
        diChuyen = true;
    }

    private IEnumerator PerformRollBackward()
    {
        diChuyen = false;
        r2.velocity = Vector2.zero;
        roll = true;
        invisible = true;
        yield return new WaitForSeconds(0.2f);
        r2.AddForce(Vector2.right * 250f * 2f * -faceRight);
        yield return new WaitForSeconds(0.3f);
        invisible = false;
        r2.velocity = Vector2.zero;
        roll = false;
        diChuyen = true;
    }

    private IEnumerator PerformDoubleJump()
    {
        enableJumpDouble = false;
        doubleJump = true;
        r2.velocity = new Vector2(r2.velocity.x, 0);
        r2.AddForce(Vector2.up * jumpPow * 0.8f);
        SoundManager.PlaySound(gameObject, Resources.Load<AudioClip>("Audio/SoundEffect/Player/Jump"));
        yield return new WaitForSeconds(0.2f);
        doubleJump = false;
    }

    #region NhanSatThuong
    public void TakeDamage(float damageAmount)//Nhan sat thuong
    {
        if (!invisible && charStat.hp > 0)
        {
            if (!gameObject.CompareTag("Boss"))
            {
                anim.SetLayerWeight(4, 1);
                DiChuyenNhanVat(0);
                takeDam = true;
                diChuyen = false;
            }                
            charStat.hp -= damageAmount;
            if (gameObject.CompareTag("Enemy") && !gameObject.GetComponent<EnemyController>().detected && !gameObject.GetComponent<EnemyController>().curious)
            {
                charStat.hp -= (damageAmount * 14);
                gameObject.GetComponent<EnemyController>().detected = true;
                gameObject.GetComponent<EnemyController>().curious = true;
                if (charStat.hp > 0)
                    Flip();
            }                           
        }
    }

    public void GetKBack(float knockBackPower)
    {
        if (!invisible)
        {
            StartCoroutine(KnockBack(knockBackPower));
        }
    }

    IEnumerator KnockBack(float knockBackPower)
    {
        if (!gameObject.CompareTag("Boss"))
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(knockBackPower * -faceRight, 0));
        if (charStat.hp <= 0)
        {
            gameObject.SendMessage("Defeated", SendMessageOptions.DontRequireReceiver);
            if (!death)
            {
                anim.SetLayerWeight(4, 1);
                death = true;
                takeDam = false;
                attackable = false;
                canMove = false;
                canAttack = false;
                canJump = false;
                gameObject.tag = "Death";
                colider.tag = "Death";
                invisible = true;
                gameObject.SendMessage("DropLoot", SendMessageOptions.DontRequireReceiver);
                yield return new WaitForSeconds(0.1f);
                StopAllCoroutines();
                yield break;
            }
            else
            {
                StopAllCoroutines();
                yield break;
            }
        }
        else
        {
            if (!gameObject.CompareTag("Boss"))
            {
                yield return new WaitForSeconds(0.5f);
                anim.SetLayerWeight(4, 0);
                takeDam = false;
                diChuyen = true;//di chuyen lai binh thuong
            }            
        }    
    }
    #endregion

}
