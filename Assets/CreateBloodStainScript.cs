
using UnityEngine;

public class CreateBloodStainScript : MonoBehaviour
{
    public GameObject BloodSplash;
 
    void Start()
    {
   
    }

  public void CreateStain()
  {
    Instantiate(BloodSplash, new Vector3(transform.position.x,-0.584f,transform.position.z), Quaternion.identity);
  }

  public void DestroyObject() 
  {
    Destroy(this.gameObject);
  }
}
