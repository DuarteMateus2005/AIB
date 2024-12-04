using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemyMove : MonoBehaviour
{
    [SerializeField] LayerMask platform_layer;
    // Start is called before the first frame update
    public float speedx=-12f, forcey;
    public GameObject laser;
    //public int attacktime=0,maxattacktime=3000;
    public float initx, maxpatrol=25f;
    private float lasttimepseen;
    private bool playerseen=false;
    private float timetoshoot;
    public bool dead=false;
    private float timeofdead;
    public bool attacking = false;
    public bool player_controlled = false;
    [SerializeField] float movementspeed = 15, jumpspeed = 40;
    bool a_keypressed, d_keypressed;
    float atk_cooldown = 0;
    private Transform target;

    void Start()
    {
        initx=transform.position.x;
    }

    // Update is called once per frame

    void patrol() { //Esta função indica o comportamento do inimigo em modo patrulha (modo diferente do de ataque)
        GetComponent<Animator>().SetBool("attacking", false);
        GetComponent<Animator>().SetTrigger("enemypatrol"); //Muda a animação para patrulha
        playerseen=false; //Se está neste modo não vê o player
       
        
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
        GetComponent<Rigidbody2D>().velocity = new Vector2(speedx, GetComponent<Rigidbody2D>().velocity[1]);
        if (Mathf.Abs(transform.position.x-initx)>maxpatrol && speedx*(transform.position.x-initx)>=0 || collidesplat(speedx)) { //Inverte a direção se chegou ao fim da patrulha ou colide
            speedx=-speedx;
        }


    }

    void attack() {//Esta função indica o comportamento do inimigo em modo ataque (modo diferente do de patrulha)
        GetComponent<Animator>().SetTrigger("enemyattack");
        GetComponent<Animator>().SetBool("attacking", true);
        playerseen =true;
        target = GameObject.FindGameObjectWithTag("Player").transform;

        //O initx indica o centro da patrulha, assim fica a patrulhar no último sitio que teve a atacar
        initx=transform.position.x;
        
        //Fica na direção do player
        if(target.position.x - transform.position.x>0) speedx=Mathf.Abs(speedx);
        else speedx=-Mathf.Abs(speedx);
        
        //Se colidir muda de direção
        if (collidesplat(speedx)) 
            speedx=-speedx;

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
        GetComponent<Rigidbody2D>().velocity = new Vector2(speedx, GetComponent<Rigidbody2D>().velocity[1]);
    

        
        //Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y, 0f);
        //Instantiate(laser, spawnPosition, Quaternion.identity, transform);


    }

    void Update()
    {
       if (!dead){
            // The enemy attacks if it sees the player or he has seen the player seen less than one second ago
            if (player_controlled)
            {
                if (Input.GetMouseButtonDown(0) && Time.time - atk_cooldown > 0.8)  //Play attack1 animation on mouse click
                {
                    if (Input.mousePosition[0] < Screen.width / 2)
                    {
                        GetComponent<SpriteRenderer>().flipX = true;
                    }
                    else
                    {
                        GetComponent<SpriteRenderer>().flipX = false;
                    }
                    GetComponent<Animator>().SetBool("attacking", true);
                    GetComponent<Animator>().SetTrigger("enemyattack");
                    atk_cooldown = Time.time;
                }

                if (Time.time - atk_cooldown > 0.8)
                {
                    GetComponent<Animator>().SetBool("attacking", false);
                    GetComponent<Animator>().SetTrigger("enemypatrol");
                }

                speedx = Input.GetAxis("Horizontal") * movementspeed;  //Calculate velocity x

                if (!Input.anyKey)
                {
                    a_keypressed = false;
                    d_keypressed = false;
                }
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    a_keypressed = true;
                }

                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    d_keypressed = true;
                }

                if ((a_keypressed || d_keypressed) && OnGround() && !Input.GetKeyDown("space"))  //Change animations between running and standing
                {
                    GetComponent<Animator>().SetTrigger("enemypatrol");
                    forcey = 0;
                }
                else if (Input.GetKeyDown("space") && OnGround())
                {
                    forcey = jumpspeed;
                }
                else
                {
                    GetComponent<Animator>().SetTrigger("enemypatrol");
                    forcey = 0;
                }

                GetComponent<Rigidbody2D>().velocity = new Vector2(speedx, GetComponent<Rigidbody2D>().velocity[1]);  //Update the horizontal speed
                GetComponent<Rigidbody2D>().AddForce(new Vector2(0, forcey), ForceMode2D.Impulse);  //Apply jump force

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
            else
            {
                if (playeratSight(speedx) || (playerseen && Time.time - lasttimepseen < 1f)) attack();
                else patrol();
                GetHit();
            }
       } 
       else if(Time.time-timeofdead>2f) GetComponent<Renderer>().enabled=false;

    }
   
     private bool playeratSight(float speedx) // Checks if the player can be seen taking into account the direction speedx
    {
        RaycastHit2D raycast= Physics2D.Raycast(transform.position, speedx*new Vector2(1,0), 10, LayerMask.GetMask("Player"));// Raycast detects through everything
        bool b=raycast.collider != null && raycast.collider.gameObject.tag=="Player";   
        if (b) lasttimepseen=Time.time; // If he sees the player the lasttimeseen is now Time.time
        return b; 
    }

    private void createBullet(){ // Esta função é chamada por evento na animação quando o raio aparece
        laser = GameObject.Find("Laser");
        if (!dead){
            GameObject lsr;
            if(GetComponent<SpriteRenderer>().flipX) {
                Vector3 spawnPosition = new Vector3(transform.position.x-0.2f, transform.position.y+0.5f, 0f);
                lsr = Instantiate(laser, spawnPosition, Quaternion.identity, transform); //Cria a bala da dir para esquerda
                lsr.GetComponent<LaserMove>().negdir();   
            }
            else {
                Vector3 spawnPosition = new Vector3(transform.position.x+0.2f, transform.position.y+0.5f, 0f);
                lsr = Instantiate(laser, spawnPosition, Quaternion.identity, transform); //Cria a bala da dir para esquerda
                lsr.GetComponent<LaserMove>().posdir();
            }
            if (player_controlled)
            {
                lsr.GetComponent<LaserMove>().player_sent = true;
            }
        }
    }

    private bool collidesplat(float speedx) // Testa se choca com um parede ou barril 
    {
        RaycastHit2D raycast= Physics2D.Raycast(transform.position, speedx*new Vector2(1,0), 2, LayerMask.GetMask("Platforms"));
            /*if (raycast.collider != null)
                Debug.Log(raycast.collider.gameObject.tag);*/
        return raycast.collider != null && raycast.collider.gameObject.tag=="Barril";    
    }  

    public void Die(){
        speedx=0;
        dead=true;
        timeofdead=Time.time;
        GetComponent<Animator>().SetTrigger("enemydie");
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponent<BoxCollider2D>().enabled = false;
    }
    private bool OnGround()
    {
        RaycastHit2D raycast = Physics2D.Raycast(GetComponent<Transform>().position, Vector2.down, 3f, platform_layer);
        //Debug.Log(raycast.collider != null);
        return raycast.collider != null;
    }
    void GetHit()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        if (Mathf.Abs(target.position.x - transform.position.x) < 3f && Mathf.Abs(target.position.y - transform.position.y) < 3f && !GameObject.Find("Player").GetComponent<Mover>().inside_capsule)
        {
            float attackdirplayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Mover>().isattacking();
            //Debug.Log("mygloat="+attackdirplayer);
            if (attackdirplayer != 0) Die();
            //else if (target.position.x - transform.position.x<=0 && attackdirplayer!=0) Die();
            //if(!playeratSight(speedx) || (GameObject.FindGameObjectWithTag("Player").GetComponent<Mover>().isattacking())*speedx>=0f)
            /*else if (!dead)
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<Mover>().Die();
                Debug.Log("Kill Player");
                Debug.Log("mygloat="+attackdirplayer);
            }
            speedx = 0f;*/
        }
    }
}
