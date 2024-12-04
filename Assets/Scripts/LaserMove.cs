using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LaserMove : MonoBehaviour
{
     [SerializeField] LayerMask platform_layer;
    // Start is called before the first frame update
    public float speedx=-20f;
    private Transform target;
    public bool player_sent = false;

    void Start()
    {
    }

    // Update is called once per frame

    void Update() {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        if (Mathf.Abs(target.position.x - transform.position.x)<1f && Mathf.Abs(target.position.y - transform.position.y)<1f && !player_sent){
            if (!GameObject.Find("Player").GetComponent<Mover>().inside_capsule)
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<Mover>().Die();
            }
            else
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<EnemyMove>().Die();
            }
            Debug.Log("Kill");
            GetComponent<Renderer>().enabled=false;
        }
        GetComponent<SpriteRenderer>().flipX = false;
        //GetComponent<BoxCollider2D>().offset = new Vector2(0.025f, -0.015f);
        GetComponent<Rigidbody2D>().velocity = new Vector2(speedx, GetComponent<Rigidbody2D>().velocity[1]);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (player_sent && collision.gameObject.tag == "Enemy" && !collision.gameObject.GetComponent<EnemyMove>().player_controlled)
        {
            collision.gameObject.GetComponent<EnemyMove>().Die();
            GetComponent<Renderer>().enabled = false;
        }
    }
    public void posdir()
    {
        speedx=20f;
    }
    public void negdir()
    {
        speedx=-20f;
    }
    

    private bool collidesplat(float speedx)
    {
        RaycastHit2D raycast= Physics2D.Raycast(transform.position, speedx*new Vector2(1,0), 2, LayerMask.GetMask("Platforms"));
            if (raycast.collider != null)
                Debug.Log(raycast.collider.gameObject.tag);
        return raycast.collider != null && raycast.collider.gameObject.tag=="Barril";    
    }    
}
