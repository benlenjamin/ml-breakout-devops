using System;
using NUnit.Framework;
using Unity.MLAgents.Integrations.Match3;
using UnityEditor.UI;
using UnityEngine;

public class BallController : MonoBehaviour
{
    // *****************************
    // * PUBLIC VARIABLES -> START *
    // *****************************

    public float initialSpeed = 5f;
    public float ballX;
    public float ballY;    

    public float ignoreCollisionDuration = 0.5f; // Duration to ignore collisions

    // ***************************
    // * PUBLIC VARIABLES -> END *
    // ***************************

    // ******************************
    // * PRIVATE VARIABLES -> START *
    // ******************************

    private Rigidbody2D rb;
    private GameObject parent;   
    private bool TopWallCollsion = false; 
    [SerializeField] private AudioClip paddleCollisionClip;
    [SerializeField] private AudioClip brickCollisionClip;
    [SerializeField] private AudioClip wallCollisionClip;  

    private float ignoreTimer = 0f;

    private Collider2D otherCollider;


    // ****************************
    // * PRIVATE VARIABLES -> END *
    // ****************************


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parent = transform.parent.gameObject;
        rb = GetComponent<Rigidbody2D>();
        
        float x = UnityEngine.Random.Range(-1.0f,1.0f);
        float y = UnityEngine.Random.Range(0.5f,1.0f);

        Vector2 direction = new Vector2(x,y);
        rb.linearVelocity = direction.normalized * initialSpeed;

    }

    void Update()
    {
        if (ignoreTimer > 0)
        {
            ignoreTimer -= Time.deltaTime;
        }
        else if (otherCollider != null && ignoreTimer <= 0) 
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), otherCollider, false); // Re-enable collisions
            otherCollider = null;
        }
        
        rb.linearVelocity = rb.linearVelocity.normalized * initialSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // used for audio clips and ball paddle collision
        if (collision.gameObject.CompareTag("Side Wall"))
        {
            SoundFXManager.instance.PlaySoundFXClip(wallCollisionClip, transform, 1f);
        }
        if (collision.gameObject.CompareTag("Brick"))
        {
            SoundFXManager.instance.PlaySoundFXClip(brickCollisionClip, transform, 1f);
        }
        if (collision.gameObject.CompareTag("Top Wall"))
        {
            SoundFXManager.instance.PlaySoundFXClip(wallCollisionClip, transform, 1f);
        }
        if (collision.gameObject.CompareTag("Paddle"))
        {
            Debug.Log(transform.position);
            Debug.Log(collision.transform.position);
            SoundFXManager.instance.PlaySoundFXClip(paddleCollisionClip, transform, 1f);
            // Calculate how far from the center of the paddle the ball hit
            float hitPoint = (transform.position.x - collision.transform.position.x) / collision.collider.bounds.size.x;
            Debug.Log(hitPoint);
            
            // Calculate new angle based on hit point
            float bounceAngle = hitPoint * 60f; // 60 degrees max angle

            // Calculate new direction
            float angleInRadians = bounceAngle * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Sin(angleInRadians), Mathf.Cos(angleInRadians));
            // Apply the new velocity
            Debug.Log(direction.y);
            rb.linearVelocity = direction * initialSpeed;

            // Increment bounces
            parent.GetComponent<GameManager>().IncrementBounces(); 
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider, true);

            otherCollider = collision.collider;
            ignoreTimer = ignoreCollisionDuration;
        }
        // reducing paddle size by half *game feature*
        if (collision.gameObject.CompareTag("Top Wall") && !TopWallCollsion)
        {
            TopWallCollsion = true;
            if(parent != null){ 
                // TODO re-enable this feature. We're disabling it for now because it's causing issues in training.
                //   See issue #96 for details.
                // parent.GetComponent<GameManager>().UpdatePaddleSize();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "DeathZone")
        {
            if (parent != null){
                if (parent.GetComponent<GameManager>().IsTrainingMode)
                {
                    return; // let the Agent handle the ball in training mode
                }
                
                Vector2 pos = parent.GetComponent<GameManager>().GetBallStartingPosition();
                rb.MovePosition(pos);
                // objects moved with the above function can still collide, could cause issues but hasn't so far
                if (parent.GetComponent<GameManager>().LoseALife() <= 0)
                {
                    rb.linearVelocity = new Vector2(0f, 0f);
                }
                else
                {
                    float x = UnityEngine.Random.Range(-1.0f,1.0f);
                    float y = UnityEngine.Random.Range(0.5f,1.0f);

                    Vector2 direction = new Vector2(x,y);
                    rb.linearVelocity = direction.normalized * initialSpeed;
                }
            }
        }
        // if(other.gameObject.CompareTag("Paddle")){
        //     Debug.Log("test");
        // }   
    }   

    public void IncreaseBallSpeed(float amount){
        initialSpeed += amount;
    }
}
