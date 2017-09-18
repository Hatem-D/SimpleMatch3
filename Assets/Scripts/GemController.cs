using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GemController : MonoBehaviour {

    GameManager game;
    LevelController lC;

    private delegate void UpdateFunction();
    //UpdateFunction doUpdateStuff;
    Rigidbody myRB;
    //Collider myCollider;
    Material myMat;
    
    public GameObject deathEffectPrefab;    

    public int dynamicGemType;
    public int staticGemType = 0;    
    public bool raycasted = false;
    float destroyForce = 10000.0f;
    
    Vector3 randomTorque;

    delegate void gemClickFunctionPointer();
    gemClickFunctionPointer gemClicked;

    delegate void gemDeadFunctionPointer();
    gemDeadFunctionPointer gemDead;


    // Use this for initialization
    void Start()
    {
        randomTorque = new Vector3(Random.Range(0.0f, destroyForce), Random.Range(0.0f, destroyForce), Random.Range(0.0f, destroyForce));
        
        
        myRB = gameObject.GetComponent<Rigidbody>();
        //myCollider = gameObject.GetComponent<Collider>();
        
        game = GameObject.FindObjectOfType<GameManager>();

        if (game != null)
        {
            gemClicked = GameManagerGemClicked;
            gemDead = GameManagerGemDeath;
        }
        else
        {
            lC = transform.GetComponentInParent<LevelController>();
            if (lC != null)
            {
                dynamicGemType = staticGemType;
                lC.AddGemToLevelList(this);
                gemClicked = LevelControllerGemClicked;
                gemDead = LevelControllerGemdeath;
            }
        }


        //if (myTrail != null) myTrail.enabled = false;
        //myMat = gameObject.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    /*void Update()
    {
        if (doUpdateStuff != null)
            doUpdateStuff();
}*/

    public void Destroy()
    {
        Invoke("ByeBye", 10.0f);
        Invoke("FreeConstraints", 0.05f);
        
       
        myRB.constraints = RigidbodyConstraints.None;
        myRB.constraints = RigidbodyConstraints.FreezeRotation;
        myRB.AddForce(new Vector3(0.0f, 0.0f, destroyForce));
        
    }

    void FreeConstraints()
    {
        myRB.constraints = RigidbodyConstraints.None;
        myRB.AddRelativeTorque(randomTorque,ForceMode.Impulse);
        gameObject.layer = 8;
    }
    void ByeBye()
	{
		Destroy (gameObject);
	}

    public enum directions { up, down, left, right, neutral}
        

    public void OnGemClicked()
    {
        if (gemClicked != null && gameObject.GetComponent<Rigidbody>().velocity.y < 0.2f && gameObject.GetComponent<Rigidbody>().velocity.y > -0.2f) { 
            gemClicked();
        }
    }

    public void GameManagerGemClicked() {
        game.gemClick(this);
    }

    public void LevelControllerGemClicked() {
        lC.gemClick(this);
    }

    public void GameManagerGemDeath()
    {
        game.gemDeath(this.gameObject);
    }

    public void LevelControllerGemdeath()
    {
        lC.gemDeath(this.gameObject);
    }

    public void GetMeAndMySisters(directions directionToIgnore = directions.neutral)
    {
        RaycastHit hitUp, hitDown, hitLeft, hitRight;
        
        

        if (directionToIgnore != directions.up && (Physics.Raycast(transform.position, Vector3.up, out hitUp, 1.3f)))
        {
            GemController gemC = hitUp.collider.gameObject.GetComponent<GemController>();
            if (gemC != null && gemC.dynamicGemType == this.dynamicGemType && gemC.raycasted == false)
            {
                gemC.raycasted = true;
                gemC.GetMeAndMySisters(directions.down);
            }
        }

        if (directionToIgnore != directions.down && (Physics.Raycast(transform.position, Vector3.down, out hitDown, 1.3f)))
        {
            GemController gemC = hitDown.collider.gameObject.GetComponent<GemController>();
            if (gemC != null && gemC.dynamicGemType == this.dynamicGemType && gemC.raycasted == false)
            {
                gemC.raycasted = true;
                gemC.GetMeAndMySisters(directions.up);
            }
        }

        if (directionToIgnore != directions.left && (Physics.Raycast(transform.position, Vector3.left, out hitLeft, 1.3f)))
        {
            GemController gemC = hitLeft.collider.gameObject.GetComponent<GemController>();
            if (gemC != null && gemC.dynamicGemType == this.dynamicGemType && gemC.raycasted == false)
            {
                gemC.raycasted = true;
                gemC.GetMeAndMySisters(directions.right);
            }
        }

        if (directionToIgnore != directions.right &&(Physics.Raycast(transform.position, Vector3.right, out hitRight, 1.3f)))
        {
            GemController gemC = hitRight.collider.gameObject.GetComponent<GemController>();
            if (gemC != null && gemC.dynamicGemType == this.dynamicGemType && gemC.raycasted == false)
            {
                gemC.raycasted = true;
                gemC.GetMeAndMySisters(directions.left);
            }
        }

        if (gemDead != null)
            gemDead();

    }



    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 8) {
            //myCollider.enabled = !myCollider.enabled;
            //collision.gameObject.GetComponent<Renderer>().material = myMat;
            if (deathEffectPrefab != null)
            {
                GameObject dep = Instantiate(deathEffectPrefab);
                dep.transform.position = gameObject.transform.position;
                dep.transform.SetParent(gameObject.transform);
            }
        }
    }

}
