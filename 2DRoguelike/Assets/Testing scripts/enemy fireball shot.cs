using UnityEngine;

public class EnemyShooter2D : MonoBehaviour
{
    public GameObject bullet;

    public float bulletSpeed = 5;

    public float shotDelay = 3;

    GameObject player;

    float timer = 0;

    public float range = 9;

    void Start()

    {

        player = GameObject.FindGameObjectWithTag("Player");

    }



    // Update is called once per frame

    void Update()

    {

        //delta is change

        timer += Time.deltaTime;

        if (timer > shotDelay)

        {

            Vector3 shootDir = player.transform.position - transform.position;

            //magnitude shows how far

            if (shootDir.magnitude < range)

            {

                timer = 0;

                GameObject shot = Instantiate(bullet, transform.position, Quaternion.identity);

                shootDir.Normalize();

                shot.GetComponent<Rigidbody2D>().velocity = shootDir * bulletSpeed;

                Destroy(shot, 10);

            }

        }

    }

}