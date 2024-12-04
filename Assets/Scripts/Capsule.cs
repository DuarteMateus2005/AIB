using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Capsule : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    public int num_enemies;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player" && GetNumberofEnemies() > 1)
        {
            Debug.Log("Capsule entered");
            collision.gameObject.transform.position = GetComponent<Transform>().position;
            collision.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
            collision.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Animator>().SetBool("empty", false);
            collision.gameObject.GetComponent<Mover>().inside_capsule = true;
            Transform enemytrans = GameObject.Find("Robot2").transform;
            vcam = GameObject.Find("Virtual Follow Camera").GetComponent<CinemachineVirtualCamera>();
            vcam.Follow = enemytrans;
            GameObject.Find("Robot2").GetComponent<EnemyMove>().player_controlled = true;
            Debug.Log(enemytrans);
            collision.gameObject.tag = "Untagged";
            collision.gameObject.layer = 0;
            GameObject.Find("Robot2").tag = "Player";
            GameObject.Find("Robot2").layer = 6;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animator>().SetBool("empty", true);
    }

    // Update is called once per frame
    void Update()
    {
        if (GetNumberofEnemies() == 0 && GameObject.Find("Player").GetComponent<Mover>().inside_capsule)
        {
            GameObject.Find("Player").GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            GameObject.Find("Player").GetComponent<BoxCollider2D>().enabled = true;
            GameObject.Find("Player").GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<Animator>().SetBool("empty", true);
            GameObject.Find("Player").GetComponent<Mover>().inside_capsule = false;
            vcam = GameObject.Find("Virtual Follow Camera").GetComponent<CinemachineVirtualCamera>();
            vcam.Follow = GameObject.Find("Player").GetComponent<Transform>();
            GameObject.Find("Robot2").GetComponent<EnemyMove>().player_controlled = false;
            GameObject.Find("Player").tag = "Player";
            GameObject.Find("Player").layer = 6;
            GameObject.Find("Robot2").tag = "Enemy";
            GameObject.Find("Robot2").layer = 10;
        }
    }
    int GetNumberofEnemies()
    {
        num_enemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Enemy").Length; i++)
        {
            if (GameObject.FindGameObjectsWithTag("Enemy")[i].GetComponent<EnemyMove>().dead)
            {
                num_enemies -= 1;
            }
        }
        Debug.Log(num_enemies);
        return num_enemies;
    }
}
