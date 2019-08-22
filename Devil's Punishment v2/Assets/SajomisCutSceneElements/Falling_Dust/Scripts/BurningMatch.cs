using UnityEngine;
using System.Collections;

public class BurningMatch : MonoBehaviour {

	public GameObject MatchMesh;
	public GameObject MatchAnim;
	public GameObject Flame;
	public ParticleSystem SmokeParticlesA;
	public ParticleSystem SmokeParticlesB;
	public ParticleSystem SmokeParticlesC;
	public AudioSource MatchLight;

    private float offset = 0;
    private Vector2 offsetVector;
    private Renderer matchMeshRenderer;

	private bool MatchLit = false;
	private bool SmokeParticlesA_On = false;
	private bool SmokeParticlesB_On = false;
	private bool SmokeParticlesC_On = false;

void Start (){

    Flame.SetActive(false);


	SmokeParticlesA.Stop();
    SmokeParticlesB.Stop();
    SmokeParticlesC.Stop();

	matchMeshRenderer = MatchMesh.GetComponent<Renderer>();
	
}  
  
  
void Update (){
 
	if (Input.GetButtonDown("Fire1")) //check to see if the left mouse was pressed - lights fuse
    {

	    if (MatchLit == false)
		{
		    MatchLight.Play ();          
			StartCoroutine ("Fuse");
		}
       
    }
            
}
 
 
IEnumerator Fuse (){

    MatchLit = true;



    // Start flame particle effects
    Flame.SetActive(true);

    // Play flame animatioms
	MatchAnim.GetComponent<Animation>().Play ();
    Flame.GetComponent<Animation>().Play ();

    yield  return new WaitForSeconds (0.2f);

    // Offset the UV set of the secondary map on the map to reveal the burnt match texture (just the tip at this stage)
    offset = 0.09f;

    while (offset < 0.1f)
    {  

        offset += (Time.deltaTime * 0.005f);
		Vector2 offsetVector = new Vector2 (offset, offset);

		matchMeshRenderer.material.SetTextureOffset ("_DetailAlbedoMap", offsetVector);

        yield return 0;

    }
			
	yield return new WaitForSeconds (5);
           
	while (offset < 0.43f)
	{  

	    // Continue offsetting the secondary match burnt texture over the match

    	offset += (Time.deltaTime * 0.0165f);
		Vector2 offsetVector = new Vector2 (offset, offset);
        matchMeshRenderer.material.SetTextureOffset ("_DetailAlbedoMap", offsetVector);

         // Trigger smoke particle effects as the burnt texture offsets over the match

      if (offset > 0.22f) 
      {
	      if (SmokeParticlesA_On == false)
		  {
		      SmokeParticlesA.Play ();
		  }

		  SmokeParticlesA_On = true;
      }


      if (offset > 0.27f) 
      {
		  if (SmokeParticlesB_On == false)
	      {
		      SmokeParticlesB.Play ();
		  }

		  SmokeParticlesB_On = true;
      }

      if (offset > 0.43f) 
      {
	      if (SmokeParticlesC_On == false)
		  {
		      SmokeParticlesC.Play ();
		  }

		  SmokeParticlesC_On = true;
      }
     
     	yield return 0;

    }
   
     
    offset = 0;

    Flame.SetActive(false);


}


}