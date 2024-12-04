using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mover : MonoBehaviour
{
    [SerializeField] float movementspeed, jumpspeed;
    [SerializeField] LayerMask platform_layer;
    float speedx, speedy;
    bool a_keypressed, d_keypressed;
    // Start is called before the first frame update
    private bool dead = false;
    public bool attack = false;
    private float timeofattack = -0.3f;
    private float inbetweenattacks = 0.4f;
    private float deadtime = 0.4f;
    public bool inside_capsule = false;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!dead)
        {
            if (Input.GetMouseButtonDown(0) && Time.time - timeofattack > inbetweenattacks)  //Play attack1 animation on mouse click
            {
                if (Input.mousePosition[0] < Screen.width / 2) GetComponent<SpriteRenderer>().flipX = true;
                else GetComponent<SpriteRenderer>().flipX = false;

                GetComponent<Animator>().SetTrigger("Attack1");
                attack = true;
                timeofattack = Time.time;
            }

            //Attack with Return key
            if (Input.GetKeyDown(KeyCode.Return) && Time.time - timeofattack > inbetweenattacks)
            {
                GetComponent<Animator>().SetTrigger("Attack1");
                timeofattack = Time.time;
                attack = true;
                RaycastHit2D raycast = Physics2D.Raycast(transform.position, speedx * new Vector2(1, 0), 1, LayerMask.GetMask("Enemy"));
                if (raycast.collider != null) raycast.collider.gameObject.GetComponent<EnemyMove>().Die();
            }

            speedx = Input.GetAxis("Horizontal") * movementspeed;  //Calculate velocity x
            if (speedx >= 0) GetComponent<Animator>().SetFloat("Speedx", speedx);
            else if (speedx < 0) GetComponent<Animator>().SetFloat("Speedx", -speedx);
            if (!Input.anyKey)
            {
                a_keypressed = false;
                d_keypressed = false;
            }
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) a_keypressed = true;
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) d_keypressed = true;


            if ((a_keypressed || d_keypressed) && OnGround() && !Input.GetKeyDown("space"))  //Change animations between running and standing
            {
                GetComponent<Animator>().SetTrigger("Run");
                attack = false;
                speedy = 0f;
            }
            else if (Input.GetKeyDown("space") && OnGround())
            {
                attack = false;
                speedy = jumpspeed;
                GetComponent<Animator>().SetTrigger("Jumping");
                GetComponent<Animator>().SetFloat("Speedy", speedy);
            }
            else
            {
                // Debug.Log("Standing");
                GetComponent<Animator>().SetTrigger("Standing");
                attack = false;
                speedy = 0f;
                GetComponent<Animator>().SetFloat("Speedy", speedy);
            }



            GetComponent<Rigidbody2D>().velocity = new Vector2(speedx, GetComponent<Rigidbody2D>().velocity[1]);  //Update the horizontal speed
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, speedy), ForceMode2D.Impulse);  //Apply jump force

            GetComponent<Animator>().SetFloat("Speedy", GetComponent<Rigidbody2D>().velocity[1]);

            if (speedx > 0)  //Flip sprite and adjust box collider
            {
                GetComponent<SpriteRenderer>().flipX = false;
                GetComponent<BoxCollider2D>().offset = new Vector2(0.025f, -0.015f);
            }
            else if (speedx < 0)
            {
                GetComponent<SpriteRenderer>().flipX = true;
                GetComponent<BoxCollider2D>().offset = new Vector2(-0.025f, -0.015f);
            }
        }
        else if (Time.time - deadtime > 5f) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private bool OnGround()
    {
        RaycastHit2D raycast = Physics2D.BoxCast(GetComponent<BoxCollider2D>().bounds.center, GetComponent<BoxCollider2D>().bounds.size, 0f, Vector2.down, 0.1f, platform_layer);
        //Debug.Log(raycast.collider!=null);
        return raycast.collider != null;
    }

    public void Die()
    {
        GetComponent<Animator>().SetTrigger("Die");
        GetComponent<SpriteRenderer>().flipX = false;
        //Debug.Log(raycast.collider!=null);
        dead = true;
        deadtime = Time.time;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponent<BoxCollider2D>().enabled = false;
    }
    public float isattacking()
    {
        bool b = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("playerattack1");
        if (b && GetComponent<SpriteRenderer>().flipX == false) return 1.0f;
        else if (b) return -1.0f;
        else return 0f;
    }
}
